namespace DickinsonBros.AccountAPI.Infrastructure.Encryption
{
    public interface IEncryptionService
    {
        string Decrypt(string encryptedString);
        string Encrypt(string rawString);
    }
}