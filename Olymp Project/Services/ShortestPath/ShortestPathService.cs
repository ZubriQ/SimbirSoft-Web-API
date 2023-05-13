using Olymp_Project.Dtos.LocationPath;
using Olymp_Project.Dtos.ShortestPath;
using Olymp_Project.Helpers.Geospatial;
using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;
using Path = Olymp_Project.Models.Path;

namespace Olymp_Project.Services.LocationsPaths
{
    /// <summary>
    /// A hypothetical service to implement in the 3rd stage of the contest.
    /// </summary>
    public class ShortestPathService : IShortestPathService
    {
        private readonly ChipizationDbContext _db;
        private readonly WeightedPathFinder _pathFinder;
        private readonly NegativeWeightPathFinder _negativePathFinder;

        public ShortestPathService(ChipizationDbContext db)
        {
            _db = db;
            _pathFinder = new();
            _negativePathFinder = new();
        }

        #region Dijkstra

        public async Task<IServiceResponse<DijkstraDto>> FindShortestPathWithDijkstraAsync(
            long? locationIdFrom, long? locationIdTo)
        {
            if (!IdValidator.IsValid(locationIdFrom, locationIdTo) ||
                locationIdFrom == locationIdTo)
            {
                return new ServiceResponse<DijkstraDto>(HttpStatusCode.BadRequest);
            }

            var paths = await GetPathsAsync();
            var start = await GetLocationByIdAsync(locationIdFrom!.Value);
            var end = await GetLocationByIdAsync(locationIdTo!.Value);
            if (start is null || end is null)
            {
                return new ServiceResponse<DijkstraDto>(HttpStatusCode.NotFound);
            }
            
            var shortestPathDto = _pathFinder.FindShortestPath(paths.ToList(), start, end);
            if (shortestPathDto.Path is null)
            {
                return new ServiceResponse<DijkstraDto>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<DijkstraDto>(HttpStatusCode.OK, shortestPathDto);
        }

        private async Task<IEnumerable<Path>> GetPathsAsync()
        {
            return await _db.Paths
                .Include(p => p.EndLocation)
                .Include(p => p.StartLocation)
                .ToListAsync();
        }

        private async Task<Location?> GetLocationByIdAsync(long id)
        {
            return await _db.Locations
                .Include(l => l.PathsFrom)
                .Include(l => l.PathsTo)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        #endregion

        #region Bellman Ford

        public async Task<IServiceResponse<BellmanFordDto>> FindShortestPathWithBellmanFordAsync(
            long? locationIdFrom, long? locationIdTo)
        {
            if (!IdValidator.IsValid(locationIdFrom, locationIdTo) ||
                locationIdFrom == locationIdTo)
            {
                return new ServiceResponse<BellmanFordDto>(HttpStatusCode.BadRequest);
            }

            var paths = await GetPathsAsync();
            var start = await GetLocationByIdAsync(locationIdFrom!.Value);
            var end = await GetLocationByIdAsync(locationIdTo!.Value);
            if (start is null || end is null)
            {
                return new ServiceResponse<BellmanFordDto>(HttpStatusCode.NotFound);
            }

            var shortestPathDto = _negativePathFinder.FindShortestPath(paths.ToList(), start, end);
            if (shortestPathDto.Path is null)
            {
                return new ServiceResponse<BellmanFordDto>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<BellmanFordDto>(HttpStatusCode.OK, shortestPathDto);
        }

        #endregion
    }
}
