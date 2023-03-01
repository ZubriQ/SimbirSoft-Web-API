using System.Net;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Dtos.AnimalKind;
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
            long animalId,
            long kindId)
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
                return await AddKindToAnimalAndSave(animalToUpdate, kindId);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        private async Task<IServiceResponse<Animal>> AddKindToAnimalAndSave(
            Animal animalToUpdate,
            long kindId)
        {
            var kindToAdd = await _db.Kinds.FindAsync(kindId);
            animalToUpdate.Kinds.Add(kindToAdd!);

            _db.Animals.Attach(animalToUpdate);
            _db.Entry(animalToUpdate).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<Animal>(HttpStatusCode.Created, animalToUpdate);
        }

        public async Task<IServiceResponse<Animal>> UpdateAnimalKindAsync(
            long animalId,
            PutAnimalKindDto request)
        {
            if (!IdValidator.IsValid(animalId, request.OldKindId, request.NewKindId))
            {
                return new ServiceResponse<Animal>(HttpStatusCode.BadRequest);
            }

            var animalToUpdate = await _db.Animals.Include(a => a.VisitedLocations).Include(a => a.Kinds)
                                                  .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animalToUpdate is null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool kindsExist = _db.Kinds.Any(k => k.Id == request.OldKindId)
                           && _db.Kinds.Any(k => k.Id == request.NewKindId);
            if (!kindsExist)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool kindExistsInAnimal = animalToUpdate.Kinds.Any(k => k.Id == request.OldKindId);
            if (!kindExistsInAnimal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            bool alreadyContainsKinds = animalToUpdate.Kinds.Any(k => k.Id == request.NewKindId)
                                        && animalToUpdate.Kinds.Any(k => k.Id == request.OldKindId);
            if (alreadyContainsKinds)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.Conflict);
            }

            // Updating.
            var oldKind = animalToUpdate.Kinds.FirstOrDefault(k => k.Id == request.OldKindId);
            if (oldKind == null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            var newKind = await _db.Kinds.FindAsync(request.NewKindId!);
            if (newKind == null)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            // TODO: optimize
            try
            {
                animalToUpdate.Kinds.Remove(oldKind);
                animalToUpdate.Kinds.Add(newKind);
                await _db.SaveChangesAsync();
                return new ServiceResponse<Animal>(HttpStatusCode.OK, animalToUpdate);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }

        public async Task<IServiceResponse<Animal>> RemoveAnimalKindAsync(long animalId, long kindId)
        {
            if (!IdValidator.IsValid(animalId) || !IdValidator.IsValid(kindId))
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

            // 1 - Тип животного с typeId не найден;
            // 2 - У животного с animalId нет типа с typeId.
            bool kindExistsInAnimal = animalToUpdate.Kinds.Any(a => a.Id == kindId);
            if (!kindExistsInAnimal)
            {
                return new ServiceResponse<Animal>(HttpStatusCode.NotFound);
            }

            // TODO: optimize
            var kindToRemove = _db.Kinds.First(a => a.Id == kindId);
            animalToUpdate.Kinds.Remove(kindToRemove);
            try
            {
                _db.Animals.Attach(animalToUpdate);
                _db.Entry(animalToUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return new ServiceResponse<Animal>(HttpStatusCode.OK, animalToUpdate);
            }
            catch (Exception)
            {
                return new ServiceResponse<Animal>();
            }
        }
    }
}
