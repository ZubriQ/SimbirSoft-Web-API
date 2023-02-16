using System.Net;
using System.Xml.Linq;

namespace Olymp_Project.Services.Kinds
{
    public class KindService : IKindService
    {
        private readonly ChipizationDbContext _db;

        public KindService(ChipizationDbContext context)
        {
            _db = context;
        }

        public async Task<(HttpStatusCode, Kind?)> AddAnimalKindAsync(string name)
        {
            try
            {
                bool exists = await _db.Kinds.AnyAsync(k => k.Name == name);
                if (exists)
                {
                    return (HttpStatusCode.Conflict, null);
                }

                return await AddNewKind(name);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        private async Task<(HttpStatusCode, Kind)> AddNewKind(string name)
        {
            Kind newKind = new Kind() { Name = name };
            await _db.Kinds.AddAsync(newKind);
            await _db.SaveChangesAsync();
            return (HttpStatusCode.OK, newKind);
        }

        public async Task<HttpStatusCode> DeleteAnimalKindAsync(long id)
        {
            var kind = await _db.Kinds.Include(k => k.Animals).FirstOrDefaultAsync(k => k.Id == id);
            if (kind is null)
            {
                return HttpStatusCode.NotFound;
            }
            if (kind.Animals.Any())
            {
                return HttpStatusCode.BadRequest;
            }

            try
            {
                return await RemoveKind(kind);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<HttpStatusCode> RemoveKind(Kind kind)
        {
            _db.Remove(kind);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        public async Task<Kind?> GetAnimalKindAsync(long id)
        {
            return await _db.Kinds.FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<(HttpStatusCode, Kind?)> UpdateAnimalKindAsync(long id, string newName)
        {
            var kindToUpdate = await _db.Kinds.FirstOrDefaultAsync(k => k.Id == id);
            if (kindToUpdate is null)
            {
                return (HttpStatusCode.NotFound, null);
            }
            var exists = await _db.Kinds.AnyAsync(k => k.Name == newName);
            if (exists)
            {
                return (HttpStatusCode.Conflict, null);
            }
            // TODO: ToLower for string comparison?

            try
            {
                return await UpdateKind(kindToUpdate, newName);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }
        
        private async Task<(HttpStatusCode, Kind)> UpdateKind(Kind kindToUpdate, string newName)
        {
            kindToUpdate.Name = newName;
            _db.Entry(kindToUpdate).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return (HttpStatusCode.OK, kindToUpdate);
        }
    }
}
