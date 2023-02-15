using System.Net;

namespace Olymp_Project.Services.Kinds
{
    public class KindService : IKindService
    {
        private readonly ChipizationDbContext _db;

        public KindService(ChipizationDbContext context)
        {
            _db = context;
        }

        public async Task<(HttpStatusCode, Kind?)> AddAnimalKindAsync(Kind location)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpStatusCode> DeleteAnimalKindAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<Kind?> GetAnimalKindAsync(long id)
        {
            return await _db.Kinds.FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<(HttpStatusCode, Kind?)> UpdateAnimalKindAsync(long id, Kind location)
        {
            throw new NotImplementedException();
        }
    }
}
