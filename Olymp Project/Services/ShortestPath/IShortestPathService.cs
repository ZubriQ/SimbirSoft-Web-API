using Olymp_Project.Dtos.LocationPath;
using Olymp_Project.Dtos.ShortestPath;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.LocationsPaths
{
    public interface IShortestPathService
    {
        Task<IServiceResponse<DijkstraDto>> FindShortestPathWithDijkstraAsync(
            long? locationIdFrom, 
            long? locationIdTo);

        Task<IServiceResponse<BellmanFordDto>> FindShortestPathWithBellmanFordAsync(
            long? locationIdFrom,
            long? locationIdTo);
    }
}
