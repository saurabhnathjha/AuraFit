using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class WorkoutDay
{
    public int WorkoutDayId { get; set; }

    public int WorkoutTemplateId { get; set; }

    public int DayOrder { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<TemplateExercise> TemplateExercises { get; set; } = new List<TemplateExercise>();

    public virtual WorkoutTemplate WorkoutTemplate { get; set; } = null!;
}
