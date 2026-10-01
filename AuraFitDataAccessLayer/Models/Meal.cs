using System;
using System.Collections.Generic;

namespace AuraFitDataAccessLayer.Models;

public partial class Meal
{
    public int MealId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? LoggedAt { get; set; }

    public string? Source { get; set; }

    public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();

    public virtual User User { get; set; } = null!;

    public int? TotalCalories { get; set; }  // new property

}
