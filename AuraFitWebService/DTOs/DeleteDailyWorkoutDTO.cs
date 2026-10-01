namespace AuraFitWebService.DTOs
{
    public class DeleteDailyWorkoutDTO
    {
        public int UserId { get; set; }
        public int ExerciseId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
    }
}
