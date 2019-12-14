using DickinsonBros.AccountAPI.Infrastructure.JWT.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DickinsonBros.AccountAPI.Infrastructure.JWT
{
    public class JWTService : IJWTService
    {
        internal readonly string _secert;
        internal readonly string _refreshSecert;
        public JWTService(IOptions<JWTSettings> encryptionSettings)
        {
            _secert = encryptionSettings.Value.Secret;
            _refreshSecert = encryptionSettings.Value.RefreshSecret;
        }

        public string GenerateJWT(IEnumerable<Claim> claims, System.DateTime expiresDateTime, bool isRefresh = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(isRefresh ? _refreshSecert : _secert);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateJWT(string nameIdentifier, System.DateTime expiresDateTime, bool isRefresh = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(isRefresh ? _refreshSecert : _secert);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, nameIdentifier)
                }),
                Expires =  expiresDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipal(string token, bool isRefresh = false, bool vaildateLifetime = true)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Encoding.ASCII.GetBytes(isRefresh ? _refreshSecert : _secert);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = vaildateLifetime
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch 
            {
                return null;
            }
        }
    }

}
