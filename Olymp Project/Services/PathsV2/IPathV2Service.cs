using Olymp_Project.Dtos.Path;
using Olymp_Project.Responses;
using Path = Olymp_Project.Models.Path;

namespace Olymp_Project.Services.PathsV2
{
    public interface IPathV2Service
    {
        /// <summary>
        /// Inserts a directed path.
        /// </summary>
        Task<IServiceResponse<Path>> InsertPathAsync(PathRequestDto request);
    }
}
