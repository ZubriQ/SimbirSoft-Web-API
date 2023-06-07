using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.AreaAnalytics
{
    public interface IAreaAnalyticsService
    {
        Task<IResponse<AreaAnalyticsResponseDto>> GetAnalyticsByAreaIdAsync(
            long? areaId, 
            AreaAnalyticsQuery query);
    }
}
