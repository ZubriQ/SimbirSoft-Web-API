using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Location = Olymp_Project.Models.Location;

namespace Olymp_Project.Helpers
{
    public class AreaAnalyzer
    {
        private GeometryFactory _geometryFactory;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<NpgsqlPoint>? _polygon;
        private List<Animal> _animals;

        public AreaAnalyzer()
        {
            _geometryFactory = new();
            _animals = new();
        }

        public void SetInitialDates(DateTime startDate, DateTime endDate) 
        {
            _startDate = startDate.Date;
            _endDate = endDate.Date;
        }

        public void SetInitialData(Area area, List<Animal> animals)
        {
            _polygon = area.Points.ToList();
            _polygon.Add(_polygon[0]);
            _animals = animals;
        }

        public AreaAnalyticsResponseDto Analyze()
        {
            List<Animal> involvedAnimals = new();
            long totalAnimalsArrived = 0;
            long totalAnimalsGone = 0;
            long totalQuantityAnimals = 0;

            Dictionary<long, string> uniqueKinds = new Dictionary<long, string>();

            // Kinds
            Dictionary<long, long> kindCount = new Dictionary<long, long>();
            Dictionary<long, (long arrived, long gone)> kindArrivedGone = new Dictionary<long, (long, long)>();

            foreach (var animal in _animals)
            {
                var visitedLocations = new List<VisitedLocation> { new VisitedLocation
                {
                    VisitDateTime = animal.ChippingDateTime,
                    Location = animal.ChippingLocation
                } };
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

            return responseDto;
        }

        private bool IsLocationInsidePolygon(Location location)
        {
            return IsPointInsidePolygon(location.Longitude, location.Latitude);
        }

        private bool IsPointInsidePolygon(double x, double y)
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
    }
}
