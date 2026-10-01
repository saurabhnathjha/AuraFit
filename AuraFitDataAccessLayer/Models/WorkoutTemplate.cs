using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class WorkoutTemplate
{
    public int WorkoutTemplateId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? WgerId { get; set; }

    public int CreatedBy { get; set; }

    public bool IsPublic { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();

    public virtual ICollection<WorkoutLog> WorkoutLogs { get; set; } = new List<WorkoutLog>();
}
