using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class UserGoal
{
    public int GoalId { get; set; }

    public int UserId { get; set; }

    public string? GoalType { get; set; }

    public double? WeeklyTargetKg { get; set; }

    public int? TargetCalories { get; set; }

    public bool? AutoSuggested { get; set; }

    public DateOnly? GoalStartDate { get; set; }

    public virtual User User { get; set; } = null!;
}
