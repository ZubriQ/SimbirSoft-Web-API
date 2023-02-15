namespace Olymp_Project.Services.Animals
{
    public interface IAnimalService
    {
        Task<Animal?> GetAnimalAsync(long id);
    }
}
