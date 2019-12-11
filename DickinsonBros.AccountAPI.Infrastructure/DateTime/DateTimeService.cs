using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.AccountAPI.Infrastructure.DateTime
{
    [ExcludeFromCodeCoverage]
    public class DateTimeService : IDateTimeService
    {
        public System.DateTime GetDateTimeUTC()
        {
            return System.DateTime.UtcNow;
        }
    }
}
