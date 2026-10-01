using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class WeightLog
{
    public int LogId { get; set; }

    public int UserId { get; set; }

    public double WeightKg { get; set; }

    public DateTime? LoggedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
