namespace Olymp_Project.Dtos.Animal
{
    public class PutAnimalDto
    {
        public float? Weight { get; set; }
        public float? Length { get; set; }
        public float? Height { get; set; }
        public string? Gender { get; set; }
        public string? LifeStatus { get; set; }
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }
    }
}
