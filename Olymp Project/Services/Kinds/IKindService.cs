using System.Net;

namespace Olymp_Project.Services.Kinds
{
    public interface IKindService
    {
        Task<Kind?> GetAnimalKindAsync(long id);
        Task<(HttpStatusCode, Kind?)> AddAnimalKindAsync(Kind location);
        Task<(HttpStatusCode, Kind?)> UpdateAnimalKindAsync(long id, Kind location);
        Task<HttpStatusCode> DeleteAnimalKindAsync(long id);
    }
}
