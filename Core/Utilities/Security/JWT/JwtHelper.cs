using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Persistence.DAOs;
using Core.Utilities.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Server.Utilities.Security.Encryption;
using Server.Utilities.Security.JWT;

namespace Core.Utilities.Security.JWT
{
    public class JwtHelper : ITokenHelper
{
    public IConfiguration Configuration { get; }
    private readonly TokenOptions _tokenOptions;

    public JwtHelper(IConfiguration configuration)
    {
        Configuration = configuration;
        _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
    }

    public AccessToken CreateToken(UserDAO userDao, IEnumerable<string> roles, bool rememberMe)
    {
        DateTime expiration;

        if (rememberMe)
            expiration = DateTime.Now.AddYears(_tokenOptions.AccessTokenExpiration);
        else
            expiration = DateTime.Now.AddHours(_tokenOptions.AccessTokenExpiration);

        var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
        
        var jwt = CreateJwtSecurityToken(userDao, signingCredentials, roles, expiration);

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var token = jwtSecurityTokenHandler.WriteToken(jwt);

        return new AccessToken
        {
            Token = token,
            Expiration = expiration
        };
    }

    private JwtSecurityToken CreateJwtSecurityToken(UserDAO userDao, SigningCredentials signingCredentials, IEnumerable<string> roles, DateTime expiration)
    {
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            expires: expiration,
            notBefore: DateTime.Now,
            claims: SetClaims(userDao, roles),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }

    private IEnumerable<Claim> SetClaims(UserDAO userDao, IEnumerable<string> roles)
    {
        var claims = new List<Claim>();
        
        claims.AddUserId(userDao.Id);
        claims.AddEmail(userDao.Email);
        claims.AddNickname(userDao.NickName);
        claims.AddRoles(roles);

        return claims;
    }
}
}