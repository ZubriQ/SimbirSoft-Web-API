namespace Olymp_Project.Services
{
    public interface IAnimalService
    {
        Task<Animal?> GetAnimalAsync(long id);
    }
}
