namespace AuraFitWebService.Utilities
{
    public class FitnessCalculator
    {
        public static float CalculateBMI(float weightKg, int heightCm)
        {
            var heightM = heightCm / 100f;
            return weightKg / (heightM * heightM);
        }

        // BMR calculation using Mifflin-St Jeor Equation
        // For men: BMR = 10*weight + 6.25*height - 5*age + 5
        // For women: BMR = 10*weight + 6.25*height - 5*age - 161
        public static float CalculateBMR(float weightKg, int heightCm, int age, string gender)
        {
            if (gender.ToLower().StartsWith("m"))
            {
                return 10 * weightKg + 6.25f * heightCm - 5 * age + 5;
            }
            else
            {
                return 10 * weightKg + 6.25f * heightCm - 5 * age - 161;
            }
        }

        // TDEE = BMR * ActivityMultiplier
        // Activity multipliers based on ActivityLevel string
        public static float CalculateTDEE(float bmr, string activityLevel)
        {
            return bmr * (activityLevel.ToLower() switch
            {
                "sedentary" => 1.2f,
                "lightly active" => 1.375f,
                "moderately active" => 1.55f,
                "very active" => 1.725f,
                "extra active" => 1.9f,
                _ => 1.2f, // default sedentary
            });
        }
    }
}
