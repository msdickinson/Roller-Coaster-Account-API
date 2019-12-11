using System.ComponentModel.DataAnnotations;

namespace DickinsonBros.AccountAPI.View.Models
{
    public class ActivateEmailRequest
    {
        [Required]
        [MinLength(1)]
        public string? Token { get; set; }
    }
}
