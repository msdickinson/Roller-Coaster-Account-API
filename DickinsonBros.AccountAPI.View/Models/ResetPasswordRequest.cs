﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.View.Models
{
    [ExcludeFromCodeCoverage]
    public class ResetPasswordRequest
    {
        [Required]
        [MinLength(1)]
        public string? Token { get; set; }

        [Required]
        [MinLength(8)]
        public string? NewPassword { get; set; }
    }
}
