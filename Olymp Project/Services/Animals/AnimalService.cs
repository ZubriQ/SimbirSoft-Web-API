namespace Olymp_Project.Services.Animals
{
    public class AnimalService : IAnimalService
    {
        private readonly ChipizationDbContext _db;

        public AnimalService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<Animal?> GetAnimalAsync(long id)
        {
            return await _db.Animals.FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
