using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Infrastructure.Encryption.Models
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public string Secret { get; set; }
        public string RefreshSecret { get; set; }
    }
}
