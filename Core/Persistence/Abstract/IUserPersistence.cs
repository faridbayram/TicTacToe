using System.Threading.Tasks;
using Core.Persistence.DAOs;

namespace Core.Persistence.Abstract
{
    public interface IUserPersistence
    {
        Task<UserDAO> GetUserAsync(string email);
        Task AddUserAsync(UserDAO userDao);
        Task UpdateUserAsync(UserDAO userDao);
    }
}