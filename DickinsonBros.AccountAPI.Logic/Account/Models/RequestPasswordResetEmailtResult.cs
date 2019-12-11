namespace DickinsonBros.AccountAPI.Logic.Account.Models
{
    public enum RequestPasswordResetEmailResult
    {
        Successful,
        EmailNotFound,
        EmailNotActivated,
        NoEmailSentDueToEmailPreference
    }
}
