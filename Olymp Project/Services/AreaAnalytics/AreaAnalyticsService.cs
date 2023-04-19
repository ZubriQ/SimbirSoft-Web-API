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
                InitializeCompleteVisitedLocations(animals);

                var areaDataAnalysis = AnalyzeAreaData(area, query.StartDate!.Value, query.EndDate!.Value, animals);
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

        private AreaAnalyticsResponseDto AnalyzeAreaData(
            Area area, DateTime startDate, DateTime endDate, List<Animal> animals)
        {
            _analyzer.SetDateRange(startDate, endDate);
            _analyzer.SetInitialData(area, animals);
            return _analyzer.Analyze();
        }
    }
}
