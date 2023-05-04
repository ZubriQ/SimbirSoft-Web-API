namespace Olymp_Project.Helpers.Geospatial
{
    public class AreaAnalyzer
    {
        private GeometryChecker? _geometryChecker;

        // Initial data.
        private List<Animal> _animals;

        // AreaAnalyticsResponseDto filling.
        private long _totalAnimalsArrived = 0;
        private long _totalAnimalsGone = 0;
        private long _totalQuantityAnimals = 0;

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
            var status = GetAnimalStatusByVisitedLocations(animal.VisitedLocations.ToList());

            if (status == AnimalStatus.Arrived || status == AnimalStatus.Inside)
            {
                involvedAnimals.Add(animal);
                _totalQuantityAnimals++;

                if (status == AnimalStatus.Arrived)
                {
                    _totalAnimalsArrived++;
                }
            }

            if (status == AnimalStatus.Gone)
            {
                _totalAnimalsGone++;
            }

            if (status != AnimalStatus.None)
            {
                ExtractAnimalKindsInfo(animal, status);
            }
        }

        private AnimalStatus GetAnimalStatusByVisitedLocations(List<VisitedLocation> visitedLocations)
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
                TotalQuantityAnimals = _totalQuantityAnimals,
                TotalAnimalsArrived = _totalAnimalsArrived,
                TotalAnimalsGone = _totalAnimalsGone,
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
    }
}
