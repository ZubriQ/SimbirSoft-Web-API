using NpgsqlTypes;
using Olymp_Project.Dtos.AreaAnalytics;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.AreaAnalytics
{
    public class AreaAnalyticsService : IAreaAnalyticsService
    {
        private readonly ChipizationDbContext _db;

        public AreaAnalyticsService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<IServiceResponse<AreaAnalyticsResponseDto>> GetAnalyticsByAreaIdAsync(
            long? areaId, AreaAnalyticsQuery query)
        {
            try
            {
                if (!IsRequestValid(areaId, query))
                {
                    return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.BadRequest);
                }

                if (await _db.Areas.FindAsync(areaId) is not Area area)
                {
                    return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.NotFound);
                }

                var visitedLocationsInArea = await GetVisitedLocationsInArea(area, query);
                var distinctAnimalsInArea = GetDistinctAnimalsInArea(visitedLocationsInArea);

                AreaAnalyticsResponseDto analytics = await CalculateAreaAnalytics(
                    distinctAnimalsInArea, query.StartDate!.Value, query.EndDate!.Value, area);

                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.OK, analytics);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.InternalServerError, errorMessage: ex.Message);
            }
        }

        private bool IsRequestValid(long? areaId, AreaAnalyticsQuery query)
        {
            return IdValidator.IsValid(areaId) && AreaAnalyticsValidator.IsValid(query);
        }

        private async Task<List<VisitedLocation>> GetVisitedLocationsInArea(Area area, AreaAnalyticsQuery query)
        {
            var allAnimals = await _db.Animals
                .Include(a => a.ChippingLocation)
                .Include(a => a.VisitedLocations)
                .ToListAsync();

            var visitedLocationsInArea = new List<VisitedLocation>();

            foreach (var animal in allAnimals)
            {
                var visitedLocations = animal.VisitedLocations
                    .Where(v => v.VisitDateTime.Date >= query.StartDate!.Value.Date && v.VisitDateTime.Date <= query.EndDate!.Value.Date)
                    .ToList();

                visitedLocationsInArea.AddRange(visitedLocations);

                if (!visitedLocations.Any())
                {
                    Location location = animal.ChippingLocation;

                    if (location != null && area.Points.Contains(new NpgsqlPoint(location.Longitude, location.Latitude)))
                    {
                        visitedLocationsInArea.Add(new VisitedLocation
                        {
                            AnimalId = animal.Id,
                            Animal = animal,
                            LocationId = location.Id,
                            Location = location,
                            VisitDateTime = query.StartDate.Value
                        });
                    }
                }
            }

            return visitedLocationsInArea;
        }


        private List<long> GetDistinctAnimalsInArea(List<VisitedLocation> visitedLocationsInArea)
        {
            return visitedLocationsInArea
                .Select(v => v.AnimalId)
                .Distinct()
                .ToList();
        }

        private async Task<AreaAnalyticsResponseDto> CalculateAreaAnalytics(
            List<long> distinctAnimalsInArea, DateTime startDate, DateTime endDate, Area area)
        {
            var animals = await GetAnimalsWithDetails(distinctAnimalsInArea);
            var animalGroups = GroupAnimalsByKind(animals, startDate, endDate, area);

            AreaAnalyticsResponseDto analytics = new AreaAnalyticsResponseDto
            {
                TotalQuantityAnimals = animals.Count,
                TotalAnimalsArrived = animalGroups.Sum(a => a.AnimalsArrived),
                TotalAnimalsGone = animalGroups.Sum(a => a.AnimalsGone),
                AnimalsAnalytics = animalGroups
            };

            return analytics;
        }



        private async Task<List<Animal>> GetAnimalsWithDetails(List<long> animalIds)
        {
            return await _db.Animals
                .Where(a => animalIds.Contains(a.Id))
                .Include(a => a.Kinds)
                .Include(a => a.VisitedLocations)
                .Include(a => a.ChippingLocation)
                .ToListAsync();
        }

        private AnimalsAnalyticsDto[] GroupAnimalsByKind(List<Animal> animals, DateTime startDate, DateTime endDate, Area area)
        {
            return animals
                .GroupBy(a => a.Kinds.FirstOrDefault()!.Id)
                .Select(g => new AnimalsAnalyticsDto
                {
                    AnimalTypeId = g.Key,
                    AnimalType = g.First().Kinds.FirstOrDefault()!.Name,
                    QuantityAnimals = g.Count(),
                    AnimalsArrived = CalculateAnimalsArrived(g, startDate, endDate, area),
                    AnimalsGone = g.Count(a => a.DeathDateTime.HasValue && a.DeathDateTime.Value.Date >= startDate.Date && a.DeathDateTime.Value.Date <= endDate.Date)
                })
                .ToArray();
        }

        private int CalculateAnimalsArrived(IEnumerable<Animal> animals, DateTime startDate, DateTime endDate, Area area)
        {
            int animalsArrived = 0;

            foreach (var animal in animals)
            {
                var visitedLocations = animal.VisitedLocations
                    .Where(v => v.VisitDateTime.Date >= startDate.Date && v.VisitDateTime.Date <= endDate.Date)
                    .ToList();

                for (int i = 0; i < visitedLocations.Count; i++)
                {
                    var currentVisitedLocation = visitedLocations[i];

                    if (currentVisitedLocation.Location == null)
                    {
                        continue;
                    }

                    var previousVisitedLocation = i > 0 ? visitedLocations[i - 1] : null;

                    var currentLocation = new NpgsqlPoint(currentVisitedLocation.Location.Longitude, currentVisitedLocation.Location.Latitude);
                    NpgsqlPoint? previousLocation = previousVisitedLocation != null && previousVisitedLocation.Location != null ? new NpgsqlPoint(previousVisitedLocation.Location.Longitude, previousVisitedLocation.Location.Latitude) : (NpgsqlPoint?)null;

                    if (IsEnteringArea(currentLocation, previousLocation, area))
                    {
                        animalsArrived++;
                    }
                }

                if (animal.ChippingDateTime.Date >= startDate.Date && animal.ChippingDateTime.Date <= endDate.Date && area.Points.Contains(new NpgsqlPoint(animal.ChippingLocation.Longitude, animal.ChippingLocation.Latitude)))
                {
                    animalsArrived++;
                }
            }

            return animalsArrived;
        }



        private bool IsEnteringArea(NpgsqlPoint currentLocation, NpgsqlPoint? previousLocation, Area area)
        {
            var currentLocationInArea = area.Points.Contains(currentLocation);

            if (!previousLocation.HasValue)
            {
                return currentLocationInArea;
            }
            else
            {
                var previousLocationInArea = area.Points.Contains(previousLocation.Value);
                return currentLocationInArea && !previousLocationInArea;
            }
        }
    }
}
