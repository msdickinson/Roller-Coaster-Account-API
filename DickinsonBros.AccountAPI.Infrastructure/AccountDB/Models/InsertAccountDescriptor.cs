using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class InsertAccountResult
    {
        public int AccountId { get; internal set; }
        public bool DuplicateUser { get; internal set; }
    }
}
