using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuraFitDataAccessLayer.Models
{
    public class WeeklyWorkoutPlan
    {
        [Key]
        public int PlanId { get; set; }

        public int UserId { get; set; }

        public string PlanName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<DailyWorkout> DailyWorkouts { get; set; }
    }
}
