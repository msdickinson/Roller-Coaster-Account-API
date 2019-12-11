//using DickinsonBros.AccountAPI.Abstractions;
//using DickinsonBros.AccountAPI.Infrastructure.DateTime;
//using DickinsonBros.AccountAPI.Infrastructure.Logging;
//using DickinsonBros.AccountAPI.Logic.Account;
//using DickinsonBros.AccountAPI.Logic.Account.Models;
//using DickinsonBros.AccountAPI.View.Controllers;
//using DickinsonBros.AccountAPI.View.Models;
//using DickinsonBros.UnitTestHelper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//namespace DickinsonBros.AccountAPI.View.Tests.Controllers
//{
//    [TestClass]
//    public class AccountControllerTests : BaseTest
//    {
//        #region CreateAsync

//        [TestMethod]
//        public async Task CreateAsync_Runs_AccountManagerCreateAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var createAccountRequest = new CreateAccountRequest
//                    {
//                        Username = "User1000",
//                        Password = "Password!"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.CreateAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new CreateAccountDescriptor
//                            {
//                                Result = CreateAccountResult.Successful,
//                                AccountId = 1000
//                            }
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = (await uut.CreateAsync(createAccountRequest)) as StatusCodeResult;

//                    //Assert
//                    accountManagerMock
//                        .Verify(
//                            accountManager => accountManager.CreateAsync
//                            (
//                                createAccountRequest.Username,
//                                createAccountRequest.Password,
//                                createAccountRequest.Email,
//                                expectedCollerationId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task CreateAsync_IsSuccessful_ReturnsAccountId()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var createAccountRequest = new CreateAccountRequest
//                    {
//                        Username = "User1000",
//                        Password = "Password!"
//                    };

//                    var expectedAccountId = 1000; 
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", "CollerationId" }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.CreateAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new CreateAccountDescriptor
//                            {
//                                Result = CreateAccountResult.Successful,
//                                AccountId = expectedAccountId
//                            }
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = (await uut.CreateAsync(createAccountRequest)) as ObjectResult;

//                    //Assert
//                    Assert.AreEqual(200, observed.StatusCode);
//                    Assert.AreEqual(expectedAccountId, observed.Value);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }


