using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.VisitedLocations
{
    public class VisitedLocationService : IVisitedLocationService
    {
        private readonly ChipizationDbContext _db;

        public VisitedLocationService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get VisitedLocations by search parameters

        public async Task<IServiceResponse<ICollection<VisitedLocation>>> GetVisitedLocationsAsync(
            long? animalId,
            DateTimeRangeQuery query,
            Paging paging)
        {
            if (!IdValidator.IsValid(animalId) || !PagingValidator.IsValid(paging))
            {
                return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal)
            {
                return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            var visitedLocations = FilterVisitedLocations(animal.VisitedLocations, query, paging);
            return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.OK, visitedLocations!);
        }

        private async Task<Animal?> GetAnimalByIdWithVisitedLocationsAsync(long animalId)
        {
            return await _db.Animals
                .Include(a => a.VisitedLocations)
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        private List<VisitedLocation> FilterVisitedLocations(
            IEnumerable<VisitedLocation> visitedLocations, DateTimeRangeQuery query, Paging paging)
        {
            visitedLocations = visitedLocations
                .Where(v => query.StartDateTime is null || v.VisitDateTime >= query.StartDateTime)
                .Where(v => query.EndDateTime is null || v.VisitDateTime <= query.EndDateTime)
                .OrderBy(l => l.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value);
            return visitedLocations.ToList();
        }

        #endregion

        #region Insert

        public async Task<IServiceResponse<VisitedLocation>> InsertVisitedLocationAsync(
            long? animalId, 
            long? locationId)
        {
            if (!IdValidator.IsValid(animalId, locationId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (!AnimalValidator.IsExistingAnimalValid(animal, locationId!.Value))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await _db.Locations.FirstOrDefaultAsync(l => l.Id == locationId) is not Location location)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            try
            {
                return await AddVisitedLocationAsync(animal, location);
            }
            catch (Exception)
            {
                return new ServiceResponse<VisitedLocation>();
            }
        }

        private async Task<IServiceResponse<VisitedLocation>> AddVisitedLocationAsync(
            Animal animal, Location location)
        {
            VisitedLocation visitedLocation = new()
            {
                Animal = animal,
                Location = location,
                VisitDateTime = DateTime.Now
            };
            _db.VisitedLocations.Add(visitedLocation);
            await _db.SaveChangesAsync();
            return new ServiceResponse<VisitedLocation>(HttpStatusCode.Created, visitedLocation);
        }

        #endregion

        #region Update

        public async Task<IServiceResponse<VisitedLocation>> UpdateVisitedLocationAsync(
            long? animalId,
            VisitedLocationRequestDto request)
        {
            #region Request Validation

            if (!IdValidator.IsValid(animalId, request.VisitedLocationPointId, request.LocationPointId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (await _db.Locations.FirstOrDefaultAsync(l => l.Id == request.LocationPointId) is not Location location)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            var visitedLocationToUpdate = animal.VisitedLocations
                .FirstOrDefault(vl => vl.Id == request.VisitedLocationPointId);
            if (visitedLocationToUpdate is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (!AnimalValidator.IsVisitedLocationValid(
                animal, visitedLocationToUpdate, request.LocationPointId!.Value))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            var hasVisitedLocation = animal.VisitedLocations
                .FirstOrDefault(vl => vl.LocationId == visitedLocationToUpdate.LocationId);
            if (hasVisitedLocation is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (!AnimalValidator.IsAdjacentLocationsValid(
                animal, request.LocationPointId!.Value, visitedLocationToUpdate.Id))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            #endregion

            try
            {
                return await UpdateVisitedLocation(visitedLocationToUpdate!, location!);
            }
            catch (Exception)
            {
                return new ServiceResponse<VisitedLocation>();
            }
        }

        private async Task<IServiceResponse<VisitedLocation>> UpdateVisitedLocation(
            VisitedLocation visitedLocation, Location location)
        {
            visitedLocation.Location = location;
            _db.VisitedLocations.Attach(visitedLocation);
            _db.Entry(visitedLocation).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<VisitedLocation>(HttpStatusCode.OK, visitedLocation);
        }

        #endregion

        #region Remove

        public async Task<HttpStatusCode> RemoveVisitedLocationAsync(
            long? animalId,
            long? visitedLocationId)
        {
            if (!IdValidator.IsValid(animalId, visitedLocationId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await GetAnimalByIdWithVisitedLocationsAsync(animalId!.Value) is not Animal animal)
            {
                return HttpStatusCode.NotFound;
            }

            bool visitedLocationExists = await _db.VisitedLocations.AnyAsync(vl => vl.Id == visitedLocationId);
            if (!visitedLocationExists)
            {
                return HttpStatusCode.NotFound;
            }

            var visitedLocationToDelete = animal.VisitedLocations
                .FirstOrDefault(vl => vl.Id == visitedLocationId);
            if (visitedLocationToDelete is null)
            {
                return HttpStatusCode.NotFound;
            }

            try
            {
                return await RemoveVisitedLocationAsync(animal, visitedLocationToDelete);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<HttpStatusCode> RemoveVisitedLocationAsync(
            Animal animal, VisitedLocation visitedLocationToDelete)
        {
            _db.VisitedLocations.Remove(visitedLocationToDelete);
            animal.VisitedLocations.Remove(visitedLocationToDelete);
            RemoveVisitedLocationIfMatchesChippingLocation(animal);

            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        private void RemoveVisitedLocationIfMatchesChippingLocation(Animal animal)
        {
            var newFirstLocation = animal.VisitedLocations
                .OrderBy(vl => vl.VisitDateTime)
                .FirstOrDefault();

            if (newFirstLocation is not null &&
                newFirstLocation.LocationId == animal.ChippingLocationId)
            {
                _db.VisitedLocations.Remove(newFirstLocation);
                animal.VisitedLocations.Remove(newFirstLocation);
            }
        }

        #endregion
    }
}
