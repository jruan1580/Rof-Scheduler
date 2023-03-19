using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RofShared.Services
{
    public interface ITokenHandler
    {
        string GenerateTokenForRole(string role, int minTokenIsValid = 30);
    }

    public class TokenHandler : ITokenHandler
    {
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public TokenHandler(IConfiguration configuration)
        {
            _jwtKey = configuration.GetSection("Jwt:Key").Value;
            _jwtIssuer = configuration.GetSection("Jwt:Issuer").Value;
            _jwtAudience = configuration.GetSection("Jwt:Audience").Value;
        }

        public string GenerateTokenForRole(string role, int minTokenIsValid = 30)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityToken(
                _jwtIssuer,
                _jwtAudience,
                claims: new List<Claim>()
                {
                    new Claim(ClaimTypes.Role, role)
                },
                expires: DateTime.Now.AddMinutes(minTokenIsValid),
                signingCredentials: credentials
            );

            return tokenHandler.WriteToken(token);
        }
    }
}
