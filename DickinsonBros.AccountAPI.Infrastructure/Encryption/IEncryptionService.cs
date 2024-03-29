﻿using DickinsonBros.AccountAPI.Infrastructure.Encryption.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace DickinsonBros.AccountAPI.Infrastructure.Encryption
{
    public interface IEncryptionService
    {
        EncryptResult Encrypt(string password, string salt = null);
        string GenerateJWT(string nameIdentifier, System.DateTime expiresDateTime, bool isRefresh = false);
        string GenerateJWT(IEnumerable<Claim> claims, System.DateTime expiresDateTime, bool isRefresh = false);
        ClaimsPrincipal GetPrincipal(string token, bool isRefresh = false, bool vaildateLifetime = true);
    }
}