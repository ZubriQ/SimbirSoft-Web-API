using Olymp_Project.Responses;

namespace Olymp_Project.Services.Animals
{
    public interface IAnimalService
    {
        Task<IServiceResponse<Animal>> GetAnimalByIdAsync(long? animalId);
        Task<IServiceResponse<ICollection<Animal>>> GetAnimalsAsync(AnimalQuery query, Paging paging);
        Task<IServiceResponse<Animal>> InsertAnimalAsync(Animal animal);
        Task<IServiceResponse<Animal>> UpdateAnimalAsync(long? animalId, PutAnimalDto request);
        Task<HttpStatusCode> RemoveAnimalAsync(long? animalId);
    }
}
