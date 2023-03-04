using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.Kinds
{
    public class KindService : IKindService
    {
        private readonly ChipizationDbContext _db;

        public KindService(ChipizationDbContext context)
        {
            _db = context;
        }

        public async Task<IServiceResponse<Kind>> GetAnimalKindAsync(long? id)
        {
            if (!IdValidator.IsValid(id))
            {
                return new ServiceResponse<Kind>(HttpStatusCode.BadRequest);
            }

            if (await _db.Kinds.FirstOrDefaultAsync(k => k.Id == id) is not Kind kind)
            {
                return new ServiceResponse<Kind>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Kind>(HttpStatusCode.OK, kind);
        }

        public async Task<IServiceResponse<Kind>> InsertAnimalKindAsync(string? type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return new ServiceResponse<Kind>(HttpStatusCode.BadRequest);
            }

            bool exists = await _db.Kinds.AnyAsync(k => k.Name == type);
            if (exists)
            {
                return new ServiceResponse<Kind>(HttpStatusCode.Conflict);
            }

            try
            {
                return await AddNewKind(type);
            }
            catch (Exception)
            {
                return new ServiceResponse<Kind>();
            }
        }

        private async Task<IServiceResponse<Kind>> AddNewKind(string type)
        {
            Kind newKind = new Kind() { Name = type };
            await _db.Kinds.AddAsync(newKind);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Kind>(HttpStatusCode.Created, newKind);
        }

        public async Task<IServiceResponse<Kind>> UpdateAnimalKindAsync(long? id, string? newName)
        {
            if (!IdValidator.IsValid(id) || string.IsNullOrWhiteSpace(newName))
            {
                return new ServiceResponse<Kind>(HttpStatusCode.BadRequest);
            }

            var exists = await _db.Kinds.AnyAsync(k => k.Name.ToLower() == newName.ToLower());
            if (exists)
            {
                return new ServiceResponse<Kind>(HttpStatusCode.Conflict);
            }

            if (await _db.Kinds.FirstOrDefaultAsync(k => k.Id == id) is not Kind kindToUpdate)
            {
                return new ServiceResponse<Kind>(HttpStatusCode.NotFound);
            }

            try
            {
                return await UpdateKind(kindToUpdate, newName);
            }
            catch (Exception)
            {
                return new ServiceResponse<Kind>();
            }
        }
        
        private async Task<IServiceResponse<Kind>> UpdateKind(Kind kind, string newName)
        {
            kind.Name = newName;
            _db.Entry(kind).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<Kind>(HttpStatusCode.OK, kind);
        }

        public async Task<HttpStatusCode> RemoveAnimalKindAsync(long? id)
        {
            if (!IdValidator.IsValid(id))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await _db.Kinds.Include(k => k.Animals).FirstOrDefaultAsync(k => k.Id == id) 
                is not Kind kind)
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
            _db.Kinds.Remove(kind);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }
    }
}
