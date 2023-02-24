

namespace Olymp_Project.Services.AnimalsKinds
{
    public interface IAnimalKindService
    {
        Task<(HttpStatusCode, Animal?)> InsertKindToAnimalAsync(long animalId, long kindId);
        Task<(HttpStatusCode, Animal?)> UpdateAnimalKindAsync(long animalId, PutAnimalKindDto request);
        Task<(HttpStatusCode, Animal?)> RemoveAnimalKindAsync(long animalId, long kindId);
    }
}
