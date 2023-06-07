using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Animals
{
    public interface IAnimalService
    {
        Task<IResponse<Animal>> GetAnimalByIdAsync(long? animalId);
        Task<IResponse<ICollection<Animal>>> GetAnimalsBySearchParameters(AnimalQuery query, Paging paging);
        Task<IResponse<Animal>> InsertAnimalAsync(Animal animal);
        Task<IResponse<Animal>> UpdateAnimalAsync(long? animalId, PutAnimalDto request);
        Task<HttpStatusCode> RemoveAnimalByIdAsync(long? animalId);
    }
}
