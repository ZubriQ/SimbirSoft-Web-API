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
            #region Validation

            if (!IsRequestValid(areaId, query))
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.BadRequest);
            }
            if (await _db.Areas.FindAsync(areaId) is not Area area)
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.NotFound);
            }

            #endregion

            try
            {
                // TODO: Optimize
                // Find all the animal movements within the given area and the specified date range.
                var startDate = query.StartDate!.Value.Date;
                var endDate = query.EndDate!.Value.Date.AddDays(1); // Adding one day to make the endDate inclusive.

                var polygon = area.Points.ToList();

                // Count Total quantity of animals.
                var animals = await _db.Animals
                    .Include(animal => animal.Kinds)
                    .Include(animal => animal.ChippingLocation)
                    .Include(animal => animal.VisitedLocations)
                        .ThenInclude(vl => vl.Location)
                    .ToListAsync();

                int totalQuantityAnimals = animals
                    .Where(animal =>
                        (animal.VisitedLocations.Count == 0 &&
                         IsPointInPolygon(polygon, animal.ChippingLocation.Longitude, animal.ChippingLocation.Latitude) &&
                         animal.ChippingDateTime >= startDate &&
                         animal.ChippingDateTime < endDate) ||
                        (animal.VisitedLocations.Any(vl =>
                            IsPointInPolygon(polygon, vl.Location.Longitude, vl.Location.Latitude) &&
                            vl.VisitDateTime >= startDate &&
                            vl.VisitDateTime < endDate)))
                    .Count();

                // Count Total animals arrived.
                var visitedLocations = await _db.VisitedLocations
                    .Include(vl => vl.Animal)
                    .Include(vl => vl.Location)
                    .Where(vl =>
                        vl.VisitDateTime >= startDate &&
                        vl.VisitDateTime < endDate)
                    .ToListAsync();

                int totalAnimalsArrived = visitedLocations
                    .Where(vl =>
                        IsPointInPolygon(polygon, vl.Location.Longitude, vl.Location.Latitude) &&
                        !IsPointInPolygon(polygon, vl.Animal.ChippingLocation.Longitude, vl.Animal.ChippingLocation.Latitude) &&
                        (
                            !vl.Animal.VisitedLocations.Any(pvl => pvl.VisitDateTime < startDate) ||
                            !IsPointInPolygon(polygon,
                                vl.Animal.VisitedLocations
                                    .Where(pvl => pvl.VisitDateTime < startDate)
                                    .OrderByDescending(pvl => pvl.VisitDateTime)
                                    .First().Location.Longitude,
                                vl.Animal.VisitedLocations
                                    .Where(pvl => pvl.VisitDateTime < startDate)
                                    .OrderByDescending(pvl => pvl.VisitDateTime)
                                    .First().Location.Latitude
                            )
                        )
                    )
                    .Select(vl => vl.Animal)
                    .Distinct()
                    .Count();

                // Count Total animals gone.
                int totalAnimalsGone = visitedLocations
                    .Where(vl =>
                        !IsPointInPolygon(polygon, vl.Location.Longitude, vl.Location.Latitude) &&
                        IsPointInPolygon(polygon, vl.Animal.ChippingLocation.Longitude, vl.Animal.ChippingLocation.Latitude) &&
                        (
                            !vl.Animal.VisitedLocations.Any(pvl => pvl.VisitDateTime < startDate) ||
                            IsPointInPolygon(polygon,
                                vl.Animal.VisitedLocations
                                    .Where(pvl => pvl.VisitDateTime < startDate)
                                    .OrderByDescending(pvl => pvl.VisitDateTime)
                                    .First().Location.Longitude,
                                vl.Animal.VisitedLocations
                                    .Where(pvl => pvl.VisitDateTime < startDate)
                                    .OrderByDescending(pvl => pvl.VisitDateTime)
                                    .First().Location.Latitude
                            )
                        )
                    )
                    .Select(vl => vl.Animal)
                    .Distinct()
                    .Count();

                // AnimalsAnalytics.
                var animalKinds = animals.SelectMany(a => a.Kinds).Distinct();

                var animalsAnalytics = animalKinds.Select(kind =>
                {
                    // Quantity animals.
                    int quantityAnimals = animals
                        .Where(a => a.Kinds.Contains(kind) &&
                            (
                                (a.VisitedLocations.Count == 0 &&
                                IsPointInPolygon(polygon, a.ChippingLocation.Longitude, a.ChippingLocation.Latitude) &&
                                a.ChippingDateTime >= startDate &&
                                a.ChippingDateTime < endDate) ||
                                (a.VisitedLocations.Any(vl =>
                                    IsPointInPolygon(polygon, vl.Location.Longitude, vl.Location.Latitude) &&
                                    vl.VisitDateTime >= startDate &&
                                    vl.VisitDateTime < endDate))
                            ))
                        .Count();

                    // Arrived animals.
                    int animalsArrived = visitedLocations
                        .Where(vl => vl.Animal.Kinds.Contains(kind) && AnimalArrivedInPolygon(polygon, vl, startDate))
                        .Select(vl => vl.Animal)
                        .Distinct()
                        .Count();

                    // Gone animals.
                    int animalsGone = visitedLocations
                        .Where(vl => vl.Animal.Kinds.Contains(kind) && AnimalLeftPolygon(polygon, vl, startDate))
                        .Select(vl => vl.Animal)
                        .Distinct()
                        .Count();

                    return new AnimalsAnalyticsDto
                    {
                        AnimalType = kind.Name,
                        AnimalTypeId = kind.Id,
                        QuantityAnimals = quantityAnimals,
                        AnimalsArrived = animalsArrived,
                        AnimalsGone = animalsGone
                    };
                }).Where(dto => dto.QuantityAnimals > 0)
                    .ToArray();

                var responseDto = new AreaAnalyticsResponseDto
                {
                    TotalQuantityAnimals = totalQuantityAnimals,
                    TotalAnimalsArrived = totalAnimalsArrived,
                    TotalAnimalsGone = totalAnimalsGone,
                    AnimalsAnalytics = animalsAnalytics
                };
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.OK, responseDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.InternalServerError, errorMessage: ex.Message);
            }
        }

        private bool AnimalArrivedInPolygon(List<NpgsqlPoint> polygon, VisitedLocation vl, DateTime startDate)
        {
            bool isCurrentlyInPolygon = IsPointInPolygon(polygon, vl.Location.Longitude, vl.Location.Latitude);

            var previousVisitedLocation = vl.Animal.VisitedLocations
                .Where(pvl => pvl.VisitDateTime < startDate)
                .OrderByDescending(pvl => pvl.VisitDateTime)
                .FirstOrDefault();

            bool wasOutsidePolygonBeforeStartDate = previousVisitedLocation == null || !IsPointInPolygon(
                polygon,
                previousVisitedLocation.Location.Longitude,
                previousVisitedLocation.Location.Latitude
            );

            return isCurrentlyInPolygon && wasOutsidePolygonBeforeStartDate;
        }

        private bool AnimalLeftPolygon(List<NpgsqlPoint> polygon, VisitedLocation vl, DateTime startDate)
        {
            bool isCurrentlyOutsidePolygon = !IsPointInPolygon(polygon, vl.Location.Longitude, vl.Location.Latitude);

            var previousVisitedLocation = vl.Animal.VisitedLocations
                .Where(pvl => pvl.VisitDateTime < startDate)
                .OrderByDescending(pvl => pvl.VisitDateTime)
                .FirstOrDefault();

            bool wasInsidePolygonBeforeStartDate = previousVisitedLocation == null || IsPointInPolygon(
                polygon,
                previousVisitedLocation.Location.Longitude,
                previousVisitedLocation.Location.Latitude
            );

            return isCurrentlyOutsidePolygon && wasInsidePolygonBeforeStartDate;
        }

        private bool IsRequestValid(long? areaId, AreaAnalyticsQuery query)
        {
            return IdValidator.IsValid(areaId) && AreaAnalyticsValidator.IsValid(query);
        }

        private bool IsPointInPolygon(List<NpgsqlPoint> polygon, double x, double y)
        {
            bool isInside = false;
            int j = polygon.Count - 1;

            // Check if the point is on one of the vertices.
            if (polygon.Any(vertex => vertex.X == x && vertex.Y == y))
            {
                return true;
            }

            for (int i = 0; i < polygon.Count; i++)
            {
                if ((polygon[i].Y < y && polygon[j].Y >= y || polygon[j].Y < y && polygon[i].Y >= y) &&
                    (polygon[i].X + (y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < x))
                {
                    isInside = !isInside;
                }
                j = i;
            }
            return isInside;
        }
    }
}
