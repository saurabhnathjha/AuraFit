using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuraFitDataAccessLayer.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AuraFitDbContext _context;

        public UserProfileRepository(AuraFitDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserIdAsync(int userId)
        {
            return await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> UpsertAsync(UserProfile profile)
        {
            try
            {
                var existingProfile = await GetByUserIdAsync(profile.UserId);

                if (existingProfile == null)
                {
                    await _context.UserProfiles.AddAsync(profile);
                }
                else
                {
                    existingProfile.Age = profile.Age;
                    existingProfile.Gender = profile.Gender;
                    existingProfile.HeightCm = profile.HeightCm;
                    existingProfile.WeightKg = profile.WeightKg;
                    existingProfile.ActivityLevel = profile.ActivityLevel;
                    existingProfile.Bmi = profile.Bmi;
                    existingProfile.Bmr = profile.Bmr;
                    existingProfile.Tdee = profile.Tdee;

                    _context.UserProfiles.Update(existingProfile);
                }

                var saved = await _context.SaveChangesAsync();
                return saved > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> SendDataToWeightLogs(WeightLog weightLog)
        {
            int retVal = 0;
            try
            {
                await _context.WeightLogs.AddAsync(weightLog);
                await _context.SaveChangesAsync();
                retVal = 1;
            }
            catch (Exception)
            {
                retVal = -1;
            }
            return retVal;
        }


        public async Task<bool> ShouldPromptWeightUpdateAsync(int userId)
        {
            try
            {
                var lastWeightLog = await _context.WeightLogs
                    .Where(log => log.UserId == userId)
                    .OrderByDescending(log => log.LoggedAt)
                    .FirstOrDefaultAsync();

                if (lastWeightLog == null)
                {
                    return true; // No log exists, should prompt
                }
                else
                {
                    return (DateTime.Now - lastWeightLog.LoggedAt.Value).TotalMilliseconds >= 1000*60*5; //5 mins
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}
