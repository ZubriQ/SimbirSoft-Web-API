using Olymp_Project.Controllers.Validators;
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

        public async Task<IServiceResponse<Animal>> GetAnimalAsync(long id)
        {
            if (!IdValidator.IsValid(id))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            var animal = await _db.Animals
                .Include(a => a.VisitedLocations).Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal is null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }
            return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
        }

        public async Task<IServiceResponse<ICollection<Animal>>> GetAnimalsAsync(
            AnimalQuery query, 
            Paging paging)
        {
            // TODO: CHECK THE DATETIMES.
            if (!PagingValidator.IsValid(paging) || !AnimalValidator.IsQueryValid(query))
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
                .Skip(paging.Skip!.Value)
                .Take(paging.Take!.Value);
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

        public async Task<IServiceResponse<Animal>> InsertAnimalAsync(Animal animal)
        {
            if (!AnimalValidator.IsRequestValid(animal))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.Conflict);
            }

            bool hasDuplicates = animal.Kinds.Count != animal.Kinds.Distinct().Count();
            if (hasDuplicates)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.Conflict);
            }

            bool kindsExist = _db.Kinds.All(k => animal.Kinds.Select(n => n.Id).Contains(k.Id));
            if (kindsExist)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool accountExists = _db.Accounts.Any(a => a.Id == animal.ChipperId);
            if (!accountExists)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool locationExists = _db.Locations.Any(l => l.Id == animal.ChippingLocationId);
            if (!locationExists)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
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

        private async Task<IServiceResponse<Animal>> AddAnimal(Animal animal)
        {
            await InitializeKinds(animal);

            _db.Animals.Add(animal);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
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

        public async Task<IServiceResponse<Animal>> UpdateAnimalAsync(long id, PutAnimalDto request)
        {
            #region Validation
            if (!IdValidator.IsValid(id) || !AnimalValidator.IsRequestValid(request))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            var animalToUpdate = await _db.Animals.FindAsync(id);
            if (animalToUpdate is null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool chipperExists = _db.Accounts.Any(a => a.Id == request.ChipperId);
            if (!chipperExists)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool locationExists = _db.Locations.Any(l => l.Id == request.ChippingLocationId);
            if (!locationExists)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            if (animalToUpdate.LifeStatus == "DEAD" && request.LifeStatus == "ALIVE")
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            if (animalToUpdate.VisitedLocations.Any()
                && animalToUpdate.VisitedLocations.First().Location.Id != request.ChippingLocationId)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }
            #endregion

            try
            {
                UpdateAnimal(animalToUpdate, request);
                await _db.SaveChangesAsync();
                return new ServiceResponse<Animal>(HttpStatusCode.OK, animalToUpdate);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        private void UpdateAnimal(Animal animal, PutAnimalDto newData)
        {
            animal.Weight = newData.Weight!.Value;
            animal.Length = newData.Length!.Value;
            animal.Height = newData.Height!.Value;
            animal.Gender = newData.Gender!;
            animal.ChipperId = newData.ChipperId!.Value;
            animal.ChippingLocationId = newData.ChippingLocationId!.Value;
            animal.LifeStatus = newData.LifeStatus!;
        }

        public async Task<HttpStatusCode> RemoveAnimalAsync(long id)
        {
            if (!IdValidator.IsValid(id))
            {
                return HttpStatusCode.BadRequest;
            }

            var animal = await _db.Animals.FindAsync(id);
            if (animal is null)
            {
                return HttpStatusCode.NotFound;
            }
            // TODO: Животное покинуло локацию чипирования, при этом
            //       есть другие посещенные точки
            // return HttpStatusCode.BadRequest;
            // TODO: Works?
            if (animal.VisitedLocations.Any(vl => vl.Location.Id != animal.ChippingLocation.Id))
            {
                return HttpStatusCode.BadRequest;
            }

            try
            {
                _db.Animals.Remove(animal);
                await _db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
