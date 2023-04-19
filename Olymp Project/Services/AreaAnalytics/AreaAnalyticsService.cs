using Olymp_Project.Helpers;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.AreaAnalytics
{
    public class AreaAnalyticsService : IAreaAnalyticsService
    {
        private readonly ChipizationDbContext _db;
        private AreaAnalyzer _analyzer;

        public AreaAnalyticsService(ChipizationDbContext db)
        {
            _db = db;
            _analyzer = new();
        }

        public async Task<IServiceResponse<AreaAnalyticsResponseDto>> GetAnalyticsByAreaIdAsync(
            long? areaId, AreaAnalyticsQuery query)
        {
            if (!IsRequestValid(areaId, query))
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.BadRequest);
            }

            if (await _db.Areas.FindAsync(areaId) is not Area area)
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.NotFound);
            }

            return await GetAreaAnalyticsAsync(area, query);
        }

        private bool IsRequestValid(long? areaId, AreaAnalyticsQuery query)
        {
            return IdValidator.IsValid(areaId) && AreaAnalyticsValidator.IsValid(query);
        }

        private async Task<IServiceResponse<AreaAnalyticsResponseDto>> GetAreaAnalyticsAsync(
            Area area, AreaAnalyticsQuery query)
        {
            try
            {
                var animals = await GetAllAnimalsAsync();
                InitializeCompleteAndFilteredVisitedLocations(animals, query.StartDate!.Value, query.EndDate!.Value);

                var areaDataAnalysis = AnalyzeAreaData(area, animals);
                return new ServiceResponse<AreaAnalyticsResponseDto>(HttpStatusCode.OK, areaDataAnalysis);
            }
            catch (Exception)
            {
                return new ServiceResponse<AreaAnalyticsResponseDto>();
            }
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

        private void InitializeCompleteAndFilteredVisitedLocations(
            List<Animal> animals, DateTime startDate, DateTime endDate)
        {
            foreach (var animal in animals)
            {
                animal.VisitedLocations.Add(new VisitedLocation()
                {
                    VisitDateTime = animal.ChippingDateTime,
                    Location = animal.ChippingLocation
                });

                var filteredVisitedLocations = animal.VisitedLocations
                    .Where(vl => vl.VisitDateTime >= startDate && vl.VisitDateTime <= endDate)
                    .OrderBy(vl => vl.VisitDateTime)
                    .ToArray();

                animal.VisitedLocations = filteredVisitedLocations;
            }
        }

        private AreaAnalyticsResponseDto AnalyzeAreaData(Area area, List<Animal> animals)
        {
            _analyzer.SetInitialData(area, animals);
            return _analyzer.Analyze();
        }
    }
}
