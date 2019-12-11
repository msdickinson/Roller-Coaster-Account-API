using DickinsonBros.AccountAPI.Contracts;
using System.ComponentModel.DataAnnotations;

namespace DickinsonBros.AccountAPI.View.Models
{
    public class UpdateEmailPreferenceWithTokenRequest
    {
        [Required]
        [MinLength(1)]
        public string? Token { get; set; }

        [Required]
        [EnumDataType(typeof(EmailPreference))]
        public EmailPreference EmailPreference { get; set; }
    }
}
