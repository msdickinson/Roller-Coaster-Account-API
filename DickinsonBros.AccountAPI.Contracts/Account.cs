using System;
using System.Diagnostics.CodeAnalysis;
namespace DickinsonBros.AccountAPI.Contracts
{
    [ExcludeFromCodeCoverage]
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
