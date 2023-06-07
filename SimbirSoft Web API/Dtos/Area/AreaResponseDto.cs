namespace SimbirSoft_Web_API.Dtos.Area
{
    public class AreaResponseDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public AreaPointDto[]? AreaPoints { get; set; }
    }
}
