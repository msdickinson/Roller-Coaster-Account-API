namespace DickinsonBros.AccountAPI.Infrastructure.Redactor
{
    public interface IRedactor
    {
        string Redact(object value);
        string Redact(string json);
    }
}