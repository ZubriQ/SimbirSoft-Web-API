namespace Olymp_Project.Database
{
    public class Animal
    {
        public long Id { get; set; }
        public long[] AnimalTypes { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; }
        public string LifeStatus { get; set; } = "ALIVE";
        public DateTime ChippingDateTime { get; set; } = DateTime.Now;
        public int ChipperId { get; set; }
        public long ChippingLocationId { get; set; }
        public long[] VisitedLocations { get; set; }
        public DateTime? DeathDateTime { get; set; } = null;

        public Animal(long id,
                      long[] animalTypes,
                      float weight,
                      float length,
                      float height,
                      string gender,
                      int chipperId,
                      long chippingLocationId,
                      long[] visitedLocations)
        {
            Id = id;
            AnimalTypes = animalTypes;
            Weight = weight;
            Length = length;
            Height = height;
            Gender = gender;
            ChipperId = chipperId;
            ChippingLocationId = chippingLocationId;
            VisitedLocations = visitedLocations;
        }
    }
}
