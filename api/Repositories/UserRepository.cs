using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public interface IUserRepository
    {
        
       
    }
    public class UserRepository: IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

    }
}
