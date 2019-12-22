using DickinsonBros.AccountAPI.View.Controllers;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DickinsonBros.AccountAPI.View.Tests.Controllers
{
    [TestClass]
    public class StatusControllerTests : BaseTest
    {
        [TestMethod]
        public async Task HealthCheckAsync_Runs_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    var uut = serviceProvider.GetControllerInstance<StatusController>();

                    //Act
                    var observed = (await uut.HealthCheckAsync()) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<StatusController>();
            return serviceCollection;
        }
    }
}
