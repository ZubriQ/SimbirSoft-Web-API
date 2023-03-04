namespace Olymp_Project.Dtos.VisitedLocation
{
    public class VisitedLocationResponseDto
    {
        public long Id { get; set; }
        public string DateTimeOfVisitLocationPoint { get; set; } = null!;
        public long LocationPointId { get; set; }
    }
}
