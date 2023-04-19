using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.AnimalsKinds
{
    public class AnimalKindService : IAnimalKindService
    {
        private readonly ChipizationDbContext _db;

        public AnimalKindService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Insert

        public async Task<IServiceResponse<Animal>> InsertKindToAnimalAsync(
            long? animalId, long? kindId)
        {
            if (!IdValidator.IsValid(animalId, kindId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalByIdAsync(animalId!.Value) is not Animal animalToUpdate ||
                !await _db.Kinds.AnyAsync(k => k.Id == kindId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool IsKindInAnimal = animalToUpdate.Kinds.Any(k => k.Id == kindId);
            if (IsKindInAnimal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.Conflict);
            }

            return await AddKindToAnimalAndSaveChangesAsync(animalToUpdate, kindId!.Value);
        }

        private async Task<Animal?> GetAnimalByIdAsync(long id)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        private async Task<IServiceResponse<Animal>> AddKindToAnimalAndSaveChangesAsync(
            Animal animalToUpdate, long kindId)
        {
            try
            {
                var kindToAdd = await _db.Kinds.FindAsync(kindId);
                animalToUpdate.Kinds.Add(kindToAdd!);

                _db.Animals.Attach(animalToUpdate);
                _db.Entry(animalToUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return new ServiceResponse<Animal>(HttpStatusCode.Created, animalToUpdate);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        #endregion

        #region Update

        public async Task<IServiceResponse<Animal>> UpdateAnimalKindAsync(
            long? animalId, PutAnimalKindDto request)
        {
            (var statusCode, var animalToUpdate) = await ValidateAnimalInUpdateAnimalKindRequest(animalId, request);
            if (statusCode is not HttpStatusCode.OK)
            {
                return new ServiceResponse<Animal>(statusCode);
            }

            if (animalToUpdate!.Kinds.FirstOrDefault(k => k.Id == request.OldKindId) is not Kind oldKind ||
                await _db.Kinds.FindAsync(request.NewKindId!) is not Kind newKind)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            return await UpdateKindInAnimalAndSaveChangesAsync(animalToUpdate, oldKind, newKind);
        }

        private async Task<(HttpStatusCode, Animal?)> ValidateAnimalInUpdateAnimalKindRequest(
            long? animalId, PutAnimalKindDto request)
        {
            if (!IdValidator.IsValid(animalId, request.OldKindId, request.NewKindId))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            if (await GetAnimalByIdAsync(animalId!.Value) is not Animal animalToUpdate ||
                !KindExistsInAnimal(animalToUpdate, request.OldKindId!.Value))
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (AnimalAlreadyContainsKinds(animalToUpdate, request.NewKindId!.Value, request.OldKindId.Value))
            {
                return (HttpStatusCode.Conflict, null);
            }
            return (HttpStatusCode.OK, animalToUpdate);
        }

        private bool KindExistsInAnimal(Animal animal, long kindId)
        {
            return animal.Kinds.Any(k => k.Id == kindId);
        }

        private bool AnimalAlreadyContainsKinds(Animal animal, long newTypeId, long oldTypeId)
        {
            return animal.Kinds.Any(k => k.Id == newTypeId) &&
                   animal.Kinds.Any(k => k.Id == oldTypeId);
        }

        private async Task<IServiceResponse<Animal>> UpdateKindInAnimalAndSaveChangesAsync(
            Animal animal, Kind oldKind, Kind newKind)
        {
            try
            {
                animal.Kinds.Remove(oldKind);
                animal.Kinds.Add(newKind);
                await _db.SaveChangesAsync();

                return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        #endregion

        #region Remove

        public async Task<IServiceResponse<Animal>> RemoveAnimalKindAsync(long? animalId, long? kindId)
        {
            (var statusCode, var animalToRemoveFrom) = await ValidateRemoveAnimalKindRequest(animalId, kindId);
            if (statusCode is not HttpStatusCode.OK)
            {
                return new ServiceResponse<Animal>(statusCode);
            }

            return await RemoveKindFromAnimalByIdsAsync(animalToRemoveFrom!, kindId!.Value);
        }

        private async Task<(HttpStatusCode, Animal?)> ValidateRemoveAnimalKindRequest(long? animalId, long? kindId)
        {
            if (!IdValidator.IsValid(animalId, kindId))
            {
                return (HttpStatusCode.BadRequest, null);
            }

            if (await GetAnimalByIdAsync(animalId!.Value) is not Animal animalToUpdate)
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (animalToUpdate.Kinds.Count() == 1 && animalToUpdate.Kinds.First().Id == kindId)
            {
                return (HttpStatusCode.BadRequest, null);
            }

            var kindExistsInDatabase = await _db.Kinds.AnyAsync(k => k.Id == kindId);
            if (!kindExistsInDatabase || !KindExistsInAnimal(animalToUpdate, kindId!.Value))
            {
                return (HttpStatusCode.NotFound, null);
            }
            return (HttpStatusCode.OK, animalToUpdate);
        }

        private async Task<IServiceResponse<Animal>> RemoveKindFromAnimalByIdsAsync(Animal animal, long kindId)
        {
            try
            {
                var kindToRemove = _db.Kinds.First(a => a.Id == kindId);
                animal.Kinds.Remove(kindToRemove);
                _db.Animals.Attach(animal);
                _db.Entry(animal).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        #endregion
    }
}
