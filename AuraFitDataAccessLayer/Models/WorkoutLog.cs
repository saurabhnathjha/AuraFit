using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class WorkoutLog
{
    public int WorkoutLogId { get; set; }

    public int UserId { get; set; }

    public int? WorkoutTemplateId { get; set; }

    public int? DurationMin { get; set; }

    public int? CaloriesBurned { get; set; }

    public DateTime? LoggedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WorkoutLogExercise> WorkoutLogExercises { get; set; } = new List<WorkoutLogExercise>();

    public virtual WorkoutTemplate? WorkoutTemplate { get; set; }
}
