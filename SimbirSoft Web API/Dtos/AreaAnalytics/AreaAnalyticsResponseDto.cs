namespace SimbirSoft_Web_API.Dtos.AreaAnalytics
{
    public class AreaAnalyticsResponseDto
    {
        public long TotalQuantityAnimals { get; set; }
        public long TotalAnimalsArrived { get; set; }
        public long TotalAnimalsGone { get; set; }
        public List<AnimalsAnalyticsDto> AnimalsAnalytics { get; set; } = null!;
    }
}
