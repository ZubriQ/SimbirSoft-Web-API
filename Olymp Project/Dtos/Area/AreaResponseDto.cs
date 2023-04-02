namespace Olymp_Project.Dtos.Area
{
    public class AreaResponseDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public AreaPointsDto[]? AreaPoints { get; set; }
    }
}
