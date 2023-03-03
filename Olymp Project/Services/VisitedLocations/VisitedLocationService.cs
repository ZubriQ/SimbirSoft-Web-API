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
            if (lastVisitedLocation != null && lastVisitedLocation.Id == locationId)
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
            return new ServiceResponse<VisitedLocation>(HttpStatusCode.OK, visitedLocation);
        }

        public async Task<IServiceResponse<VisitedLocation>> UpdateVisitedLocationAsync(
            long? animalId, 
            VisitedLocationRequestDto request)
        {
            if (!IdValidator.IsValid(animalId, request.VisitedLocationId, request.LocationId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            if (await _db.Animals.Include(a => a.VisitedLocations).FirstOrDefaultAsync(a => a.Id == animalId)
                is not Animal animal)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            // Объект с информацией о посещенной точке локации
            var visitedLocationToUpdate = await _db.VisitedLocations
                .FirstOrDefaultAsync(l => l.Id == request.VisitedLocationId);
            if (visitedLocationToUpdate is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }
            if (visitedLocationToUpdate.LocationId == request.LocationId)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            var animalLocations = animal.VisitedLocations.OrderBy(al => al.VisitDateTime);

            // Проверка на совпадение следующей/предыдущей точки.
            var previousLocation = animalLocations
                .LastOrDefault(al => al.VisitDateTime < visitedLocationToUpdate.VisitDateTime);
            var nextLocation = animalLocations
                .FirstOrDefault(al => al.VisitDateTime > visitedLocationToUpdate.VisitDateTime);

            if ((previousLocation is not null
                 && previousLocation.LocationId == visitedLocationToUpdate.LocationId) ||
                (nextLocation is not null
                 && nextLocation.LocationId == visitedLocationToUpdate.LocationId))
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.BadRequest);
            }

            // У животного нет объекта с информацией о посещенной точке локации
            // с visitedLocationPointId.
            var isVisited = animal.VisitedLocations
                .FirstOrDefault(v => v.LocationId == visitedLocationToUpdate.LocationId);
            if (isVisited is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            var existingLocation = await _db.Locations
                .FirstOrDefaultAsync(l => l.Id == request.LocationId);
            if (existingLocation is null)
            {
                return new ServiceResponse<VisitedLocation>(HttpStatusCode.NotFound);
            }

            try
            {
                return await UpdateVisitedLocation(visitedLocationToUpdate, existingLocation);
            }
            catch
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
                // TODO: Optimize
                _db.VisitedLocations.Remove(visitedLocationToDelete);
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
