using System.Collections.Generic;
using Core.Persistence.DAOs;

namespace Server.Utilities.Security.JWT
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(UserDAO user, IEnumerable<string> operationClaims, bool rememberMe);
    }
}