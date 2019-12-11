using DickinsonBros.AccountAPI.Infrastructure.Encryption.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DickinsonBros.AccountAPI.Infrastructure.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        internal readonly string _secert;
        internal readonly string _refreshSecert;
        public EncryptionService(IOptions<AppSettings> appSettings)
        {
            _secert = appSettings.Value.Secret;
            _refreshSecert = appSettings.Value.RefreshSecret;
        }

        public EncryptResult Encrypt(string password, string salt = null)
        {
            byte[] saltByteArray;
            if (string.IsNullOrWhiteSpace(salt))
            {
                saltByteArray = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltByteArray);
                }
            }
            else
            {
                saltByteArray = Convert.FromBase64String(salt);
            }

            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltByteArray,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return new EncryptResult
            {
                Hash = hash,
                Salt = Convert.ToBase64String(saltByteArray)
            };
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
