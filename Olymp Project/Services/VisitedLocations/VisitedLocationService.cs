using Olymp_Project.Dtos.VisitedLocation;
using Olymp_Project.Models;
using System.Net;

namespace Olymp_Project.Services.VisitedLocations
{
    public class VisitedLocationService : IVisitedLocationService
    {
        private readonly ChipizationDbContext _db;

        public VisitedLocationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<(HttpStatusCode, IEnumerable<VisitedLocation>)> GetVisitedLocationsAsync(
            long animalId,
            DateTimeRangeQuery query,
            Paging paging)
        {
            var animal = await _db.Animals.Include(a => a.VisitedLocations)
                                          .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal is null)
            {
                return (HttpStatusCode.NotFound, Enumerable.Empty<VisitedLocation>());
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
                                 .Skip(paging.Skip!.Value)
                                 .Take(paging.Take!.Value)
                                 .ToList();
            return (HttpStatusCode.OK, locations);
        }

        public async Task<(HttpStatusCode, VisitedLocation?)> InsertVisitedLocationAsync(
            long animalId, 
            long locationId)
        {
            var animal = await _db.Animals.Include(a => a.VisitedLocations)
                                          .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (animal.LifeStatus == "DEAD")
            {
                return (HttpStatusCode.BadRequest, null);
            }

            // Животное находится в точке чипирования и никуда не перемещалось,
            // попытка добавить точку локации, равную точке чипирования.
            if (animal.ChippingLocationId == locationId && animal.VisitedLocations.Count == 0)
            {
                return (HttpStatusCode.BadRequest, null);
            }

            // Попытка добавить точку локации, в которой уже находится животное.
            var lastVisitedLocation = animal.VisitedLocations
                .OrderByDescending(l => l.VisitDateTime)
                .FirstOrDefault();
            if (lastVisitedLocation != null && lastVisitedLocation.Id == locationId)
            {
                return (HttpStatusCode.BadRequest, null);
            }

            var location = await _db.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
            if (location is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            try
            {
                // TODO: Optimize
                VisitedLocation newVisitedLocation = new()
                {
                    Animal = animal,
                    Location = location,
                    VisitDateTime = DateTime.Now
                };
                _db.VisitedLocations.Add(newVisitedLocation);
                await _db.SaveChangesAsync();

                return (HttpStatusCode.OK, newVisitedLocation);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<(HttpStatusCode, VisitedLocation?)> UpdateVisitedLocationAsync(
            long animalId, 
            VisitedLocationRequestDto request)
        {
            var animal = await _db.Animals.Include(a => a.VisitedLocations)
                                          .FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            // Объект с информацией о посещенной точке локации
            var visitedLocationToUpdate = await _db.VisitedLocations
                .FirstOrDefaultAsync(l => l.Id == request.VisitedLocationId);
            if (visitedLocationToUpdate is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            if (visitedLocationToUpdate.LocationId == request.LocationId)
            {
                return (HttpStatusCode.BadRequest, null);
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
                return (HttpStatusCode.BadRequest, null);
            }

            // У животного нет объекта с информацией о посещенной точке локации
            // с visitedLocationPointId.
            var isVisited = animal.VisitedLocations
                .FirstOrDefault(v => v.LocationId == visitedLocationToUpdate.LocationId);
            if (isVisited is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            var existingLocation = await _db.Locations
                .FirstOrDefaultAsync(l => l.Id == request.LocationId);
            if (existingLocation is null)
            {
                return (HttpStatusCode.NotFound, null);
            }

            try
            {
                // TODO: Optimize
                visitedLocationToUpdate.Location = existingLocation;
                _db.VisitedLocations.Attach(visitedLocationToUpdate);
                _db.Entry(visitedLocationToUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return (HttpStatusCode.OK, visitedLocationToUpdate);
            }
            catch
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<HttpStatusCode> RemoveVisitedLocationAsync(
            long animalId,
            long visitedLocationId)
        {
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
