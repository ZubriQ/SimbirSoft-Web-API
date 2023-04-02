using System.Text.Json.Serialization;

namespace Olymp_Project.Dtos.Kind
{
    public class KindRequestDto
    {
        [JsonPropertyName("type")]
        public string? Name { get; set; }
    }
}
