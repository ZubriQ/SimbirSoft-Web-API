using System.Text.Json.Serialization;

namespace SimbirSoft_Web_API.Dtos.AreaAnalytics
{
    public class AnimalsAnalyticsDto
    {
        [JsonPropertyName("animalType")]
        public string AnimalKind { get; set; } = null!;

        [JsonPropertyName("animalTypeId")]
        public long AnimalKindId { get; set; }

        public long QuantityAnimals { get; set; }

        public long AnimalsArrived { get; set; }

        public long AnimalsGone { get; set; }
    }
}
