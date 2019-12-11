using DickinsonBros.AccountAPI.Contracts;
using System.ComponentModel.DataAnnotations;

namespace DickinsonBros.AccountAPI.View.Models
{
    public class UpdateEmailPreferenceRequest
    {
        [Required]
        [EnumDataType(typeof(EmailPreference))]
        public EmailPreference EmailPreference { get; set; }
    }
}
