namespace Olymp_Project.Dtos.AreaAnalytics
{
    public class AreaAnalyticsResponseDto
    {
        public long TotalQuantityAnimals { get; set; }
        public long TotalAnimalsArrived { get; set; }
        public long TotalAnimalsGone { get; set; }
        public AnimalsAnalyticsDto[] AnimalsAnalytics { get; set; } = null!;
    }
}
