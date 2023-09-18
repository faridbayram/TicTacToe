using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Core.Utilities.Resources.Custom;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Core.Utilities.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims, string email)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email,email));
        }

        public static void AddNickname(this ICollection<Claim> claims, string nickname)
        {
            claims.Add(new Claim(CustomClaims.Nickname, nickname));
        }

        public static void AddRoles(this ICollection<Claim> claims, IEnumerable<string> roles)
        {
            roles.ToList().ForEach(role=>claims.Add(new Claim(ClaimTypes.Role, role)));
        }

        public static void AddUserId(this ICollection<Claim> claims, string userId)
        {
            claims.Add(new Claim(CustomClaims.UserId, userId.ToString()));
        }
    }
}