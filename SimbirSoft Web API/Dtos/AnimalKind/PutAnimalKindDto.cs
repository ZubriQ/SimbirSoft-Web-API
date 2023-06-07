using System.Text.Json.Serialization;

namespace SimbirSoft_Web_API.Dtos.AnimalKind
{
    public class PutAnimalKindDto
    {
        [JsonPropertyName("oldTypeId")]
        public long? OldKindId { get; set; }

        [JsonPropertyName("newTypeId")]
        public long? NewKindId { get; set; }
    }
}
