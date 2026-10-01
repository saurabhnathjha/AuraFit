
namespace AuraFitWebService.Utilities
{
    public static class GoalCalculator
    {
        public static string SuggestGoalType(double bmi)
        {
            if (bmi < 18.5) return "gain";
            if (bmi < 25) return "maintain";
            return "lose";
        }

        public static double SuggestWeeklyTargetKg(string goalType, double currentWeightKg)
        {
            return goalType.ToLower() switch
            {
                "gain" => Math.Round(currentWeightKg * 0.005, 2),    // ~0.5% of body weight
                "lose" => Math.Round(currentWeightKg * 0.0075, 2),   // ~0.75% of body weight
                _ => 0.0
            };
        }

        public static int CalculateTargetCalories(double tdee, string goalType, double currentWeightKg)
        {
            double adjustment = goalType.ToLower() switch
            {
                "gain" => Math.Clamp(currentWeightKg * 3, 250, 500),  // Gain: 250–500 kcal
                "lose" => -Math.Clamp(currentWeightKg * 3.5, 300, 700), // Loss: 300–700 kcal
                _ => 0
            };

            return (int)(tdee + adjustment);
        }

    }
}
