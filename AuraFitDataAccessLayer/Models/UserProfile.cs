using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class UserProfile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public int? HeightCm { get; set; }

    public double? WeightKg { get; set; }

    public string? ActivityLevel { get; set; }

    public double? Bmi { get; set; }

    public double? Bmr { get; set; }

    public double? Tdee { get; set; }

    public virtual User User { get; set; } = null!;
}
