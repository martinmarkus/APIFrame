using APIFrame.Core.Configuration;
using APIFrame.Core.Constants;
using APIFrame.Web.Authentication.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace APIFrame.Web.Authentication
{
    public class JwtGeneratorService : IJwtGeneratorService
    {
        private readonly BaseOptions _baseOptions;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public JwtGeneratorService(
            IOptions<BaseOptions> options,
            JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _baseOptions = options.Value;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }

        public string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _baseOptions.AuthIssuerName,
                Expires = DateTime.UtcNow.AddMinutes(Math.Abs(_baseOptions.AuthSessionMinutes)),
                SigningCredentials = new SigningCredentials(
                       new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_baseOptions.AuthSecretKey)),
                       SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return _jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
