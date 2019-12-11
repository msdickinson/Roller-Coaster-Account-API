using System;

namespace DickinsonBros.AccountAPI.Infrastructure.DateTime
{
    public interface IDateTimeService
    {
        System.DateTime GetDateTimeUTC();
    }
}