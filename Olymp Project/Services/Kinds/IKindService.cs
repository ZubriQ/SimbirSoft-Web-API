using Olymp_Project.Responses;

namespace Olymp_Project.Services.Kinds
{
    public interface IKindService
    {
        Task<IServiceResponse<Kind>> GetAnimalKindByIdAsync(long? kindId);
        Task<IServiceResponse<Kind>> InsertAnimalKindAsync(string? name);
        Task<IServiceResponse<Kind>> UpdateAnimalKindAsync(long? kindId, string? newName);
        Task<HttpStatusCode> RemoveAnimalKindAsync(long? kindId);
    }
}
