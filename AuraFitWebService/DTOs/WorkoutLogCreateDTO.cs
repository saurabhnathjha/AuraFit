namespace AuraFitWebService.DTOs
{
    public class WorkoutLogCreateDTO
    {
        public int? WorkoutTemplateId { get; set; }
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
        public int? DurationMin { get; set; }
        public int? CaloriesBurned { get; set; }

        public List<WorkoutLogExerciseCreateDTO> Exercises { get; set; }
    }

}
