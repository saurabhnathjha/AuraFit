using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuraFitDataAccessLayer.Models;

public partial class FoodItem
{
    public int FoodItemId { get; set; }

    public int MealId { get; set; }

    public string Name { get; set; } = null!;

    public string? Quantity { get; set; }

    public int? Calories { get; set; }

    public string? Source { get; set; }

    [JsonIgnore]
    public virtual Meal Meal { get; set; } = null!;
}
