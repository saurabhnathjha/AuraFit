using AuraFitDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AuraFitDataAccessLayer.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(int userId);
        Task<bool> UpsertAsync(UserProfile profile);
        Task<bool> ShouldPromptWeightUpdateAsync(int userId);
        Task<int> SendDataToWeightLogs(WeightLog weightLog);
    }
}
