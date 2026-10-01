using AuraFitDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraFitDataAccessLayer.Repositories.Interfaces
{
    public interface IUserGoalRepository
    {
         Task<UserGoal?> GetByUserIdAsync(int userId);
        Task<bool> UpsertAsync(UserGoal goal);

    }
}
