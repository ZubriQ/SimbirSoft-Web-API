using System.Text.Json.Serialization;

namespace SimbirSoft_Web_API.Dtos.VisitedLocation
{
    public class VisitedLocationResponseDto
    {
        public long Id { get; set; }

        [JsonPropertyName("dateTimeOfVisitLocationPoint")]
        public string VisitDateTime { get; set; } = null!;

        public long LocationPointId { get; set; }
    }
}
