using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Kinds
{
    public interface IKindService
    {
        Task<IResponse<Kind>> GetAnimalKindByIdAsync(long? kindId);
        Task<IResponse<Kind>> InsertAnimalKindAsync(string? name);
        Task<IResponse<Kind>> UpdateAnimalKindAsync(long? kindId, string? newName);
        Task<HttpStatusCode> RemoveAnimalKindByIdAsync(long? kindId);
    }
}
