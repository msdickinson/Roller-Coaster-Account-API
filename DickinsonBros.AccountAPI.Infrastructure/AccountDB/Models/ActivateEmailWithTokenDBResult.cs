using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class ActivateEmailWithTokenDBResult
    {
        public bool VaildToken { get; set; }
        public bool EmailWasAlreadyActivated { get; set; }
    }
}
