using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuraFitDataAccessLayer.Models
{
    public class DailyWorkout
    {
        [Key]
        public int DailyWorkoutId { get; set; }

        public int PlanId { get; set; }

        public string DayOfWeek { get; set; } // E.g., "Monday", "Tuesday"

        public int ExerciseId { get; set; }

        [ForeignKey("PlanId")]
        public WeeklyWorkoutPlan WeeklyWorkoutPlan { get; set; }
        
    }

}
