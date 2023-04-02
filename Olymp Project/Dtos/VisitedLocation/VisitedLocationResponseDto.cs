using System.Text.Json.Serialization;

namespace Olymp_Project.Dtos.VisitedLocation
{
    public class VisitedLocationResponseDto
    {
        public long Id { get; set; }

        [JsonPropertyName("dateTimeOfVisitLocationPoint")]
        public string VisitDateTime { get; set; } = null!;

        public long LocationPointId { get; set; }
    }
}
