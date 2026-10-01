public class AddDailyWorkoutDTO
{
    public int UserId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public List<int> ExerciseIds { get; set; } = new();
}
