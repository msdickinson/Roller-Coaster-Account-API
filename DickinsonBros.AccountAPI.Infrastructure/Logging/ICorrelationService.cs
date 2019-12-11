namespace DickinsonBros.AccountAPI.Infrastructure.Logging
{
    public interface ICorrelationService
    {
        string CorrelationId { get; set; }
    }
}