using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.VisitedLocations
{
    public interface IVisitedLocationService
    {
        Task<IResponse<ICollection<VisitedLocation>>> GetVisitedLocationsAsync(
            long? animalId,
            DateTimeRangeQuery query,
            Paging paging);

        Task<IResponse<VisitedLocation>> InsertVisitedLocationAsync(
            long? animalId,
            long? locationId);

        Task<IResponse<VisitedLocation>> UpdateVisitedLocationAsync(
            long? animalId,
            VisitedLocationRequestDto request);

        Task<HttpStatusCode> RemoveVisitedLocationAsync(
            long? animalId,
            long? visitedLocationId);
    }
}
