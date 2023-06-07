using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.AnimalsKinds
{
    public interface IAnimalKindService
    {
        Task<IResponse<Animal>> InsertKindToAnimalAsync(long? animalId, long? kindId);
        Task<IResponse<Animal>> UpdateAnimalKindAsync(long? animalId, PutAnimalKindDto request);
        Task<IResponse<Animal>> RemoveAnimalKindAsync(long? animalId, long? kindId);
    }
}
