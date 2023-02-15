namespace Olymp_Project.Services
{
    public class AnimalService
    {
        private readonly AnimalChipizationContext _db;

        public AnimalService(AnimalChipizationContext db)
        {
            _db = db;
        }

        public async Task<Animal?> GetAnimalAsync(long id)
        {
            try
            {
                return await _db.Animals
                    //.AsNoTracking()
                    //.Include(a => a.VisitedLocations)
                    //.Include(a => a.Types)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
