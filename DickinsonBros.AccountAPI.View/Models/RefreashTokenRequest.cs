using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.View.Models
{
    [ExcludeFromCodeCoverage]
    public class RefreshTokenRequest
    {
        [Required]
        [MinLength(1)]
        public string? access_token { get; set; }

        [Required]
        [MinLength(1)]
        public string? refresh_token { get; set; }
    }
}
