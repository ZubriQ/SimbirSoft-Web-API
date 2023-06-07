using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Areas
{
    public interface IAreaService
    {
        Task<IResponse<Area>> GetAreaByIdAsync(long? areaId);
        Task<IResponse<Area>> InsertAreaAsync(AreaRequestDto request);
        Task<IResponse<Area>> UpdateAreaByIdAsync(long? areaId, AreaRequestDto request);
        Task<HttpStatusCode> RemoveAreaByIdAsync(long? areaId);
    }
}
