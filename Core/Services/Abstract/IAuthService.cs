using System.Threading.Tasks;
using Core.Models;
using Core.Utilities.Results;
using Server.Utilities.Security.JWT;


namespace Core.Services.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<LoginResponseModel>> LoginAsync(LoginRequestModel requestModel);
        Task<IDataResult<RegisterResponseModel>> RegisterAsync(RegisterRequestModel requestModel);
    }
}

