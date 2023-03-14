using Olymp_Project.Responses;

namespace Olymp_Project.Services.VisitedLocations
{
    public interface IVisitedLocationService
    {
        Task<IServiceResponse<ICollection<VisitedLocation>>> GetVisitedLocationsAsync(
            long? animalId,
            DateTimeRangeQuery query,
            Paging paging);

        Task<IServiceResponse<VisitedLocation>> InsertVisitedLocationAsync(
            long? animalId,
            long? locationId);

        Task<IServiceResponse<VisitedLocation>> UpdateVisitedLocationAsync(
            long? animalId,
            VisitedLocationRequestDto request);

        Task<HttpStatusCode> RemoveVisitedLocationAsync(
            long? animalId,
            long? visitedLocationId);
    }
}
