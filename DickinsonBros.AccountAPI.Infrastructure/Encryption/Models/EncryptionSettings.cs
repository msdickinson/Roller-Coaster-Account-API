using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Infrastructure.Encryption.Models
{
    [ExcludeFromCodeCoverage]
    public class EncryptionSettings
    {
        public string ThumbPrint { get; set; }
        public string StoreLocation { get; set; }
    } 
}
