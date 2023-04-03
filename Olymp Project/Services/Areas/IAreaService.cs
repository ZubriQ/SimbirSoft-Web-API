using Olymp_Project.Responses;

namespace Olymp_Project.Services.Areas
{
    public interface IAreaService
    {
        Task<IServiceResponse<Area>> GetAreaByIdAsync(long? areaId);
        Task<IServiceResponse<Area>> InsertAreaAsync(AreaRequestDto request);
        Task<IServiceResponse<Area>> UpdateAreaByIdAsync(long? areaId, AreaRequestDto request);
        Task<HttpStatusCode> RemoveAreaByIdAsync(long? areaId);
    }
}
