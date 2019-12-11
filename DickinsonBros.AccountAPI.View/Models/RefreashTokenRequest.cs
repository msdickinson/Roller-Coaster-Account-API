using System.ComponentModel.DataAnnotations;

namespace DickinsonBros.AccountAPI.View.Models
{
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
