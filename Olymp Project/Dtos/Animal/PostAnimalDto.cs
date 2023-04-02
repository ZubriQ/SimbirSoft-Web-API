using System.Text.Json.Serialization;

namespace Olymp_Project.Dtos.Animal
{
    public class PostAnimalDto
    {
        [JsonPropertyName("animalTypes")]
        public long[]? AnimalKinds { get; set; }
        public float? Weight { get; set; }
        public float? Length { get; set; }
        public float? Height { get; set; }
        public string? Gender { get; set; }
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }
    }
}
