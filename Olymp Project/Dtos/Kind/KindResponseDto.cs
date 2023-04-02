using System.Text.Json.Serialization;

namespace Olymp_Project.Dtos.Kind
{
    public class KindResponseDto
    {
        public long Id { get; set; }

        [JsonPropertyName("type")]
        public string Name { get; set; } = null!;
    }
}
