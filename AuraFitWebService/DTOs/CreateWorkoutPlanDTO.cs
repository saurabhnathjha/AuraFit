using System.Collections.Generic;

namespace AuraFitWebServices.DTOs
{
    public class CreateWorkoutPlanDTO
    {
        public int UserId { get; set; }
        public string PlanName { get; set; }

        public List<DailyWorkoutDTO> DailyWorkouts { get; set; }
    }
}
