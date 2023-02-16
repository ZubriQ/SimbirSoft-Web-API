namespace Olymp_Project.Dtos.Animal
{
    public class GetAnimalDto
    {
        public long Id { get; set; }
        public long[] AnimalKinds { get; set; } = null!;
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = null!;
        public string LifeStatus { get; set; } = null!;
        public DateTime ChippingDateTime { get; set; }
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[] VisitedLocations { get; set; } = null!;
        public DateTime? DeathDateTime { get; set; } = null;
    }
}