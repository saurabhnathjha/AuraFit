namespace AuraFitWebService.DTOs
{
    public class UserGoalDTO
    {
        public string? GoalType { get; set; }  // "gain", "lose", "maintain" or null for auto  
        public double? WeeklyTargetKg { get; set; } // nullable - auto or manual  
        public int? TargetCalories { get; set; } // nullable - auto or manual  
        public bool AutoSuggested { get; set; } = true;  // default true = use autosuggestion  
        public DateOnly GoalStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today); // fixed conversion issue  
    }
}
