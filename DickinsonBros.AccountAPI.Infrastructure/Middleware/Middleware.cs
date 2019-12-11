using DickinsonBros.AccountAPI.Infrastructure.DateTime;
using DickinsonBros.AccountAPI.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.AccountAPI.Infrastructure.Middleware
{
    public class Middleware : IMiddleware
    {
        internal const string CORRELATION_ID = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        internal readonly IGuidService _guidService;
        internal readonly ILoggingService<Middleware> _loggingService;

        internal readonly ICorrelationService _correlationService;
        public Middleware(
            RequestDelegate next,
            IGuidService guidService,
            ICorrelationService correlationService,
            ILoggingService<Middleware> loggingService
        )
        {
            _next = next;
            _guidService = guidService;
            _loggingService = loggingService;
            _correlationService = correlationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _correlationService.CorrelationId = EnsureCorrelationId(context);

            var requestBody = await FormatRequestAsync(context.Request);
            var originalBodyStream = context.Response.Body;

            _loggingService.LogInformationRedacted
            (
                $"+ {context.Request.Path}",
                new Dictionary<string, object>
                {
                    { "Path", context.Request.Path.Value },
                    { "Prams", context.Request.Query.ToDictionary() },
                    { "Body", requestBody }
                }
            );
                
            _loggingService.LogInformationRedacted
            (
                $"+ {context.Request.Path}",
                new Dictionary<string, object>
                {
                    { "Path", context.Request.Path.Value },
                    { "Prams", context.Request.Query.ToDictionary() },
                    { "Body", requestBody },
                    { "BodyDecrypted", requestBody },
                    { "DecryptSuccessful", true }
                }
            );

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                try
                {
                    await _next(context);

                    context.Response.Headers.TryAdd
                    (
                        CORRELATION_ID,
                       _correlationService.CorrelationId
                    );

                    var responseBodyString = await FormatResponseAsync(context.Response);
                    await responseBody.CopyToAsync(originalBodyStream);
                    _loggingService.LogInformationRedacted
                    (
                        $"Response {context.Request.Path}",
                        new Dictionary<string, object>
                        {
                            { "Body", responseBodyString },
                            { "Status Code", context.Response.StatusCode }
                        }
                    );
                }
                catch (Exception exception)
                {
                    context.Response.StatusCode = 500;
                    context.Response.Headers.TryAdd
                    (
                        CORRELATION_ID,
                       _correlationService.CorrelationId
                    );
                   
                    _loggingService.LogErrorRedacted
                    (
                        $"Unhandled exception {context.Request.Query}",
                        exception,
                        new Dictionary<string, object>
                        {
                            { "Status Code", context.Response.StatusCode }
                        }
                    );
                }
                finally
                {
                    _loggingService.LogInformationRedacted
                    (
                        $"- {context.Request.Query}"
                    );
                }
            }
        }

        internal string EnsureCorrelationId(HttpContext context)
        {
            if (!context.Request.Headers.Any(e => e.Key == CORRELATION_ID))
            {
                return _guidService.NewGuid().ToString();
            }

            return context.Request.Headers.First(e => e.Key == CORRELATION_ID).Value;
        }

        internal async Task<string> FormatRequestAsync(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            var body = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;

            return body;
        }

        internal async Task<string> FormatResponseAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            string body = await new StreamReader(response.Body).ReadToEndAsync();

            response.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }
    }
}
