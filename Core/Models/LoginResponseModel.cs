using Core.Persistence.DAOs;
using Server.Utilities.Security.JWT;

namespace Core.Models
{
    public class LoginResponseModel
    {
        public AccessToken AccessToken { get; set; }
        public UserDAO User { get; set; }
    }
}