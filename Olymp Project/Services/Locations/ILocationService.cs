using Olymp_Project.Responses;

namespace Olymp_Project.Services.Locations
{
    public interface ILocationService
    {
        Task<IServiceResponse<Location>> GetLocationByIdAsync(long? id);
        Task<IServiceResponse<Location>> InsertLocationAsync(LocationRequestDto request);
        Task<IServiceResponse<Location>> UpdateLocationAsync(long? locationId, LocationRequestDto request);
        Task<HttpStatusCode> RemoveLocationAsync(long? locationId);
    }
}
