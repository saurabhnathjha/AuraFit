namespace AuraFitWebService.DTOs
{
    public class WorkoutLogExerciseReadDTO
    {
        public string ExerciseName { get; set; }
        public string Category { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
        public float? WeightKg { get; set; }
        public int DurationMin { get; set; }
        public int? CaloriesBurned { get; set; }
        public bool PersonalRecord { get; set; }
        public string Notes { get; set; }
    }

}
