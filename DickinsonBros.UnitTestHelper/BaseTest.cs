using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DickinsonBros.UnitTestHelper
{
    public class BaseTest
    {
        public void RunDependencyInjectedTest(Action<IServiceProvider> callback, params Action<IServiceCollection>[] serviceCollectionConfigurators)
        {
            RunDependencyInjectedTestAsync(
                async serviceProvider =>
                {
                    callback(serviceProvider);
                    await Task.CompletedTask;
                },
                serviceCollectionConfigurators
            ).GetAwaiter().GetResult();
        }
        public async Task RunDependencyInjectedTestAsync(Func<IServiceProvider, Task> callback, params Action<IServiceCollection>[] serviceCollectionConfigurators)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();

            var IConfigurationRootMock = new Mock<IConfiguration>();
            serviceCollection.AddSingleton(IConfigurationRootMock.Object);

            serviceCollectionConfigurators.ForEach(serviceCollectionConfigurator => serviceCollectionConfigurator(serviceCollection));

            using(var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                await callback(serviceProvider);
            }
        }

        public T SetupController<T>(IServiceProvider serviceProvider, IDictionary<string, string> headers) where T : ControllerBase
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            var httpResponseMock = new Mock<HttpResponse>();

            var principal = new ClaimsPrincipal();

            var responseHeaderDictionary = new HeaderDictionary();

            httpContextMock
              .SetupGet(httpContext => httpContext.User)
              .Returns(() => principal);

            httpContextMock
                .SetupGet(httpContext => httpContext.Request)
                .Returns(() => httpRequestMock.Object);

            httpContextMock
                .SetupGet(httpContext => httpContext.Response)
                .Returns(() => httpResponseMock.Object);

            httpRequestMock
                .SetupGet(httpRequest => httpRequest.HttpContext)
                .Returns(() => httpContextMock.Object);

            httpResponseMock
               .SetupGet(httpResponse => httpResponse.HttpContext)
               .Returns(() => httpContextMock.Object);

            httpRequestMock
                .SetupGet(httpRequest => httpRequest.Headers)
                .Returns(() =>
                {
                    var headerDictionary = new HeaderDictionary();

                    foreach (var header in headers)
                    {
                        headerDictionary.Add(header.Key, header.Value);
                    }
                    return headerDictionary;
                });

            var uut = serviceProvider.GetRequiredService<T>();

            uut.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            return uut;
        }
    }
}
