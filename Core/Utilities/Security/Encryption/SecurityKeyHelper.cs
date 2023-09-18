using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Server.Utilities.Security.Encryption
{
    public class SecurityKeyHelper
    {
        public static SecurityKey CreateSecurityKey(string key)
        {
            var keyAsByteArr = Encoding.UTF8.GetBytes(key);
            return new SymmetricSecurityKey(keyAsByteArr);
        }
    }
}