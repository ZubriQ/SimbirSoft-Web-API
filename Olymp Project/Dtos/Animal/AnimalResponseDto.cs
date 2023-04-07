using System.Text.Json.Serialization;

namespace Olymp_Project.Dtos.Animal
{
    public class AnimalResponseDto
    {
        public long Id { get; set; }

        [JsonPropertyName("animalTypes")]
        public long[] AnimalKinds { get; set; } = null!;
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = null!;
        public string LifeStatus { get; set; } = null!;
        public string ChippingDateTime { get; set; } = null!;
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[] VisitedLocations { get; set; } = null!;
        public string? DeathDateTime { get; set; }
    }
}