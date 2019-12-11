using System;
using System.Diagnostics.CodeAnalysis;
namespace DickinsonBros.AccountAPI.Contracts
{
    [ExcludeFromCodeCoverage]
    public class Account
    {
        public int AccountId { get; set; }
        public bool Locked { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public EmailPreference EmailPreference { get; set; }
        public Guid EmailPreferenceToken { get; set; }
        public bool EmailActivated { get; set; }
        public Guid ActivateEmailToken { get; set; }
    }
}
