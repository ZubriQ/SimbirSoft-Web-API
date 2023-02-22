using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Models;
using System.Linq;
using System.Net;

namespace Olymp_Project.Services.Animals
{
    public class AnimalService : IAnimalService
    {
        private readonly ChipizationDbContext _db;

        public AnimalService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Animal

        public async Task<Animal?> GetAnimalAsync(long id)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IQueryable<Animal>> GetAnimalsAsync(AnimalQuery query, Paging paging)
        {
            try
            {
                return GetFilteredAnimals(query, paging);
            }
            catch
            {
                return new List<Animal>().AsQueryable();
            }
        }

        private IQueryable<Animal> GetFilteredAnimals(AnimalQuery query, Paging paging)
        {
            // TODO: Task or ValueTask?
            var animals = _db.Animals
                .Include(a => a.VisitedLocations)
                .Include(a => a.Kinds)
                .AsQueryable();

            animals = FilterAnimals(query, animals);

            return animals
                .OrderBy(a => a.Id)
                .Skip(paging.From.Value)
                .Take(paging.Size.Value);
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

        public async Task<(HttpStatusCode, Animal?)> InsertAnimalAsync(Animal animal)
        {
            bool hasDuplicates = animal.Kinds.Count != animal.Kinds.Distinct().Count();
            if (hasDuplicates)
            {
                return (HttpStatusCode.Conflict, null);
            }

            bool kindsExist = _db.Kinds.All(k => animal.Kinds.Select(n => n.Id).Contains(k.Id));
            if (kindsExist)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool accountExists = _db.Accounts.Any(a => a.Id == animal.ChipperId);
            if (!accountExists)
            {
                return (HttpStatusCode.NotFound, null);
            }

            bool locationExists = _db.Locations.Any(l => l.Id == animal.ChippingLocationId);
            if (!locationExists)
            {
                return (HttpStatusCode.NotFound, null);
            }

            try
            {
                return await InsertAnimal(animal);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        private async Task<(HttpStatusCode, Animal?)> InsertAnimal(Animal animal)
        {
            await InitializeKinds(animal);

            _db.Animals.Add(animal);
            await _db.SaveChangesAsync();
            return (HttpStatusCode.Created, animal);
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

        public async Task<(HttpStatusCode, Animal?)> UpdateAnimalAsync(long id, PutAnimalDto request)
        {
            #region Validation
            var animalToUpdate = await _db.Animals.FindAsync(id);
            if (animalToUpdate is null)
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
                && animalToUpdate.VisitedLocations.First().Location.Id != request.ChippingLocationId)
            {
                return (HttpStatusCode.BadRequest, null);
            }
            #endregion

            UpdateAnimal(animalToUpdate, request);
            await _db.SaveChangesAsync();
            return (HttpStatusCode.OK, animalToUpdate);
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
            var animal = await _db.Animals.FindAsync(id);
            if (animal is null)
            {
                return HttpStatusCode.NotFound;
            }
            // TODO: Животное покинуло локацию чипирования, при этом
            //       есть другие посещенные точки
            // return HttpStatusCode.BadRequest;
            // TODO: 401
            _db.Animals.Remove(animal);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        #endregion

        #region Animal's kinds

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

        #endregion
    }
}
