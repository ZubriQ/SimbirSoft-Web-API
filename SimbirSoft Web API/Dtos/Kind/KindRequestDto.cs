using System.Text.Json.Serialization;

namespace SimbirSoft_Web_API.Dtos.Kind
{
    public class KindRequestDto
    {
        [JsonPropertyName("type")]
        public string? Name { get; set; }
    }
}
