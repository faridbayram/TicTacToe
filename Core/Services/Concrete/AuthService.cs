using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Persistence.Abstract;
using Core.Persistence.DAOs;
using Core.Services.Abstract;
using Core.Utilities.Results;
using Server.Utilities.Security.Hashing;
using Server.Utilities.Security.JWT;

namespace Core.Services.Concrete
{
    public class AuthService : IAuthService
{
    private readonly IUserPersistence _userPersistence;
    private readonly ITokenHelper _tokenHelper;
    private readonly IBonusService _bonusService;

    public AuthService(IUserPersistence userPersistence, ITokenHelper tokenHelper, IBonusService bonusService)
    {
        _userPersistence = userPersistence;
        _tokenHelper = tokenHelper;
        _bonusService = bonusService;
    }

    public async Task<IDataResult<LoginResponseModel>> LoginAsync(LoginRequestModel requestModel)
    {
        try
        {
            var userDao = await _userPersistence.GetUserAsync(requestModel.Email);
            
            var loginIsSuccessful = HashingHelper.VerifyPasswordHash(
                requestModel.Password,
                Convert.FromBase64String(userDao.PasswordSalt),
                Convert.FromBase64String(userDao.PasswordHash));

            if (!loginIsSuccessful)
                return new ErrorDataResult<LoginResponseModel>("invalid credentials");
            
            await _bonusService.GiveBonusAsync(userDao);

            var token = CreateAccessToken(userDao, requestModel.RememberMe);
            return new SuccessDataResult<LoginResponseModel>(new LoginResponseModel
            {
                AccessToken = token,
                User = userDao
            });

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ErrorDataResult<LoginResponseModel>(e.Message);
        }
        
        
    }

    public async Task<IDataResult<RegisterResponseModel>> RegisterAsync(RegisterRequestModel requestModel)
    {
        try
        {
            HashingHelper.CreatePasswordHash(requestModel.Password, out var passwordSalt, out var passwordHash);

            var userDao = new UserDAO()
            {
                Email = requestModel.Email,
                NickName = requestModel.NickName,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt)
            };

            await _userPersistence.AddUserAsync(userDao);

            await _bonusService.GiveBonusAsync(userDao);
            
            var token = CreateAccessToken(userDao, false);

            return new SuccessDataResult<RegisterResponseModel>(new RegisterResponseModel
            {
                AccessToken = token,
                User = userDao
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ErrorDataResult<RegisterResponseModel>(e.Message);
        }
    }

    private AccessToken CreateAccessToken(UserDAO userDao, bool rememberMe)
    {
        IEnumerable<string> roles = new[] { "player" };
        var accessToken = _tokenHelper.CreateToken(userDao, roles, rememberMe);
        return accessToken;
    }
}
}