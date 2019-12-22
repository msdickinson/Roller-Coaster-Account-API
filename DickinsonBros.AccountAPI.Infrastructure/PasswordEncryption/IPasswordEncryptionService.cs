using DickinsonBros.AccountAPI.Infrastructure.PasswordEncryption.Models;

namespace DickinsonBros.AccountAPI.Infrastructure.PasswordEncryption
{
    public interface IPasswordEncryptionService
    {
        EncryptResult Encrypt(string password, string salt = null);
    }
}