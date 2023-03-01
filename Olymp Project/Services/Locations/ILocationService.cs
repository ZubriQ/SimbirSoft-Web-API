using Olymp_Project.Responses;
using System.Net;

namespace Olymp_Project.Services.Locations
{
    public interface ILocationService
    {
        Task<IServiceResponse<Location>> GetLocationAsync(long? id);
        Task<IServiceResponse<Location>> InsertLocationAsync(LocationRequestDto location);
        Task<IServiceResponse<Location>> UpdateLocationAsync(long? id, LocationRequestDto location);
        Task<HttpStatusCode> RemoveLocationAsync(long? id);
    }
}
