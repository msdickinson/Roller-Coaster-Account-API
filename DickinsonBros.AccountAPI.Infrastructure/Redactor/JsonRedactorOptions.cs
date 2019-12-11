namespace DickinsonBros.AccountAPI.Infrastructure.Redactor
{
    public class JsonRedactorOptions
    {
        public string[] PropertiesToRedact { get; set; }
        public string[] ValuesToRedact { get; set; }
    }
}
