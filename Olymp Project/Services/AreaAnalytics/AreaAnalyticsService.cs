using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;
using Location = Olymp_Project.Models.Location;

namespace Olymp_Project.Services.AreaAnalytics
{
    public class AreaAnalyticsService : IAreaAnalyticsService
    {
        private readonly ChipizationDbContext _db;
        private GeometryFactory _geometryFactory = new();
        private DateTime _startDate;
        private DateTime _endDate;
        private List<NpgsqlPoint> _polygon;

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
                _startDate = query.StartDate!.Value.Date;
                _endDate = query.EndDate!.Value.Date; // .AddDays(1); ?
                _polygon = area.Points.ToList();
                _polygon.Add(_polygon[0]);

                var animals = await GetAllAnimalsAsync();
                InitializeCompleteVisitedLocations(animals);
                List<Animal> involvedAnimals = new List<Animal>();
                long totalAnimalsArrived = 0;
                long totalAnimalsGone = 0;
                long totalQuantityAnimals = 0;

                Dictionary<long, string> uniqueKinds = new Dictionary<long, string>();

                // Kinds
                Dictionary<long, int> kindCount = new Dictionary<long, int>();
                Dictionary<long, (int arrived, int gone)> kindArrivedGone = new Dictionary<long, (int, int)>();

                foreach (var animal in animals)
                {
                    var visitedLocations = new List<VisitedLocation> { new VisitedLocation { VisitDateTime = animal.ChippingDateTime, Location = animal.ChippingLocation } };
                    visitedLocations.AddRange(animal.VisitedLocations
                        .Where(vl => vl.VisitDateTime >= _startDate && vl.VisitDateTime <= _endDate)
                        .OrderBy(vl => vl.VisitDateTime));

                    AnimalStatus? status = null;

                    bool isEarliestLocationInsideArea = IsLocationInsidePolygon(visitedLocations.First().Location);
                    bool isLastLocationInsideArea = IsLocationInsidePolygon(visitedLocations.Last().Location);

                    if (isEarliestLocationInsideArea && isLastLocationInsideArea)
                    {
                        status = AnimalStatus.Inside;
                    }
                    else if (!isEarliestLocationInsideArea && isLastLocationInsideArea)
                    {
                        status = AnimalStatus.Entered;
                    }
                    else if (isEarliestLocationInsideArea && !isLastLocationInsideArea)
                    {
                        status = AnimalStatus.Gone;
                        totalAnimalsGone++;
                    }

                    if (status.HasValue && (status.Value == AnimalStatus.Entered || status.Value == AnimalStatus.Inside))
                    {
                        involvedAnimals.Add(animal);
                        totalQuantityAnimals++; // Total quantity

                        if (status.Value == AnimalStatus.Entered)
                        {
                            totalAnimalsArrived++;
                        }

                        // Also extract kinds
                        foreach (var kind in animal.Kinds)
                        {
                            if (!uniqueKinds.ContainsKey(kind.Id))
                            {
                                uniqueKinds.Add(kind.Id, kind.Name);
                                kindCount[kind.Id] = 0;
                                kindArrivedGone[kind.Id] = (0, 0);
                            }

                            kindCount[kind.Id]++; // +

                            if (status.HasValue && (status.Value == AnimalStatus.Entered || status.Value == AnimalStatus.Inside))
                            {
                                if (status.Value == AnimalStatus.Entered)
                                {
                                    kindArrivedGone[kind.Id] = (kindArrivedGone[kind.Id].arrived + 1, kindArrivedGone[kind.Id].gone);
                                }
                            }
                            else if (isEarliestLocationInsideArea && !isLastLocationInsideArea)
                            {
                                kindArrivedGone[kind.Id] = (kindArrivedGone[kind.Id].arrived, kindArrivedGone[kind.Id].gone + 1);
                            }
                        }
                    }
                    else if (status.HasValue && status.Value == AnimalStatus.Gone)
                    {
                        // Also extract kinds
                        foreach (var kind in animal.Kinds)
                        {
                            if (!uniqueKinds.ContainsKey(kind.Id))
                            {
                                uniqueKinds.Add(kind.Id, kind.Name);
                                kindCount[kind.Id] = 0;
                                kindArrivedGone[kind.Id] = (0, 0);
                            }

                            if (status.HasValue && (status.Value == AnimalStatus.Entered || status.Value == AnimalStatus.Inside))
                            {
                                if (status.Value == AnimalStatus.Entered)
                                {
                                    kindArrivedGone[kind.Id] = (kindArrivedGone[kind.Id].arrived + 1, kindArrivedGone[kind.Id].gone);
                                }
                            }
                            else if (isEarliestLocationInsideArea && !isLastLocationInsideArea)
                            {
                                kindArrivedGone[kind.Id] = (kindArrivedGone[kind.Id].arrived, kindArrivedGone[kind.Id].gone + 1);
                            }
                        }
                    }
                }

