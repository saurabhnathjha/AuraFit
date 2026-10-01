using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class ExerciseCatalog
{
    public int ExerciseCatalogId { get; set; }

    public int? WgerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public string? Description { get; set; }

    public string? Equipment { get; set; }

    public string? ImageUrl { get; set; }

    public double? METValue { get; set; }
    public virtual ICollection<TemplateExercise> TemplateExercises { get; set; } = new List<TemplateExercise>();

    public virtual ICollection<WorkoutLogExercise> WorkoutLogExercises { get; set; } = new List<WorkoutLogExercise>();
}
