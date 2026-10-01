using System.Text.Json.Serialization;

namespace AuraFitWebService.NutririonixModels
{
    public class NutritionixFood
    {
        [JsonPropertyName("food_name")]
        public string FoodName { get; set; } = null!;

        [JsonPropertyName("serving_qty")]
        public double ServingQty { get; set; }

        [JsonPropertyName("serving_unit")]
        public string ServingUnit { get; set; } = null!;

        [JsonPropertyName("nf_calories")]
        public double Calories { get; set; }
    }
}
