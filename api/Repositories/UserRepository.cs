
using api.Data;

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
