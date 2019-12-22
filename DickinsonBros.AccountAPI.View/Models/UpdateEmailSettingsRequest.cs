using DickinsonBros.AccountAPI.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.View.Models
{
    [ExcludeFromCodeCoverage]
    public class UpdateEmailPreferenceRequest
    {
        [Required]
        [EnumDataType(typeof(EmailPreference))]
        public EmailPreference EmailPreference { get; set; }
    }
}
