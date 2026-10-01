using AuraFitWebService.DTOs;

namespace AuraFitWebService.Controllers
{
    public class MealLogRequestDTO
    {
        public string Name { get; set; } = null!;           // e.g. "Lunch"
        public string? Description { get; set; }            // optional notes
        public string Query { get; set; } = null!;
    }
}
