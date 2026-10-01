using System.Text.Json.Serialization;

namespace AuraFitWebService.NutririonixModels
{
    public class NutritionixResponse
    {
        [JsonPropertyName("foods")]
        public List<NutritionixFood> Foods { get; set; } = new();
    }
}
