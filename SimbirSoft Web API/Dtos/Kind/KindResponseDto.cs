using System.Text.Json.Serialization;

namespace SimbirSoft_Web_API.Dtos.Kind
{
    public class KindResponseDto
    {
        public long Id { get; set; }

        [JsonPropertyName("type")]
        public string Name { get; set; } = null!;
    }
}
