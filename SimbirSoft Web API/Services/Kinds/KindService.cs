using SimbirSoft_Web_API.Helpers.Validators;
using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Kinds
{
    public class KindService : IKindService
    {
        private readonly ChipizationDbContext _db;

        public KindService(ChipizationDbContext context)
        {
            _db = context;
        }

        #region Get by id

        public async Task<IResponse<Kind>> GetAnimalKindByIdAsync(long? kindId)
        {
            if (!IdValidator.IsValid(kindId))
            {
                return new ServiceResponse<Kind>(HttpStatusCode.BadRequest);
            }

            if (await _db.Kinds.FirstOrDefaultAsync(k => k.Id == kindId) is not Kind kind)
            {
                return new ServiceResponse<Kind>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Kind>(HttpStatusCode.OK, kind);
        }

        #endregion

        #region Insert

        public async Task<IResponse<Kind>> InsertAnimalKindAsync(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ServiceResponse<Kind>(HttpStatusCode.BadRequest);
            }

            bool exists = await _db.Kinds.AnyAsync(k => k.Name == name);
            if (exists)
            {
                return new ServiceResponse<Kind>(HttpStatusCode.Conflict);
            }

            try
            {
                return await AddNewKind(name);
            }
            catch (Exception)
            {
                return new ServiceResponse<Kind>();
            }
        }

        private async Task<IResponse<Kind>> AddNewKind(string kind)
        {
            Kind newKind = new Kind() { Name = kind };
            await _db.Kinds.AddAsync(newKind);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Kind>(HttpStatusCode.Created, newKind);
        }

        #endregion

        #region Update

        public async Task<IResponse<Kind>> UpdateAnimalKindAsync(long? kindId, string? newName)
        {
            if (!IdValidator.IsValid(kindId) || string.IsNullOrWhiteSpace(newName))
            {
                return new ServiceResponse<Kind>(HttpStatusCode.BadRequest);
            }

            var exists = await _db.Kinds.AnyAsync(k => k.Name.ToLower() == newName.ToLower());
            if (exists)
            {
                return new ServiceResponse<Kind>(HttpStatusCode.Conflict);
            }

            if (await _db.Kinds.FirstOrDefaultAsync(k => k.Id == kindId) is not Kind kindToUpdate)
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

        private async Task<IResponse<Kind>> UpdateKind(Kind kind, string newName)
        {
            kind.Name = newName;
            _db.Entry(kind).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<Kind>(HttpStatusCode.OK, kind);
        }

        #endregion

        #region Remove

        public async Task<HttpStatusCode> RemoveAnimalKindByIdAsync(long? kindId)
        {
            if (!IdValidator.IsValid(kindId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await GetKindWithAnimalsAsync(kindId!.Value) is not Kind kind)
            {
                return HttpStatusCode.NotFound;
            }

            if (kind.Animals.Any())
            {
                return HttpStatusCode.BadRequest;
            }

            return await RemoveKindFromDatabaseAsync(kind);
        }

        private async Task<Kind?> GetKindWithAnimalsAsync(long kindId)
        {
            return await _db.Kinds
                .Include(k => k.Animals)
                .FirstOrDefaultAsync(k => k.Id == kindId);
        }

        private async Task<HttpStatusCode> RemoveKindFromDatabaseAsync(Kind kind)
        {
            try
            {
                _db.Kinds.Remove(kind);
                await _db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        #endregion
    }
}
