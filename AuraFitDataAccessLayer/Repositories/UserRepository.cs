using Microsoft.EntityFrameworkCore;
using AuraFitDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraFitDataAccessLayer.Repositories.Interfaces;

namespace AuraFitDataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuraFitDbContext _context;
        public UserRepository(AuraFitDbContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task AddUserAsync(User user)
        {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}

