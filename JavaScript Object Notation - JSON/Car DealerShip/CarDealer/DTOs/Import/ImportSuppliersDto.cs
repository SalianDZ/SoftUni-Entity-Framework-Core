using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CarDealer.DTOs.Import
{
    public class ImportSuppliersDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("isImporter")]
        public bool IsImporter { get; set; }
    }
}
