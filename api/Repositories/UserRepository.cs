using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public interface IUserRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task CreateAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
        Task RevokeDescendantsAsync(RefreshToken token, string ipAddress, string reason);
    }
    public class UserRepository: IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task CreateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeDescendantsAsync(RefreshToken token, string ipAddress, string reason)
        {
            if (!string.IsNullOrEmpty(token.ReplacedByToken))
            {
                var childToken = await GetByTokenAsync(token.ReplacedByToken);
                if (childToken != null && childToken.IsActive)
                {
                    childToken.Revoked = DateTime.UtcNow;
                    childToken.RevokedByIp = ipAddress;
                    childToken.ReasonRevoked = reason;
                    await UpdateAsync(childToken);
                    await RevokeDescendantsAsync(childToken, ipAddress, reason);
                }
            }
        }
    }
}
