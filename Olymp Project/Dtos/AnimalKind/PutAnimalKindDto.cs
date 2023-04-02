using System.Text.Json.Serialization;

namespace Olymp_Project.Dtos.AnimalKind
{
    public class PutAnimalKindDto
    {
        [JsonPropertyName("oldTypeId")]
        public long? OldKindId { get; set; }

        [JsonPropertyName("newTypeId")]
        public long? NewKindId { get; set; }
    }
}
