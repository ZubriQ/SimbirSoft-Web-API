using System.Net;

namespace Olymp_Project.Services.Animals
{
    public interface IAnimalService
    {
        Task<Animal?> GetAnimalAsync(long id);
        Task<IEnumerable<Animal>> GetAnimalsAsync(AnimalQuery query, Paging paging);
        Task<(HttpStatusCode, Animal?)> InsertAnimalAsync(Animal animal);
        Task<(HttpStatusCode, Animal?)> UpdateAnimalAsync(long id, PutAnimalDto request);
    }
}
