namespace AuraFitWebService.Utilities
{
    public static class CalorieHelper
    {
        private static readonly Dictionary<string, double> DefaultMets = new()
        {
            { "cardio", 7.0 },
            { "strength", 6.0 },
            { "yoga", 3.0 },
            { "hiit", 8.0 },
            { "default", 5.0 }
        };

        /// <summary>
        /// Estimates calories burned using MET value, duration, and weight.
        /// </summary>
        public static int EstimateCaloriesFromMet(double met, int durationMin, double weightKg)
        {
            return (int)(met * weightKg * (durationMin / 60.0));
        }

        /// <summary>
        /// Estimates calories burned using default workout type categories if MET is unknown.
        /// </summary>
        public static int EstimateCaloriesFromType(string? type, int durationMin, double weightKg)
        {
            var met = DefaultMets.TryGetValue(type?.ToLower() ?? "", out var val)
                ? val
                : DefaultMets["default"];

            return EstimateCaloriesFromMet(met, durationMin, weightKg);
        }
    }
}
