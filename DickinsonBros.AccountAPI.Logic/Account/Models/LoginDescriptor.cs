using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Logic.Account.Models
{
    [ExcludeFromCodeCoverage]
    public class LoginDescriptor
    {
        public LoginResult Result { get; internal set; }
        public int? AccountId { get; internal set; }
    }
}
