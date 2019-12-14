using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Infrastructure.JWT.Models
{
    [ExcludeFromCodeCoverage]
    public class JWTSettings
    {
        public string Secret { get; set; }
        public string RefreshSecret { get; set; }
    } 
}
