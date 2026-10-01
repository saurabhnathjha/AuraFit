namespace AuraFitWebService.DTOs
{
    public class UserProfileDTO
    {
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;  // e.g. "Male", "Female", "Other"
        public int HeightCm { get; set; }
        public float WeightKg { get; set; }
        public string ActivityLevel { get; set; } = string.Empty; // e.g. "Sedentary", "Lightly Active"
    }
}
