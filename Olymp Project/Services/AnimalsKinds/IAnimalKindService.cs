using Olymp_Project.Responses;

namespace Olymp_Project.Services.AnimalsKinds
{
    public interface IAnimalKindService
    {
        Task<IServiceResponse<Animal>> InsertKindToAnimalAsync(long animalId, long kindId);
        Task<IServiceResponse<Animal>> UpdateAnimalKindAsync(long animalId, PutAnimalKindDto request);
        Task<IServiceResponse<Animal>> RemoveAnimalKindAsync(long animalId, long kindId);
    }
}
