using DickinsonBros.AccountAPI.Infrastructure.Redactor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DickinsonBros.AccountAPI.Logic.Tests
{
    [TestClass]
    public class AccountManagerTests
    {
        [TestMethod]
        public void Tests()
        {
            var r = new Redactor(null);
        }
    }
}
//namespace DickinsonBros.AccountAPI.Logic.Tests
//{
//    [TestClass]
//    public class AccountManagerTests : BaseTest
//    {
//        #region LoginAsync

//        [TestMethod]
//        public async Task LoginAsync_Runs_CallsSelectAccountByUserNameAsync()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var username = "User1000";
//                    var password = "Password!";
//                    var hash = "PasswordHashed";
//                    var salt = "";

//                    var expected = new LoginDescriptor
//                    {
//                        AccountId = null,
//                        Result = LoginResult.Successful
//                    };


//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByUserNameAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new Contracts.Account()
//                            {
//                                PasswordHash = hash
//                            }
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceStub
//                    .Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<strin