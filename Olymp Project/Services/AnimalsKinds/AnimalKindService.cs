using Olymp_Project.Helpers.Validators;
using Olymp_Project.Models;
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

        public async Task<IServiceResponse<Animal>> InsertKindToAnimalAsync(
            long? animalId, long? kindId)
        {
            if (!IdValidator.IsValid(animalId, kindId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            var animalToUpdate = await _db.Animals
                .Include(a => a.VisitedLocations).Include(a => a.Kinds)             
                .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animalToUpdate is null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool kindExists = await _db.Kinds.AnyAsync(k => k.Id == kindId);
            if (!kindExists)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool IsKindInAnimal = animalToUpdate.Kinds.Any(k => k.Id == kindId);
            if (IsKindInAnimal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.Conflict);
            }

            try
            {
                return await AddKindToAnimalAndSave(animalToUpdate, kindId!.Value);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        private async Task<IServiceResponse<Animal>> AddKindToAnimalAndSave(
            Animal animalToUpdate, long kindId)
        {
            var kindToAdd = await _db.Kinds.FindAsync(kindId);
            animalToUpdate.Kinds.Add(kindToAdd!);

            _db.Animals.Attach(animalToUpdate);
            _db.Entry(animalToUpdate).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<Animal>(HttpStatusCode.Created, animalToUpdate);
        }

        public async Task<IServiceResponse<Animal>> UpdateAnimalKindAsync(
            long? animalId,
            PutAnimalKindDto request)
        {
            if (!IdValidator.IsValid(animalId, request.OldTypeId, request.NewTypeId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            var animalToUpdate = await _db.Animals
                .Include(a => a.VisitedLocations).Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animalToUpdate is null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            //bool kindsExist = _db.Kinds.Any(k => k.Id == request.OldKindId)
            //               && _db.Kinds.Any(k => k.Id == request.NewKindId);
            //if (!kindsExist)
            //{
            //    return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            //}

            bool kindExistsInAnimal = animalToUpdate.Kinds.Any(k => k.Id == request.OldTypeId);
            if (!kindExistsInAnimal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool alreadyContainsKinds = animalToUpdate.Kinds.Any(k => k.Id == request.NewTypeId)
                                     && animalToUpdate.Kinds.Any(k => k.Id == request.OldTypeId);
            if (alreadyContainsKinds)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.Conflict);
            }

            // Updating.
            var oldKind = animalToUpdate.Kinds.FirstOrDefault(k => k.Id == request.OldTypeId);
            if (oldKind == null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            var newKind = await _db.Kinds.FindAsync(request.NewTypeId!);
            if (newKind == null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            try
            {
                return await RemoveKindAndSaveChanges(animalToUpdate, oldKind, newKind);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        private async Task<IServiceResponse<Animal>> RemoveKindAndSaveChanges(
            Animal animal, Kind oldKind, Kind newKind)
        {
            animal.Kinds.Remove(oldKind);
            animal.Kinds.Add(newKind);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
        }

        public async Task<IServiceResponse<Animal>> RemoveAnimalKindAsync(long? animalId, long? kindId)
        {
            if (!IdValidator.IsValid(animalId, kindId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            var animalToUpdate = await _db.Animals.Include(a => a.VisitedLocations).Include(a => a.Kinds)
                                                  .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animalToUpdate is null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            if (animalToUpdate.Kinds.Count() == 1 && animalToUpdate.Kinds.First().Id == kindId)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            var kindExists = await _db.Kinds.FirstOrDefaultAsync(k => k.Id == kindId);
            if (kindExists is null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }
            // 1 - Тип животного с typeId не найден;
            // 2 - У животного с animalId нет типа с typeId.
            bool kindExistsInAnimal = animalToUpdate.Kinds.Any(a => a.Id == kindId);
            if (!kindExistsInAnimal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            try
            {
                return await RemoveAnimalKindById(animalToUpdate, kindId!.Value);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        private async Task<IServiceResponse<Animal>> RemoveAnimalKindById(Animal animal, long kindId)
        {
            var kindToRemove = _db.Kinds.First(a => a.Id == kindId);
            animal.Kinds.Remove(kindToRemove);

            _db.Animals.Attach(animal);
            _db.Entry(animal).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<Animal>(HttpStatusCode.OK, animal);
        }
    }
}
