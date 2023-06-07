using System.Text.Json.Serialization;

namespace SimbirSoft_Web_API.Dtos.VisitedLocation
{
    public class VisitedLocationRequestDto
    {
        /// <summary>
        /// Visited location that we need to update.
        /// </summary>
        [JsonPropertyName("visitedLocationPointId")]
        public long? VisitedLocationId { get; set; }

        /// <summary>
        /// New locationId for a concrete VisitedLocation.
        /// </summary>
        [JsonPropertyName("locationPointId")]
        public long? LocationId { get; set; }
    }
}
