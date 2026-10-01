namespace AuraFitWebService.DTOs
{
    public class WorkoutLogReadDTO
    {
        public int WorkoutLogId { get; set; }
        public DateTime LoggedAt { get; set; }
        public int DurationMin { get; set; }
        public int CaloriesBurned { get; set; }

        public string? WorkoutTemplateName { get; set; } // Optional: show what plan was followed
        public List<WorkoutLogExerciseReadDTO> Exercises { get; set; }
    }

}
