namespace DickinsonBros.AccountAPI.Infrastructure.Email.Models
{
    public class SendActivateEmailRequest
    {
        public string Email { get; set; }
        public string ActivateToken { get; set; }
        public string UpdateEmailSettingsToken { get; set; }
        public object Username { get; set; }
    }
}