//        [TestMethod]
//        public async Task CreateAsync_DuplicateUser_ReturnsConflict409()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var createAccountRequest = new CreateAccountRequest
//                    {
//                        Username = "User1000",
//                        Password = "Password!"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
//                    accountManagerMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.CreateAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new CreateAccountDescriptor
//                            {
//                                Result = CreateAccountResult.DuplicateUser,
//                                AccountId = null
//                            }
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.CreateAsync(createAccountRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(409, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region LoginAsync

//        [TestMethod]
//        public async Task LoginAsync_Runs_AccountManagerLoginAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var loginRequest = new LoginRequest
//                    {
//                        Username = "User1000",
//                        Password = "Password!"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.LoginAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new LoginDescriptor
//                            {
//                                Result = LoginResult.Successful,
//                                AccountId = 1000
//                            }
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.LoginAsync(loginRequest) as ObjectResult;

//                    //Assert
//                    accountManagerMock
//                        .Verify(
//                            accountManager => accountManager.LoginAsync
//                            (
//                                loginRequest.Username,
//                                loginRequest.Password,
//                                expectedCollerationId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task LoginAsync_IsSuccessful_ReturnsAccountId()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var loginRequest = new LoginRequest
//                    {
//                        Username = "User1000",
//                        Password = "Password!"
//                    };

//                    var expectedAccountId = 1000;
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", "CollerationId" }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.LoginAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new LoginDescriptor
//                            {
//                                Result = LoginResult.Successful,
//                                AccountId = expectedAccountId
//                            }
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.LoginAsync(loginRequest) as ObjectResult;

//                    //Assert
//                    Assert.AreEqual(200, observed.StatusCode);
//                    Assert.AreEqual(expectedAccountId, observed.Value);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task LoginAsync_AccountNotFound_Returns404()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var loginRequest = new LoginRequest
//                    {
//                        Username = "User1000",
//                        Password = "Password!"
//                    };

//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", "CollerationId" }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.LoginAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new LoginDescriptor
//                            {
//                                Result = LoginResult.AccountNotFound
//                            }
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.LoginAsync(loginRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(404, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task LoginAsync_InvaildPassword_Returns401()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var loginRequest = new LoginRequest
//                    {
//                        Username = "User1000",
//                        Password = "Password!"
//                    };

//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", "CollerationId" }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.LoginAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new LoginDescriptor
//                            {
//                                Result = LoginResult.InvaildPassword
//                            }
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.LoginAsync(loginRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(401, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region RequestPasswordResetEmailAsync

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_Runs_RequestPasswordResetEmailAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
//                    {
//                        Email = "Email@Email.com"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.RequestPasswordResetEmailAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            RequestPasswordResetEmailResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as ObjectResult;

//                    //Assert
//                    accountManagerMock
//                        .Verify(
//                            accountManager => accountManager.RequestPasswordResetEmailAsync
//                            (
//                                requestPasswordResetEmailRequest.Email,
//                                expectedCollerationId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_Successful_Returns200()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
//                    {
//                        Email = "Email@Email.com"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.RequestPasswordResetEmailAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            RequestPasswordResetEmailResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(200, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_EmailNotFound_Returns404()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
//                    {
//                        Email = "Email@Email.com"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.RequestPasswordResetEmailAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            RequestPasswordResetEmailResult.EmailNotFound
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(404, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region UpdatePasswordAsync

//        [TestMethod]
//        public async Task UpdatePasswordAsync_Runs_RequestPasswordResetEmailAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var updatePasswordRequest = new UpdatePasswordRequest
//                    {
//                        ExistingPassword = "A",
//                        NewPassword = "B",
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.UpdatePasswordAsync
//                            (
//                                It.IsAny<int>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             UpdatePasswordResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    await uut.UpdatePasswordAsync(updatePasswordRequest);

//                    //Assert
//                    accountManagerMock
//                        .Verify(
//                            accountManager => accountManager.UpdatePasswordAsync
//                            (
//                                It.IsAny<int>(),
//                                updatePasswordRequest.ExistingPassword,
//                                updatePasswordRequest.NewPassword,
//                                expectedCollerationId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_Successful_Returns200()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var updatePasswordRequest = new UpdatePasswordRequest
//                    {
//                        ExistingPassword = "A",
//                        NewPassword = "B",
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.UpdatePasswordAsync
//                            (
//                                It.IsAny<int>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             UpdatePasswordResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.UpdatePasswordAsync(updatePasswordRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(200, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_InvaildExistingPassword_ReturnsInvaildExistingPaswordAnd400()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var updatePasswordRequest = new UpdatePasswordRequest
//                    {
//                        ExistingPassword = "A",
//                        NewPassword = "B",
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.UpdatePasswordAsync
//                            (
//                                It.IsAny<int>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             UpdatePasswordResult.InvaildExistingPassword
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.UpdatePasswordAsync(updatePasswordRequest) as ObjectResult;

//                    //Assert
//                    Assert.AreEqual(400, observed.StatusCode);
//                    Assert.AreEqual("Invaild Existing Password", observed.Value);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region ResetPasswordAsync

//        [TestMethod]
//        public async Task ResetPasswordAsync_Runs_ResetPasswordAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var resetPasswordRequest = new ResetPasswordRequest
//                    {
//                        Token = "A",
//                        NewPassword = "B",
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ResetPasswordAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ResetPasswordResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    await uut.ResetPasswordAsync(resetPasswordRequest);

//                    //Assert
//                    accountManagerMock
//                        .Verify(
//                            accountManager => accountManager.ResetPasswordAsync
//                            (
//                                resetPasswordRequest.Token,
//                                resetPasswordRequest.NewPassword,
//                                expectedCollerationId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_Successful_Returns200()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var resetPasswordRequest = new ResetPasswordRequest
//                    {
//                        Token = "A",
//                        NewPassword = "B",
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ResetPasswordAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ResetPasswordResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.ResetPasswordAsync(resetPasswordRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(200, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_TokenNotFound_Returns404()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var resetPasswordRequest = new ResetPasswordRequest
//                    {
//                        Token = "A",
//                        NewPassword = "B",
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ResetPasswordAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ResetPasswordResult.TokenNotFound
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.ResetPasswordAsync(resetPasswordRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(404, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_TokenExpired_ReturnsTokenExpiredAnd400()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var resetPasswordRequest = new ResetPasswordRequest
//                    {
//                        Token = "A",
//                        NewPassword = "B",
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ResetPasswordAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ResetPasswordResult.TokenExpired
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.ResetPasswordAsync(resetPasswordRequest) as ObjectResult;

//                    //Assert
//                    Assert.AreEqual(400, observed.StatusCode);
//                    Assert.AreEqual("Token Expired", observed.Value);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region UpdateEmailSettingsAsync

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_Runs_ResetPasswordAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var updateEmailSettingsRequest = new UpdateEmailSettingsRequest
//                    {
//                        Token = "A",
//                        EmailPreference = EmailPreference.Any
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.UpdateEmailSettingsAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<EmailPreference>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             UpdateEmailSettingsResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    await uut.UpdateEmailSettingsAsync(updateEmailSettingsRequest);

//                    //Assert
//                    accountManagerMock
//                        .Verify(
//                            accountManager => accountManager.UpdateEmailSettingsAsync
//                            (
//                                updateEmailSettingsRequest.Token,
//                                updateEmailSettingsRequest.EmailPreference,
//                                expectedCollerationId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_Successful_Returns200()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var updateEmailSettingsRequest = new UpdateEmailSettingsRequest
//                    {
//                        Token = "A",
//                        EmailPreference = EmailPreference.Any
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.UpdateEmailSettingsAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<EmailPreference>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             UpdateEmailSettingsResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.UpdateEmailSettingsAsync(updateEmailSettingsRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(200, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_TokenExpired_Returns400()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var updateEmailSettingsRequest = new UpdateEmailSettingsRequest
//                    {
//                        Token = "A",
//                        EmailPreference = EmailPreference.Any
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.UpdateEmailSettingsAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<EmailPreference>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             UpdateEmailSettingsResult.TokenExpired
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.UpdateEmailSettingsAsync(updateEmailSettingsRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(400, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_TokenNotFound_Returns400()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var updateEmailSettingsRequest = new UpdateEmailSettingsRequest
//                    {
//                        Token = "A",
//                        EmailPreference = EmailPreference.Any
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.UpdateEmailSettingsAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<EmailPreference>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             UpdateEmailSettingsResult.TokenNotFound
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.UpdateEmailSettingsAsync(updateEmailSettingsRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(400, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region ActivateEmailAsync

//        [TestMethod]
//        public async Task ActivateEmailAsync_Runs_ActivateEmailAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var activateEmailRequest = new ActivateEmailRequest
//                    {
//                        Token = "A"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ActivateEmailAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ActivateEmailResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    await uut.ActivateEmailAsync(activateEmailRequest);

//                    //Assert
//                    accountManagerMock
//                        .Verify(
//                            accountManager => accountManager.ActivateEmailAsync
//                            (
//                                activateEmailRequest.Token,
//                                expectedCollerationId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ActivateEmailAsync_Successful_Returns200()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var activateEmailRequest = new ActivateEmailRequest
//                    {
//                        Token = "A"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ActivateEmailAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ActivateEmailResult.Successful
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.ActivateEmailAsync(activateEmailRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(200, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ActivateEmailAsync_TokenExpired_Returns400()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var activateEmailRequest = new ActivateEmailRequest
//                    {
//                        Token = "A"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ActivateEmailAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ActivateEmailResult.TokenExpired
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.ActivateEmailAsync(activateEmailRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(400, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ActivateEmailAsync_TokenNotFound_Returns400()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var activateEmailRequest = new ActivateEmailRequest
//                    {
//                        Token = "A"
//                    };

//                    var expectedCollerationId = "CollerationId";
//                    var headers = new Dictionary<string, string>
//                    {
//                        { "X-Correlation-ID", expectedCollerationId }
//                    };

//                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

//                    accountManagerMock
//                        .Setup
//                        (
//                            accountManager => accountManager.ActivateEmailAsync
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                             ActivateEmailResult.TokenNotFound
//                        );

//                    var uut = SetupController<AccountController>(serviceProvider, headers);

//                    //Act
//                    var observed = await uut.ActivateEmailAsync(activateEmailRequest) as StatusCodeResult;

//                    //Assert
//                    Assert.AreEqual(400, observed.StatusCode);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region Helpers

//        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
//        {
//            serviceCollection.AddSingleton<AccountController>();
//            serviceCollection.AddSingleton(Mock.Of<IAccountManager>());
//            serviceCollection.AddSingleton(Mock.Of<ILoggingService<AccountController>>());
//            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());

//            return serviceCollection;
//        }
//        #endregion


//    }
//}
