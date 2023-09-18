using System.Threading.Tasks;
using Core.Persistence.DAOs;

namespace Core.Services.Abstract
{
    public interface IBonusService
    {
        Task GiveBonusAsync(UserDAO user);
    }
}