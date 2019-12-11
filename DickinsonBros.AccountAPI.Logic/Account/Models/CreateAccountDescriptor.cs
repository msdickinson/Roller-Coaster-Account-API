using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Logic.Account.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateAccountDescriptor
    {
        public CreateAccountResult Result { get; internal set; }
        public int AccountId { get; internal set; }
    }
}
