using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

namespace Olymp_Project.Services.VisitedLocations
{
    public class VisitedLocationService : IVisitedLocationService
    {
        private readonly ChipizationDbContext _db;

        public VisitedLocationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<IServiceResponse<ICollection<VisitedLocation>>> GetVisitedLocationsAsync(
            long? animalId,
            DateTimeRangeQuery query,
            Paging paging)
        {
            if (!IdValidator.IsValid(animalId) || !PagingValidator.IsValid(paging))
            {
                return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            var animal = await _db.Animals.Include(a => a.VisitedLocations)
                                          .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal is null)
            {
                return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            var locations = animal.VisitedLocations.ToList();

            // TODO: CHECK DATES FOR ISO-8601. return 400.
            if (query.StartDateTime is not null)
            {
                locations = locations.Where(v => v.VisitDateTime >= query.StartDateTime).ToList();
            }

            if (query.EndDateTime is not null)
            {
                locations = locations.Where(v => v.VisitDateTime <= query.EndDateTime).ToList();
            }

            locations = locations.OrderBy(l => l.Id)
                                 .Skip(paging.From!.Value)
                                 .Take(paging.Size!.Value)
                                 .ToList();
            return new CollectionServiceResponse<VisitedLocation>(HttpStatusCode.OK, locations);
        }

        public async Task<IServiceResponse<VisitedLocation>> InsertVisitedLocationAsync(
            long? animalId, 
            long? locationId)
        {
            if (!IdValidator.IsValid(animalId, locationId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            var animal = await _db.Animals.Include(a => a.VisitedLocations)
                                          .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (animal.LifeStatus == "DEAD")
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            // Животное находится в точке чипирования и никуда не перемещалось,
            // попытка добавить точку локации, равную точке чипирования.
            if (animal.ChippingLocationId == locationId && animal.VisitedLocations.Count == 0)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            // Попытка добавить точку локации, в которой уже находится животное.
            var lastVisitedLocation = animal.VisitedLocations
                .OrderByDescending(l => l.VisitDateTime)
                .FirstOrDefault();
            if (lastVisitedLocation != null && lastVisitedLocation.LocationId == locationId)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            var location = await _db.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
            if (location is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            try
            {
                return await AddVisitedLocation(animal, location);
            }
            catch (Exception)
            {
                return new ServiceResponse<VisitedLocation>();
            }
        }

        private async Task<IServiceResponse<VisitedLocation>> AddVisitedLocation(
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

        public async Task<IServiceResponse<VisitedLocation>> UpdateVisitedLocationAsync(
            long? animalId,
            VisitedLocationRequestDto request)
        {
            #region Validation

            if (!IdValidator.IsValid(animalId, request.VisitedLocationPointId, request.LocationPointId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            var animal = await _db.Animals
                .Include(a => a.Kinds)
                .Include(a => a.VisitedLocations)
                .ThenInclude(vl => vl.Location)
                .FirstOrDefaultAsync(a => a.Id == animalId);

            if (animal == null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            var existingLocation = await _db.Locations
                .FirstOrDefaultAsync(l => l.Id == request.LocationPointId);
            if (existingLocation is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (existingLocation is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            var visitedLocationToUpdate = animal.VisitedLocations
                .FirstOrDefault(vl => vl.Id == request.VisitedLocationPointId);

            if (visitedLocationToUpdate is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (visitedLocationToUpdate.LocationId == request.LocationPointId)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            var isVisited = animal.VisitedLocations
                .FirstOrDefault(v => v.LocationId == visitedLocationToUpdate.LocationId);

            if (isVisited is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            if (visitedLocationToUpdate == animal.VisitedLocations.Last() &&
                animal.ChippingLocationId == request.LocationPointId)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            //var locations = animal.VisitedLocations
            //    .OrderByDescending(vl => vl.VisitDateTime).ToList();

            //int currentIndex = locations.IndexOf(visitedLocationToUpdate);

            //if (currentIndex == -1)
            //{
            //    return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            //}

            //var nextVisitedLocation = currentIndex + 1 < locations.Count
            //    ? locations[currentIndex + 1]
            //    : null;

            //var previousVisitedLocation = currentIndex - 1 >= 0
            //    ? locations[currentIndex - 1]
            //    : null;

            //if (nextVisitedLocation?.LocationId == request.LocationPointId ||
            //    previousVisitedLocation?.LocationId == request.LocationPointId)
            //{
            //    return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            //}

            // FIX THIS SHIT

            //var locations = animal.VisitedLocations.OrderByDescending(al => al.VisitDateTime).ToList();
            //var location = locations.Where(l => l.Id == visitedLocationToUpdate.Id);

            //VisitedLocation? nextVisitedLocation, previousVisitedLocation;
            //nextVisitedLocation = locations
            //    .SkipWhile(x => x != location).Skip(1).DefaultIfEmpty(locations[0]).FirstOrDefault();
            //previousVisitedLocation = locations
            //    .TakeWhile(x => x != location).DefaultIfEmpty(locations[locations.Count - 1] ).LastOrDefault();

            //if (nextVisitedLocation is not null && nextVisitedLocation.LocationId == request.LocationPointId)
            //{
            //    return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            //}
            //if (previousVisitedLocation is not null && previousVisitedLocation.LocationId == request.LocationPointId)
            //{
            //    return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            //}

            var locations = animal.VisitedLocations.OrderBy(al => al.VisitDateTime).ToList();
            //var location = locations.Where(l => l.Id == visitedLocationToUpdate.Id);
            VisitedLocation? prev = null, next = null;

            int index = locations.FindIndex(l => l.Id == visitedLocationToUpdate.Id);
            if (index - 1 > -1)
                prev = locations[index - 1];
            if (index + 1 < locations.Count)
                next = locations[index + 1];

            if (next is not null && next.LocationId == request.LocationPointId)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (prev is not null && prev.LocationId == request.LocationPointId)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            #endregion

            try
            {
                return await UpdateVisitedLocation(visitedLocationToUpdate, existingLocation);
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

        public async Task<HttpStatusCode> RemoveVisitedLocationAsync(
            long? animalId,
            long? visitedLocationId)
        {
            if (!IdValidator.IsValid(animalId, visitedLocationId))
            {
                return HttpStatusCode.BadRequest;
            }

            var animal = await _db.Animals.Include(a => a.VisitedLocations)
                                          .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal is null)
            {
                return HttpStatusCode.NotFound;
            }

            var visitedLocation = await _db.VisitedLocations
                .FirstOrDefaultAsync(vl => vl.Id == visitedLocationId);
            if (visitedLocation is null)
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
                _db.VisitedLocations.Remove(visitedLocationToDelete);
                animal.VisitedLocations.Remove(visitedLocationToDelete);
                
                // TODO: Optimize
                if (animal.VisitedLocations.Count > 0)
                {
                    var newFirstLocation = animal.VisitedLocations
                        .OrderByDescending(vl => vl.VisitDateTime)
                        .First();

                    if (newFirstLocation.LocationId == animal.ChippingLocationId)
                    {
                        _db.VisitedLocations.Remove(newFirstLocation);
                        animal.VisitedLocations.Remove(newFirstLocation);
                    }
                }

                await _db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
