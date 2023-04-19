namespace Olymp_Project.Helpers
{
    public class AreaAnalyzer
    {
        private GeometryChecker? _geometryChecker;

        // Date range & initial data.
        private DateTime _startDate;
        private DateTime _endDate;
        private List<Animal> _animals;

        // AreaAnalyticsResponseDto filling.
        private long totalAnimalsArrived = 0;
        private long totalAnimalsGone = 0;
        private long totalQuantityAnimals = 0;

        // AnimalsAnalyticsDto[] filling.
        private Dictionary<long, string> _uniqueKinds;
        private Dictionary<long, long> _kindCount;
        private Dictionary<long, (long arrived, long gone)> _kindArrivedGone;

        public AreaAnalyzer()
        {
            _animals = new();
            _uniqueKinds = new();
            _kindCount = new();
            _kindArrivedGone = new();
        }

        public void SetDateRange(DateTime startDate, DateTime endDate) 
        {
            _startDate = startDate.Date;
            _endDate = endDate.Date;
        }

        public void SetInitialData(Area area, List<Animal> animals)
        {
            var closedPolygon = area.Points.ToList();
            closedPolygon.Add(closedPolygon[0]);
            _geometryChecker = new GeometryChecker(closedPolygon);
            _animals = animals;
        }

        public AreaAnalyticsResponseDto Analyze()
        {
            List<Animal> involvedAnimals = new();
            foreach (var animal in _animals)
            {
                TryExtractAnimalData(involvedAnimals, animal);
            }

            return CreateAreaAnalyticsResponseDto();
        }

        private void TryExtractAnimalData(List<Animal> involvedAnimals, Animal animal)
        {
            var visitedLocations = GetVisitedLocations(animal);
            var status = GetAnimalStatus(visitedLocations);

            if (status == AnimalStatus.Arrived || status == AnimalStatus.Inside)
            {
                involvedAnimals.Add(animal);
                totalQuantityAnimals++;

                if (status == AnimalStatus.Arrived)
                {
                    totalAnimalsArrived++;
                }
            }

            if (status == AnimalStatus.Gone)
            {
                totalAnimalsGone++;
            }

            if (status != AnimalStatus.None)
            {
                ExtractAnimalKindsInfo(animal, status);
            }
        }

        private List<VisitedLocation> GetVisitedLocations(Animal animal)
        {
            var visitedLocations = new List<VisitedLocation> { new VisitedLocation
                {
                    VisitDateTime = animal.ChippingDateTime,
                    Location = animal.ChippingLocation
                }};
            visitedLocations
                .AddRange(animal.VisitedLocations
                .Where(vl => vl.VisitDateTime >= _startDate && vl.VisitDateTime <= _endDate)
                .OrderBy(vl => vl.VisitDateTime));

            return visitedLocations;
        }

        private AnimalStatus GetAnimalStatus(List<VisitedLocation> visitedLocations)
        {
            var isEarliestLocationInsideArea = _geometryChecker!
                .IsLocationInsidePolygon(visitedLocations.First().Location);
            var isLastLocationInsideArea = _geometryChecker
                .IsLocationInsidePolygon(visitedLocations.Last().Location);

            if (isEarliestLocationInsideArea && isLastLocationInsideArea)
            {
                return AnimalStatus.Inside;
            }    
            if (!isEarliestLocationInsideArea && isLastLocationInsideArea)
            {
                return AnimalStatus.Arrived;
            }
            if (isEarliestLocationInsideArea && !isLastLocationInsideArea)
            {
                return AnimalStatus.Gone;
            }

            return AnimalStatus.None;
        }

        private void ExtractAnimalKindsInfo(Animal animal, AnimalStatus status)
        {
            foreach (var kind in animal.Kinds)
            {
                TryInitializeUniqueKind(kind);
                UpdateUniqueKindFields(kind, status);
            }
        }

        private void TryInitializeUniqueKind(Kind kind)
        {
            if (!_uniqueKinds.ContainsKey(kind.Id))
            {
                _uniqueKinds.Add(kind.Id, kind.Name);
                _kindCount[kind.Id] = 0;
                _kindArrivedGone[kind.Id] = (0, 0);
            }
        }

        private void UpdateUniqueKindFields(Kind kind, AnimalStatus status)
        {
            if (status == AnimalStatus.Arrived)
            {
                _kindArrivedGone[kind.Id] =
                    (_kindArrivedGone[kind.Id].arrived + 1, _kindArrivedGone[kind.Id].gone);
            }

            if (status == AnimalStatus.Gone)
            {
                _kindArrivedGone[kind.Id] =
                   (_kindArrivedGone[kind.Id].arrived, _kindArrivedGone[kind.Id].gone + 1);
            }
            else
            {
                _kindCount[kind.Id]++;
            }
        }

        private AreaAnalyticsResponseDto CreateAreaAnalyticsResponseDto()
        {
            var responseDto = new AreaAnalyticsResponseDto
            {
                TotalQuantityAnimals = totalQuantityAnimals,
                TotalAnimalsArrived = totalAnimalsArrived,
                TotalAnimalsGone = totalAnimalsGone,
                AnimalsAnalytics = new()
            };

            foreach (var kind in _uniqueKinds)
            {
                responseDto.AnimalsAnalytics.Add(new AnimalsAnalyticsDto
                {
                    AnimalKind = kind.Value,
                    AnimalKindId = kind.Key,
                    QuantityAnimals = _kindCount[kind.Key],
                    AnimalsArrived = _kindArrivedGone[kind.Key].arrived,
                    AnimalsGone = _kindArrivedGone[kind.Key].gone
                });
            }

            return responseDto;
        }

        //#region Geometry methods

        //private bool IsLocationInsidePolygon(Location location)
        //{
        //    return IsPointInsidePolygon(location.Longitude, location.Latitude);
        //}

        //private bool IsPointInsidePolygon(double x, double y)
        //{
        //    var coordinates = _polygon!.Select(p => new Coordinate(p.X, p.Y)).ToArray();
        //    var linearRing = new LinearRing(coordinates);
        //    var polygonShell = new Polygon(linearRing);
        //    var polygonBoundary = (LineString)polygonShell.Boundary;
            
        //    var point = _geometryFactory.CreatePoint(new Coordinate(x, y));
        //    var distance = point.Distance(polygonBoundary);

        //    var isInside = polygonShell.Contains(point) || Math.Abs(distance) < 1e-6;
        //    return isInside;
        //}

        //#endregion
    }
}
