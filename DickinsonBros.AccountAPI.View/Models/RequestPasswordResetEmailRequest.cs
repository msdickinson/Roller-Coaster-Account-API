using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.View.Models
{
    [ExcludeFromCodeCoverage]
    public class RequestPasswordResetEmailRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
