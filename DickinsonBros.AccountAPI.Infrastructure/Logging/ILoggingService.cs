using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DickinsonBros.AccountAPI.Infrastructure.Logging
{
    public interface ILoggingService<out T>
    {
        void LogDebugRedacted(string message, IDictionary<string, object> properties = null);
        void LogInformationRedacted(string message, IDictionary<string, object> properties = null);
        void LogWarningRedacted(string messagee, IDictionary<string, object> properties = null);
        void LogErrorRedacted(string message, Exception exception, IDictionary<string, object> properties = null);
    }
}