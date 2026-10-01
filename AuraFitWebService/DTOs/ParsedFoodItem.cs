namespace AuraFitWebService.DTOs
{
    public class ParsedFoodItem
    {
        public string Name { get; set; } = null!;
        public string? Quantity { get; set; }
        public int Calories { get; set; }
    }
}
