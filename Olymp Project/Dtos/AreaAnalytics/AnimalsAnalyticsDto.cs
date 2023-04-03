namespace Olymp_Project.Dtos.AreaAnalytics
{
    public class AnimalsAnalyticsDto
    {
        public string AnimalType { get; set; } = null!;
        public long AnimalTypeId { get; set; }
        public long QuantityAnimals { get; set; }
        public long AnimalsArrived { get; set; }
        public long AnimalsGone { get; set; }
    }
}
