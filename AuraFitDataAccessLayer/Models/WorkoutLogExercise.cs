using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuraFitDataAccessLayer.Models;

public partial class WorkoutLogExercise
{
    public int WorkoutLogExerciseId { get; set; }

    public int WorkoutLogId { get; set; }

    public int ExerciseCatalogId { get; set; }

    public int? Reps { get; set; }

    public int? Sets { get; set; }

    public double? WeightKg { get; set; }

    public int? DurationMin { get; set; }

    public int? CaloriesBurned { get; set; }

    public bool? PersonalRecord { get; set; }

    public string? Notes { get; set; }

    public virtual ExerciseCatalog ExerciseCatalog { get; set; } = null!;
    [JsonIgnore]
    public virtual WorkoutLog WorkoutLog { get; set; } = null!;
}
