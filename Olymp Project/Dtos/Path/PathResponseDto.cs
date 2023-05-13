namespace Olymp_Project.Dtos.Path
{
    public class PathResponseDto
    {
        public long Id { get; set; }
        public long StartLocationId { get; set; }
        public long EndLocationId { get; set; }
        public double Weight { get; set; }
    }
}
