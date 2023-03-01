using Olymp_Project.Responses;

namespace Olymp_Project.Services.Kinds
{
    public interface IKindService
    {
        Task<IServiceResponse<Kind>> GetAnimalKindAsync(long id);
        Task<IServiceResponse<Kind>> InsertAnimalKindAsync(string name);
        Task<IServiceResponse<Kind>> UpdateAnimalKindAsync(long id, string newName);
        Task<HttpStatusCode> RemoveAnimalKindAsync(long id);
    }
}
