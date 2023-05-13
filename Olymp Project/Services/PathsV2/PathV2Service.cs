using Olymp_Project.Dtos.Path;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;
using Path = Olymp_Project.Models.Path;

namespace Olymp_Project.Services.PathsV2
{
    public class PathV2Service : IPathV2Service
    {
        private readonly ChipizationDbContext _db;

        public PathV2Service(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Insert

        public async Task<IServiceResponse<Path>> InsertPathAsync(PathRequestDto request)
        {
            if (!IdValidator.IsValid(request.EndLocationId, request.StartLocationId) ||
                request.EndLocationId == request.StartLocationId)
            {
                return new ServiceResponse<Path>(HttpStatusCode.BadRequest);
            }

            if (!await LocationExist(request.EndLocationId!.Value) ||
                !await LocationExist(request.StartLocationId!.Value))
            {
                return new ServiceResponse<Path>(HttpStatusCode.NotFound);
            }

            return await AddPathToDatabaseAsync(request);
        }

        private async Task<bool> LocationExist(long id)
        {
            return await _db.Locations.AnyAsync(l => l.Id == id);
        }

        private async Task<IServiceResponse<Path>> AddPathToDatabaseAsync(PathRequestDto request)
        {
            var path = new Path()
            {
                StartLocationId = request.StartLocationId!.Value,
                EndLocationId = request.EndLocationId!.Value,
                Weight = request.Weight!.Value
            };
            await _db.Paths.AddAsync(path);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Path>(HttpStatusCode.Created, path);
        }

        #endregion
    }
}
