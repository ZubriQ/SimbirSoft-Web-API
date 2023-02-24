using System.Net;
using Olymp_Project.Dtos.AnimalKind;

namespace Olymp_Project.Services.AnimalsKinds
{
    public class AnimalKindService : IAnimalKindService
    {
        private readonly ChipizationDbContext _db;

        public AnimalKindService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<(HttpStatusCode, Animal?)> InsertKindToAnimalAsync(
            long animalId,
            long kindId)
        {
            var animalToUpdate = await _db.Animals.Include(a => a.VisitedLocations).Include(a => a.Kinds)
                                                  .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animalToUpdate is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool kindExists = await _db.Kinds.AnyAsync(k => k.Id == kindId);
            if (!kindExists)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool IsKindInAnimal = animalToUpdate.Kinds.Any(k => k.Id == kindId);
            if (IsKindInAnimal)
            {
                return (HttpStatusCode.Conflict, null);
            }

            try
            {
                return await InsertKindToAnimalAndSave(animalToUpdate, kindId);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        private async Task<(HttpStatusCode, Animal?)> InsertKindToAnimalAndSave(
            Animal animalToUpdate,
            long kindId)
        {
            var kindToAdd = await _db.Kinds.FindAsync(kindId);
            animalToUpdate.Kinds.Add(kindToAdd!);

            _db.Animals.Attach(animalToUpdate);
            _db.Entry(animalToUpdate).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return (HttpStatusCode.Created, animalToUpdate);
        }

        public async Task<(HttpStatusCode, Animal?)> UpdateAnimalKindAsync(
            long animalId,
            PutAnimalKindDto request)
        {
            var animalToUpdate = await _db.Animals.Include(a => a.VisitedLocations).Include(a => a.Kinds)
                                                  .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animalToUpdate is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            // Validation.
            bool kindsExist = _db.Kinds.Any(k => k.Id == request.OldKindId)
                              && _db.Kinds.Any(k => k.Id == request.NewKindId);
            if (!kindsExist)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool kindExistsInAnimal = animalToUpdate.Kinds.Any(k => k.Id == request.OldKindId);
            if (!kindExistsInAnimal)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool alreadyContainsKinds = animalToUpdate.Kinds.Any(k => k.Id == request.NewKindId)
                                        && animalToUpdate.Kinds.Any(k => k.Id == request.OldKindId);
            if (alreadyContainsKinds)
            {
                return (HttpStatusCode.Conflict, null);
            }

            // Updating.
            var oldKind = animalToUpdate.Kinds.FirstOrDefault(k => k.Id == request.OldKindId);
            if (oldKind == null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            var newKind = await _db.Kinds.FindAsync(request.NewKindId!);
            if (newKind == null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            // TODO: optimize
            try
            {
                animalToUpdate.Kinds.Remove(oldKind);
                animalToUpdate.Kinds.Add(newKind);
                await _db.SaveChangesAsync();
                return (HttpStatusCode.OK, animalToUpdate);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<(HttpStatusCode, Animal?)> RemoveAnimalKindAsync(long animalId, long kindId)
        {
            var animalToUpdate = await _db.Animals.Include(a => a.VisitedLocations).Include(a => a.Kinds)
                                          .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animalToUpdate is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (animalToUpdate.Kinds.Count() == 1 && animalToUpdate.Kinds.First().Id == kindId)
            {
                return (HttpStatusCode.BadRequest, null);
            }

            // 1 - Тип животного с typeId не найден;
            // 2 - У животного с animalId нет типа с typeId.
            bool kindExistsInAnimal = animalToUpdate.Kinds.Any(a => a.Id == kindId);
            if (!kindExistsInAnimal)
            {
                return (HttpStatusCode.NotFound, null);
            }

            // TODO: optimize
            var kindToRemove = _db.Kinds.First(a => a.Id == kindId);
            animalToUpdate.Kinds.Remove(kindToRemove);

            try
            {
                _db.Animals.Attach(animalToUpdate);
                _db.Entry(animalToUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return (HttpStatusCode.OK, animalToUpdate);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }
    }
}
