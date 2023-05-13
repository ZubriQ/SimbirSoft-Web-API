using Olymp_Project.Dtos.Path;
using Olymp_Project.Responses;
using Path = Olymp_Project.Models.Path;

namespace Olymp_Project.Services.Paths
{
    public interface IPathService
    {
        /// <summary>
        /// Inserts a bidirectional path.
        /// </summary>
        Task<IServiceResponse<Path>> InsertPathAsync(PathRequestDto request);
        Task<IServiceResponse<Path>> GetPathByIdAsync(long? pathId);
        Task<IServiceResponse<ICollection<Path>>> GetAllPathsAsync();
        Task<IServiceResponse<Path>> UpdatePathByIdAsync(long? pathId, PathRequestDto request);
        Task<HttpStatusCode> RemovePathByIdAsync(long? pathId);
    }
}
