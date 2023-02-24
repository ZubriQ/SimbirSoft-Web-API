using System.Net;

namespace Olymp_Project.Services.Locations
{
    public interface ILocationService
    {
        Task<Location?> GetLocationAsync(long id);
        Task<(HttpStatusCode, Location?)> AddLocationAsync(Location location);
        Task<(HttpStatusCode, Location?)> UpdateLocationAsync(long id, Location location);
        Task<HttpStatusCode> RemoveLocationAsync(long id);
    }
}