                var responseDto = new AreaAnalyticsResponseDto
                {
                    TotalQuantityAnimals = 0,
                    TotalAnimalsArrived = 0,
                    TotalAnimalsGone = 0,
                    AnimalsAnalytics = new()
                };
                responseDto.TotalQuantityAnimals = totalQuantityAnimals;
                responseDto.TotalAnimalsArrived = totalAnimalsArrived;
                responseDto.TotalAnimalsGone = totalAnimalsGone;

                // Code exracted kinds
                foreach (var kind in uniqueKinds)
                {
                    responseDto.AnimalsAnalytics.Add(new AnimalsAnalyticsDto
                    {
                        AnimalKind = kind.Value,
                        AnimalKindId = kind.Key,
                        QuantityAnimals = kindCount[kind.Key],
                        AnimalsArrived = kindArrivedGone[kind.Key].arrived,
                        AnimalsGone = kindArrivedGone[kind.Key].gone
                    });
                }

                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.OK, responseDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.InternalServerError, errorMessage: ex.Message);
            }
        }

        #region Methods

        private bool IsLocationInsidePolygon(Location location)
        {
            return IsPointInPolygon(location.Longitude, location.Latitude);
        }

        #endregion

        #region My Default Methods
        private bool IsRequestValid(long? areaId, AreaAnalyticsQuery query)
        {
            return IdValidator.IsValid(areaId) && AreaAnalyticsValidator.IsValid(query);
        }

        private async Task<List<Animal>> GetAllAnimalsAsync()
        {
            return await _db.Animals
                .Include(animal => animal.Kinds)
                .Include(animal => animal.ChippingLocation)
                .Include(animal => animal.VisitedLocations)
                    .ThenInclude(vl => vl.Location)
                .OrderBy(animal => animal.ChippingDateTime)
                .ToListAsync();
        }

        private void InitializeCompleteVisitedLocations(List<Animal> animals)
        {
            foreach (var animal in animals)
            {
                if (animal.VisitedLocations.Count == 0)
                {
                    animal.VisitedLocations.Add(new VisitedLocation()
                    {
                        VisitDateTime = animal.ChippingDateTime,
                        Location = animal.ChippingLocation
                    });
                }
            }
        }
        #endregion

        #region Is Point In Polygon
        private bool IsPointInPolygon(double x, double y)
        {
            var point = _geometryFactory.CreatePoint(new Coordinate(x, y));
            var coordinates = _polygon.Select(p => new Coordinate(p.X, p.Y)).ToArray();
            var linearRing = new LinearRing(coordinates);
            var polygonShell = new Polygon(linearRing);
            var polygonBoundary = (LineString)polygonShell.Boundary;

            var distance = point.Distance(polygonBoundary);
            var isInside = polygonShell.Contains(point) || Math.Abs(distance) < 1e-6;
            return isInside;
        }
        #endregion
    }

    public enum AnimalStatus
    {
        Gone,
        Entered,
        Inside
    }
}
