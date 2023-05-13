using Microsoft.Extensions.Configuration;
using Npgsql;
using Olymp_Project.Dtos.Path;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;
using System.Data;
using Path = Olymp_Project.Models.Path;

namespace Olymp_Project.Services.Paths
{
    /// <summary>
    /// A hypothetical service to implement in the 3rd stage of the contest.
    /// </summary>
    public class PathService : IPathService
    {
        private readonly ChipizationDbContext _db;

        public PathService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Insert

        public async Task<IServiceResponse<Path>> InsertPathAsync(PathRequestDto request)
        {
            if (!IdValidator.IsValid(request.StartLocationId, request.EndLocationId) ||
                !PathRequestValidator.IsWeightValid(request.Weight))
            {
                return new ServiceResponse<Path>(HttpStatusCode.BadRequest);
            }

            if (await PathAlreadyExists(request.StartLocationId!.Value, request.EndLocationId!.Value) ||
                request.StartLocationId!.Value == request.EndLocationId!.Value)
            {
                return new ServiceResponse<Path>(HttpStatusCode.Conflict);
            }

            if (!await LocationsExist(request.StartLocationId!.Value, request.EndLocationId!.Value))
            {
                return new ServiceResponse<Path>(HttpStatusCode.NotFound);
            }

            return await AddPathToDatabaseAsync(request);
        }

        private async Task<bool> PathAlreadyExists(long startLocationId, long endLocationId)
        {
            return await _db.Paths.AnyAsync(p =>
                p.StartLocationId == startLocationId &&
                p.EndLocationId == endLocationId);
        }

        private async Task<bool> LocationsExist(long startLocationId, long endLocationId)
        {
            return await _db.Locations.AnyAsync(l => l.Id == endLocationId) ||
                   await _db.Locations.AnyAsync(l => l.Id == startLocationId);
        }

        private async Task<IServiceResponse<Path>> AddPathToDatabaseAsync(PathRequestDto request)
        {
            (var path1, var path2) = CreateBidirectionalPaths(request);
            _db.Paths.Add(path1);
            _db.Paths.Add(path2);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Path>(HttpStatusCode.Created, path1);
        }

        private (Path path1, Path path2) CreateBidirectionalPaths(PathRequestDto request)
        {
            var path1 = new Path()
            {
                EndLocationId = request.EndLocationId!.Value,
                StartLocationId = request.StartLocationId!.Value,
                Weight = request.Weight!.Value
            };

            var path2 = new Path()
            {
                EndLocationId = request.StartLocationId!.Value,
                StartLocationId = request.EndLocationId!.Value,
                Weight = request.Weight!.Value,
                IsReversed = true
            };

            return (path1, path2);
        }

        #endregion

        #region Get by id

        public async Task<IServiceResponse<Path>> GetPathByIdAsync(long? pathId)
        {
            if (!IdValidator.IsValid(pathId))
            {
                return new ServiceResponse<Path>(HttpStatusCode.BadRequest);
            }

            if (await _db.Paths.FindAsync(pathId!.Value) is not Path path)
            {
                return new ServiceResponse<Path>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Path>(HttpStatusCode.OK, path);
        }

        #endregion

        #region

        public async Task<IServiceResponse<ICollection<Path>>> GetAllPathsAsync()
        {
            var response = new CollectionServiceResponse<Path>(HttpStatusCode.OK, _db.GetAllPaths().ToList());
            return await Task.FromResult<IServiceResponse<ICollection<Path>>>(response);
        }

        #endregion

        #region Update

        public async Task<IServiceResponse<Path>> UpdatePathByIdAsync(long? pathId, PathRequestDto request)
        {
            #region Validation

            if (!IdValidator.IsValid(request.StartLocationId, request.EndLocationId) ||
                !PathRequestValidator.IsWeightValid(request.Weight))
            {
                return new ServiceResponse<Path>(HttpStatusCode.BadRequest);
            }

            if (await PathAlreadyExists(request.StartLocationId!.Value, request.EndLocationId!.Value) ||
                request.StartLocationId!.Value == request.EndLocationId!.Value)
            {
                return new ServiceResponse<Path>(HttpStatusCode.Conflict);
            }

            if (!await LocationsExist(request.StartLocationId!.Value, request.EndLocationId!.Value))
            {
                return new ServiceResponse<Path>(HttpStatusCode.NotFound);
            }

            if (await _db.Paths.FindAsync(pathId) is not Path pathToUpdate)
            {
                return new ServiceResponse<Path>(HttpStatusCode.NotFound);
            }

            #endregion

            return await UpdatePathInDatabaseAsync(
                pathToUpdate, request.StartLocationId!.Value, request.EndLocationId!.Value, request.Weight!.Value);
        }

        private async Task<IServiceResponse<Path>> UpdatePathInDatabaseAsync(
            Path path, long startLocationId, long endLocationId, double weight)
        {
            var secondBidirectionalPath = await _db.Paths.FirstOrDefaultAsync(p =>
                p.EndLocationId == path.StartLocationId &&
                p.StartLocationId == path.EndLocationId);

            path.StartLocationId = startLocationId;
            path.EndLocationId = endLocationId;
            path.Weight = weight;

            secondBidirectionalPath!.EndLocationId = startLocationId;
            secondBidirectionalPath.StartLocationId = endLocationId;
            secondBidirectionalPath.Weight = weight;

            await _db.SaveChangesAsync();

            return new ServiceResponse<Path>(HttpStatusCode.OK, path);
        }

        #endregion

        #region Delete

        public async Task<HttpStatusCode> RemovePathByIdAsync(long? pathId)
        {
            if (!IdValidator.IsValid(pathId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await _db.Paths.FindAsync(pathId!.Value) is not Path pathToRemove)
            {
                return HttpStatusCode.NotFound;
            }

            return await RemovePathFromDatabaseAsync(pathToRemove);
        }

        private async Task<HttpStatusCode> RemovePathFromDatabaseAsync(Path pathToRemove)
        {
            var bidirectionalPathToDelete = await _db.Paths.FirstOrDefaultAsync(p =>
                p.EndLocationId == pathToRemove.StartLocationId &&
                p.StartLocationId == pathToRemove.EndLocationId);

            _db.Paths.Remove(pathToRemove);
            if (bidirectionalPathToDelete is not null)
            {
                _db.Paths.Remove(bidirectionalPathToDelete!);
            }
            await _db.SaveChangesAsync();

            return HttpStatusCode.OK;
        }

        #endregion
    }
}
