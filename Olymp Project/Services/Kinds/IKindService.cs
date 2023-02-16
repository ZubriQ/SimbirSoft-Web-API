using System.Net;

namespace Olymp_Project.Services.Kinds
{
    public interface IKindService
    {
        Task<Kind?> GetAnimalKindAsync(long id);
        Task<(HttpStatusCode, Kind?)> AddAnimalKindAsync(string name);
        Task<(HttpStatusCode, Kind?)> UpdateAnimalKindAsync(long id, string newName);
        Task<HttpStatusCode> DeleteAnimalKindAsync(long id);
    }
}
