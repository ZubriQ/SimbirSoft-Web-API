namespace Olymp_Project.Dtos.Animal
{
    public class PostAnimalDto
    {
        public long[]? AnimalTypes { get; set; }
        public float? Weight { get; set; }
        public float? Length { get; set; }
        public float? Height { get; set; }
        public string? Gender { get; set; }
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }
    }
}
