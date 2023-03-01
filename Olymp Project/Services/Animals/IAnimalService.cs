using Olymp_Project.Responses;
using System.Net;

namespace Olymp_Project.Services.Animals
{
    public interface IAnimalService
    {
        Task<IServiceResponse<Animal>> GetAnimalAsync(long id);
        Task<IServiceResponse<ICollection<Animal>>> GetAnimalsAsync(AnimalQuery query, Paging paging);
        Task<IServiceResponse<Animal>> InsertAnimalAsync(Animal animal);
        Task<IServiceResponse<Animal>> UpdateAnimalAsync(long id, PutAnimalDto request);
        Task<HttpStatusCode> RemoveAnimalAsync(long id);
    }
}
