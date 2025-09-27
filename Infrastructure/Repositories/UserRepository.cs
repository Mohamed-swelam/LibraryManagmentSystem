using Core.DTOs;
using Core.Entites;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ApplicationUser?> GetUserById(string userId)
        {
            return await context.Users.Include(e => e.RefreshTokens)
                .FirstOrDefaultAsync(e => e.Id == userId) ?? null;
        }

        public async Task<ApplicationUser?> GetUserByRefreshtoken(string refreshtoken)
        {
            return await context.Users.Include(e=>e.RefreshTokens)
                .FirstOrDefaultAsync(e=>e.RefreshTokens!=null 
                && e.RefreshTokens.Any(e=>e.Token== refreshtoken)) ?? null;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
       
    }
}
