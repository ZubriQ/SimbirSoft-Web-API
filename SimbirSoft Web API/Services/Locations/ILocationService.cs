using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Locations
{
    public interface ILocationService
    {
        Task<IResponse<Location>> GetLocationByIdAsync(long? id);
        Task<IResponse<Location>> GetLocationByCoordinatesAsync(LocationRequestDto request);
        Task<IResponse<Location>> InsertLocationAsync(LocationRequestDto request);
        Task<IResponse<Location>> UpdateLocationAsync(long? locationId, LocationRequestDto request);
        Task<HttpStatusCode> RemoveLocationAsync(long? locationId);
    }
}
