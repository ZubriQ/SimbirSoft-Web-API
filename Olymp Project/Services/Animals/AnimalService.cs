using Microsoft.EntityFrameworkCore;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Models;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.Animals
{
    public class AnimalService : IAnimalService
    {
        private readonly ChipizationDbContext _db;

        public AnimalService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get Animal by id

        public async Task<IServiceResponse<Animal>> GetAnimalByIdAsync(long? animalId)
        {
            if (!IdValidator.IsValid(animalId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalById(animalId) is not Animal animal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
        }

        private async Task<Animal?> GetAnimalById(long? animalId)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        #endregion

        #region Get Animals by search parameters

        public async Task<IServiceResponse<ICollection<Animal>>> GetAnimalsAsync(
            AnimalQuery query, 
            Paging paging)
        {
            if (!PagingValidator.IsValid(paging) || !AnimalDtoValidator.IsQueryValid(query))
            {
                return new CollectionServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            try
            {
                var filteredAnimals = GetFilteredAnimals(query, paging);
                return new CollectionServiceResponse<Animal>(HttpStatusCode.OK, filteredAnimals);
            }
            catch
            {
                return new CollectionServiceResponse<Animal>();
            }
        }

        private IQueryable<Animal> GetFilteredAnimals(AnimalQuery query, Paging paging)
        {
            var animals = _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .AsQueryable();

            animals = FilterAnimals(query, animals);

            return animals
                .OrderBy(a => a.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value);
        }

        private IQueryable<Animal> FilterAnimals(AnimalQuery query, IQueryable<Animal> animals)
        {
            if (query.StartDateTime.HasValue)
            {
                animals = animals.Where(a => a.ChippingDateTime >= query.StartDateTime);
            }

            if (query.EndDateTime.HasValue)
            {
                animals = animals.Where(a => a.ChippingDateTime <= query.EndDateTime);
            }

            if (query.ChipperId.HasValue)
            {
                animals = animals.Where(a => a.ChipperId == query.ChipperId.Value);
            }

            if (query.ChippingLocationId.HasValue)
            {
                animals = animals.Where(a => a.ChippingLocationId == query.ChippingLocationId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.LifeStatus))
            {
                animals = animals.Where(a => a.LifeStatus == query.LifeStatus);
            }

            if (!string.IsNullOrWhiteSpace(query.Gender))
            {
                animals = animals.Where(a => a.Gender == query.Gender);
            }

            return animals;
        }

        #endregion

        #region Insert

        public async Task<IServiceResponse<Animal>> InsertAnimalAsync(Animal animal)
        {
            var statusCode = ValidateInsertAnimal(animal);
            if (statusCode is not HttpStatusCode.OK)
            {
                return new ServiceResponse<Animal>(statusCode);
            }

            try
            {
                return await AddAnimal(animal);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        private HttpStatusCode ValidateInsertAnimal(Animal animal)
        {
            if (!AnimalDtoValidator.IsRequestValid(animal))
            {
                return HttpStatusCode.BadRequest;
            }

            bool kindExistsInAnimal = animal.Kinds.All(kind => _db.Kinds.Any(k => k.Id == kind.Id));
            if (!kindExistsInAnimal)
            {
                return HttpStatusCode.NotFound;
            }

            bool accountExists = _db.Accounts.Any(a => a.Id == animal.ChipperId);
            if (!accountExists)
            {
                return HttpStatusCode.NotFound;
            }

            bool locationExists = _db.Locations.Any(l => l.Id == animal.ChippingLocationId);
            if (!locationExists)
            {
                return HttpStatusCode.NotFound;
            }

            bool hasDuplicates = animal.Kinds.Count != animal.Kinds.Distinct().Count();
            if (hasDuplicates)
            {
                return HttpStatusCode.Conflict;
            }

            return HttpStatusCode.OK;
        }

        private async Task<IServiceResponse<Animal>> AddAnimal(Animal animal)
        {
            await InitializeKinds(animal);
            _db.Animals.Add(animal);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Animal>(HttpStatusCode.Created, animal);
        }

        private async Task InitializeKinds(Animal animal)
        {
            List<Kind> kinds = new List<Kind>();
            foreach (var kind in animal.Kinds)
            {
                var newKind = await _db.Kinds.FindAsync(kind.Id);
                if (newKind is not null)
                {
                    kinds.Add(newKind);
                }
            }
            animal.Kinds = kinds;
        }

        #endregion

        #region Update

        public async Task<IServiceResponse<Animal>> UpdateAnimalAsync(long? animalId, PutAnimalDto request)
        {
            (var statusCode, var animalToUpdate) = await ValidateUpdateAnimalAsync(animalId, request);
            if (statusCode is not HttpStatusCode.OK)
            {
                return new ServiceResponse<Animal>(statusCode);
            }

            try
            {
                UpdateAnimal(animalToUpdate!, request);
                await _db.SaveChangesAsync();
                return new ServiceResponse<Animal>(HttpStatusCode.OK, animalToUpdate!);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        private async Task<(HttpStatusCode, Animal?)> ValidateUpdateAnimalAsync(long? animalId, PutAnimalDto request)
        {
            if (!IdValidator.IsValid(animalId) || !AnimalDtoValidator.IsRequestValid(request))
            {
                return (HttpStatusCode.BadRequest, null);
            }
            
            if (await GetAnimalByIdAsync(animalId!.Value) is not Animal animalToUpdate)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool chipperExists = _db.Accounts.Any(a => a.Id == request.ChipperId);
            if (!chipperExists)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool locationExists = _db.Locations.Any(l => l.Id == request.ChippingLocationId);
            if (!locationExists)
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (animalToUpdate.LifeStatus == "DEAD" && request.LifeStatus == "ALIVE")
            {
                return (HttpStatusCode.BadRequest, null);
            }

            if (animalToUpdate.VisitedLocations.Any()
                && animalToUpdate.VisitedLocations.First().LocationId == request.ChippingLocationId)
            {
                return (HttpStatusCode.BadRequest, null);
            }

            return (HttpStatusCode.OK, animalToUpdate);
        }

        //private async Task<(bool, Animal?)> ValidateUpdateRequest(long animalId, PutAnimalDto request)
        //{
        //    if (await GetAnimalByIdAsync(animalId) is not Animal animalToUpdate)
        //    {
        //        return false;
        //    }

        //    bool chipperExists = _db.Accounts.Any(a => a.Id == request.ChipperId);
        //    if (!chipperExists)
        //    {
        //        return false;
        //    }

        //    bool locationExists = _db.Locations.Any(l => l.Id == request.ChippingLocationId);
        //    if (!locationExists)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        private async Task<Animal?> GetAnimalByIdAsync(long id)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        private void UpdateAnimal(Animal animal, PutAnimalDto newData)
        {
            animal.Weight = newData.Weight!.Value;
            animal.Length = newData.Length!.Value;
            animal.Height = newData.Height!.Value;
            animal.Gender = newData.Gender!;
            animal.ChipperId = newData.ChipperId!.Value;
            animal.ChippingLocationId = newData.ChippingLocationId!.Value;
            if (animal.LifeStatus == "ALIVE" && newData.LifeStatus == "DEAD")
            {
                animal.DeathDateTime = DateTime.Now;
                animal.LifeStatus = newData.LifeStatus!;
            }
        }

        #endregion

        #region Remove

        public async Task<HttpStatusCode> RemoveAnimalAsync(long? animalId)
        {
            (var statusCode, var animal) = await ValidateRemoving(animalId);
            if (statusCode is not HttpStatusCode.OK)
            {
                return statusCode;
            }

            try
            {
                return await RemoveAnimalAndSaveChanges(animal!);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<(HttpStatusCode, Animal?)> ValidateRemoving(long? animalId)
        {
            if (!IdValidator.IsValid(animalId))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            var animal = await _db.Animals.Include(k => k.Kinds).Include(vl => vl.VisitedLocations)
                .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (animal.VisitedLocations.Any(vl => vl.LocationId != animal.ChippingLocationId))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            return (HttpStatusCode.OK, animal);
        }

        private async Task<HttpStatusCode> RemoveAnimalAndSaveChanges(Animal animal)
        {
            _db.Animals.Remove(animal);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        #endregion
    }
}
