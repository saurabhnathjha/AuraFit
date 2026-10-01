public class WorkoutLogExerciseCreateDTO
{
    public int ExerciseCatalogId { get; set; }
    public int? Reps { get; set; }
    public int? Sets { get; set; }
    public float? WeightKg { get; set; }
    public int? DurationMin { get; set; }

    // ✅ Make this optional
    public int? CaloriesBurned { get; set; }

    public bool PersonalRecord { get; set; }
    public string? Notes { get; set; }
}
