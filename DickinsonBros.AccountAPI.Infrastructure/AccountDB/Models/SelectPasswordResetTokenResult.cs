using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class SelectAccountIdFromPasswordResetTokenResult
    {
        public bool TokenFound { get; internal set; }
        public bool TokenExpired { get; internal set; }
        public int AccountId { get; internal set; }
    }
}
