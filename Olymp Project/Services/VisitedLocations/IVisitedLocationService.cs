using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Dtos.VisitedLocation;
using System.Net;

namespace Olymp_Project.Services.VisitedLocations
{
    public interface IVisitedLocationService
    {
        Task<(HttpStatusCode, IEnumerable<VisitedLocation>)> GetVisitedLocationsAsync(
            long animalId, 
            DateTimeRangeQuery query, 
            Paging paging);

        Task<(HttpStatusCode, VisitedLocation?)> InsertVisitedLocationAsync(
            long animalId, 
            long locationId);

        Task<(HttpStatusCode, VisitedLocation?)> UpdateVisitedLocationAsync(
            long animalId,
            VisitedLocationRequestDto request);

        Task<HttpStatusCode> RemoveVisitedLocationAsync(long animalId, long visitedLocationId);
    }
}
