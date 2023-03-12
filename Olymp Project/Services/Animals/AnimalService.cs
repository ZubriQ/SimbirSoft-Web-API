using Olymp_Project.Helpers.Validators;
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

        #region Get by id

        public async Task<IServiceResponse<Animal>> GetAnimalByIdAsync(long? animalId)
        {
            if (!IdValidator.IsValid(animalId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalAsync(animalId) is not Animal animal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
        }

        private async Task<Animal?> GetAnimalAsync(long? animalId)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        #endregion

        #region Get by search parameters

        public IServiceResponse<ICollection<Animal>> GetAnimals(AnimalQuery query, Paging paging)
        {
            if (!PagingValidator.IsValid(paging) || !AnimalDtoValidator.IsQueryValid(query))
            {
                return new CollectionServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            try
            {
                var filteredAnimals = GetAnimalsWithFilter(query);
                var pagedAnimals = PaginateAnimals(filteredAnimals, paging);
                return new CollectionServiceResponse<Animal>(HttpStatusCode.OK, pagedAnimals);
            }
            catch
            {
                return new CollectionServiceResponse<Animal>();
            }
        }

        private IQueryable<Animal> GetAnimalsWithFilter(AnimalQuery query)
        {
            var animals = GetAnimalsAsQueryable();
            animals = FilterByDateTime(animals, query.StartDateTime, query.EndDateTime);
            animals = FilterByChipping(animals, query.ChipperId, query.ChippingLocationId);
            animals = FilterByLifeStatusAndGender(animals, query.LifeStatus!, query.Gender!);

            return animals;
        }

        private IQueryable<Animal> GetAnimalsAsQueryable()
        {
            return _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .AsQueryable();
        }

        private IQueryable<Animal> FilterByDateTime(
            IQueryable<Animal> animals, DateTime? startDateTime, DateTime? endDateTime)
        {
            if (startDateTime.HasValue)
            {
                animals = animals.Where(a => a.ChippingDateTime >= startDateTime.Value);
            }
            if (endDateTime.HasValue)
            {
                animals = animals.Where(a => a.ChippingDateTime <= endDateTime.Value);
            }
            return animals;
        }

        private IQueryable<Animal> FilterByChipping(
            IQueryable<Animal> animals, int? chipperId, long? chippingLocationId)
        {
            if (chipperId.HasValue)
            {
                animals = animals.Where(a => a.ChipperId == chipperId.Value);
            }
            if (chippingLocationId.HasValue)
            {
                animals = animals.Where(a => a.ChippingLocationId == chippingLocationId.Value);
            }
            return animals;
        }

        private IQueryable<Animal> FilterByLifeStatusAndGender(
            IQueryable<Animal> animals, string lifeStatus, string gender)
        {
            if (!string.IsNullOrWhiteSpace(lifeStatus))
            {
                animals = animals.Where(a => a.LifeStatus == lifeStatus);
            }
            if (!string.IsNullOrWhiteSpace(gender))
            {
                animals = animals.Where(a => a.Gender == gender);
            }
            return animals;
        }

        private List<Animal> PaginateAnimals(IQueryable<Animal> animals, Paging paging)
        {
            return animals
                .OrderBy(a => a.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value)
                .ToList();
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
            
            if (await GetAnimalByIdAsync(animalId!.Value) is not Animal animalToUpdate ||
                !AccountAndLocationExist(request.ChipperId!.Value, request.ChippingLocationId!.Value))
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (!IsLifeStatusValid(animalToUpdate, request) ||
                !IsVisitedLocationsValid(animalToUpdate, request.ChippingLocationId.Value))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            return (HttpStatusCode.OK, animalToUpdate);
        }

        private async Task<Animal?> GetAnimalByIdAsync(long id)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        private bool AccountAndLocationExist(int chipperId, long chippingLocationId)
        {
            return _db.Accounts.Any(a => a.Id == chipperId) &&
                   _db.Locations.Any(l => l.Id == chippingLocationId);
        }

        private bool IsLifeStatusValid(Animal animal, PutAnimalDto request)
        {
            return animal.LifeStatus != "DEAD" || request.LifeStatus != "ALIVE";
        }

        private bool IsVisitedLocationsValid(Animal animal, long chippingLocationId)
        {
            return !animal.VisitedLocations.Any()
                 || animal.VisitedLocations.First().LocationId != chippingLocationId;
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
            (var statusCode, var animal) = await ValidateRemoveRequest(animalId);
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

        private async Task<(HttpStatusCode, Animal?)> ValidateRemoveRequest(long? animalId)
        {
            if (!IdValidator.IsValid(animalId))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            if (await GetAnimalAsync(animalId) is not Animal animal)
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
