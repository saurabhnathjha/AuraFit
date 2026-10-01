using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class TemplateExercise
{
    public int TemplateExerciseId { get; set; }

    public int WorkoutDayId { get; set; }

    public int ExerciseCatalogId { get; set; }

    public int ExerciseOrder { get; set; }

    public string? Reps { get; set; }

    public int? Sets { get; set; }

    public int? RestSeconds { get; set; }

    public string? Notes { get; set; }

    public virtual ExerciseCatalog ExerciseCatalog { get; set; } = null!;

    public virtual WorkoutDay WorkoutDay { get; set; } = null!;
}
