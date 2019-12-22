namespace DickinsonBros.AccountAPI.Infrastructure.PasswordEncryption.Models
{
    public class EncryptResult
    {
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
