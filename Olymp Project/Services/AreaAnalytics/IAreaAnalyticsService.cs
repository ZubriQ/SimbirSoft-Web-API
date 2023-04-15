using Olymp_Project.Responses;

namespace Olymp_Project.Services.AreaAnalytics
{
    public interface IAreaAnalyticsService
    {
        Task<IServiceResponse<AreaAnalyticsResponseDto>> GetAnalyticsByAreaIdAsync(
            long? areaId, 
            AreaAnalyticsQuery query);
    }
}
