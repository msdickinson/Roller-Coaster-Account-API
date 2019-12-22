using DickinsonBros.AccountAPI.Contracts;
using DickinsonBros.AccountAPI.Infrastructure.DateTime;
using DickinsonBros.AccountAPI.Infrastructure.JWT;
using DickinsonBros.AccountAPI.Infrastructure.Logging;
using DickinsonBros.AccountAPI.Logic.Account;
using DickinsonBros.AccountAPI.Logic.Account.Models;
using DickinsonBros.AccountAPI.View.Controllers;
using DickinsonBros.AccountAPI.View.Models;
using DickinsonBros.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DickinsonBros.AccountAPI.View.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests : BaseTest
    {
        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_Runs_AccountManagerCreateAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAccountRequest = new CreateAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new CreateAccountDescriptor
                            {
                                Result = CreateAccountResult.Successful,
                                AccountId = 1000
                            }
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = (await uut.CreateAsync(createAccountRequest)) as StatusCodeResult;

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.CreateAsync
                            (
                                createAccountRequest.Username,
                                createAccountRequest.Password,
                                createAccountRequest.Email
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAsync_IsSuccessful_ReturnsTokens()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAccountRequest = new CreateAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var expectedAccessToken = "AccessToken123";
                    var expectedRefreshToken = "RefreshToken123";

                    var accountId = 1000;

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.CreateAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new CreateAccountDescriptor
                            {
                                Result = CreateAccountResult.Successful,
                                AccountId = accountId
                            }
                        );

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT(It.IsAny<string>(), It.IsAny<DateTime>(), false)
                        )
                        .Returns
                        (
                            expectedAccessToken
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT(It.IsAny<string>(), It.IsAny<DateTime>(), true)
                        )
                        .Returns
                        (
                            expectedRefreshToken
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.CreateAsync(createAccountRequest) as OkObjectResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual(expectedAccessToken, ((JWTResponse)observed.Value).AccessToken);
                    Assert.AreEqual(AccountController.FIFTEEN_MIN_IN_SECONDS, ((JWTResponse)observed.Value).AccessTokenExpiresIn);
                    Assert.AreEqual(expectedRefreshToken, ((JWTResponse)observed.Value).RefreshToken);
                    Assert.AreEqual(AccountController.TWO_HOURS_IN_SECONDS, ((JWTResponse)observed.Value).RefreshTokenExpiresIn);
                    Assert.AreEqual(AccountController.BearerTokenType, ((JWTResponse)observed.Value).TokenType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateUser_ReturnsConflict409()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var createAccountRequest = new CreateAccountRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountDBService => accountDBService.CreateAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new CreateAccountDescriptor
                            {
                                Result = CreateAccountResult.DuplicateUser,
                                AccountId = null
                            }
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.CreateAsync(createAccountRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(409, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region LoginAsync

        [TestMethod]
        public async Task LoginAsync_Runs_AccountManagerLoginAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new LoginDescriptor
                            {
                                Result = LoginResult.Successful,
                                AccountId = 1000
                            }
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.LoginAsync(loginRequest) as ObjectResult;

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.LoginAsync
                            (
                                loginRequest.Username,
                                loginRequest.Password
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_IsSuccessful_ReturnsTokens()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var accountId = 1000;
                    var expectedAccessToken = "AccessToken123";
                    var expectedRefreshToken = "RefreshToken123";
                    
                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new LoginDescriptor
                            {
                                Result = LoginResult.Successful,
                                AccountId = accountId
                            }
                        );

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT(It.IsAny<string>(), It.IsAny<DateTime>(), false)
                        )
                        .Returns
                        (
                            expectedAccessToken
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT(It.IsAny<string>(), It.IsAny<DateTime>(), true)
                        )
                        .Returns
                        (
                            expectedRefreshToken
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.LoginAsync(loginRequest) as OkObjectResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual(expectedAccessToken, ((JWTResponse)observed.Value).AccessToken);
                    Assert.AreEqual(AccountController.FIFTEEN_MIN_IN_SECONDS, ((JWTResponse)observed.Value).AccessTokenExpiresIn);
                    Assert.AreEqual(expectedRefreshToken, ((JWTResponse)observed.Value).RefreshToken);
                    Assert.AreEqual(AccountController.TWO_HOURS_IN_SECONDS, ((JWTResponse)observed.Value).RefreshTokenExpiresIn);
                    Assert.AreEqual(AccountController.BearerTokenType, ((JWTResponse)observed.Value).TokenType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_AccountNotFound_Returns404()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new LoginDescriptor
                            {
                                Result = LoginResult.AccountNotFound
                            }
                        );
                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.LoginAsync(loginRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(404, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LoginAsync_AccountLocked_Returns403()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new LoginDescriptor
                            {
                                Result = LoginResult.AccountLocked
                            }
                        );
                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.LoginAsync(loginRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public async Task LoginAsync_InvaildPassword_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var loginRequest = new LoginRequest
                    {
                        Username = "User1000",
                        Password = "Password!"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.LoginAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            new LoginDescriptor
                            {
                                Result = LoginResult.InvaildPassword
                            }
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.LoginAsync(loginRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region RefreshTokenAsync

        [TestMethod]
        public async Task RefreshTokenAsync_VaildTokens_Returns200AndNewTokens()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var refreshTokenRequest = new RefreshTokenRequest
                    {
                        access_token = "AccessToken",
                        refresh_token = "RefreshToken"
                    };

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier.ToString(), "1000")
                    };

                    var expectedAccessToken = "AccessTokenValue";
                    var expectedRefreshToken = "RefreshTokenValue";

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();
                    
                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                false,
                                false
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                true,
                                true
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT
                            (
                                It.IsAny<IEnumerable<Claim>>(),
                                It.IsAny<DateTime>(),
                                It.IsAny<bool>()
                            )
                        )
                        .Returns(expectedAccessToken);

                       jwtServiceMock
                            .Setup
                            (
                                jwtService => jwtService.GenerateJWT
                                (
                                    It.IsAny<string>(),
                                    It.IsAny<DateTime>(),
                                    true
                                )
                            )
                            .Returns(expectedRefreshToken);

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RefreshTokenAsync(refreshTokenRequest) as OkObjectResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                    Assert.AreEqual(expectedAccessToken, ((JWTResponse)observed.Value).AccessToken);
                    Assert.AreEqual(expectedRefreshToken, ((JWTResponse)observed.Value).RefreshToken);
                    Assert.AreEqual(AccountController.FIFTEEN_MIN_IN_SECONDS, ((JWTResponse)observed.Value).AccessTokenExpiresIn);
                    Assert.AreEqual(AccountController.TWO_HOURS_IN_SECONDS, ((JWTResponse)observed.Value).RefreshTokenExpiresIn);
                    Assert.AreEqual(AccountController.BearerTokenType, ((JWTResponse)observed.Value).TokenType);


                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokenAsync_AccessTokenClaimsIsNull_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var refreshTokenRequest = new RefreshTokenRequest
                    {
                        access_token = "AccessToken",
                        refresh_token = "RefreshToken"
                    };

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier.ToString(), "1000")
                    };

                    var expectedAccessToken = "AccessTokenValue";
                    var expectedRefreshToken = "RefreshTokenValue";

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                false,
                                false
                            )
                        )
                        .Returns
                        (
                            (ClaimsPrincipal)null
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                true,
                                true
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT
                            (
                                It.IsAny<IEnumerable<Claim>>(),
                                It.IsAny<DateTime>(),
                                It.IsAny<bool>()
                            )
                        )
                        .Returns(expectedAccessToken);

                    jwtServiceMock
                         .Setup
                         (
                             jwtService => jwtService.GenerateJWT
                             (
                                 It.IsAny<string>(),
                                 It.IsAny<DateTime>(),
                                 true
                             )
                         )
                         .Returns(expectedRefreshToken);

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RefreshTokenAsync(refreshTokenRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokenAsync_RefreshTokenClaimsIsNull_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var refreshTokenRequest = new RefreshTokenRequest
                    {
                        access_token = "AccessToken",
                        refresh_token = "RefreshToken"
                    };

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier.ToString(), "1000")
                    };

                    var expectedAccessToken = "AccessTokenValue";
                    var expectedRefreshToken = "RefreshTokenValue";

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                false,
                                false
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                true,
                                true
                            )
                        )
                        .Returns
                        (
                            (ClaimsPrincipal)null
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT
                            (
                                It.IsAny<IEnumerable<Claim>>(),
                                It.IsAny<DateTime>(),
                                It.IsAny<bool>()
                            )
                        )
                        .Returns(expectedAccessToken);

                    jwtServiceMock
                         .Setup
                         (
                             jwtService => jwtService.GenerateJWT
                             (
                                 It.IsAny<string>(),
                                 It.IsAny<DateTime>(),
                                 true
                             )
                         )
                         .Returns(expectedRefreshToken);

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RefreshTokenAsync(refreshTokenRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokenAsync_AccessTokenIsNotAuthenticated_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var refreshTokenRequest = new RefreshTokenRequest
                    {
                        access_token = "AccessToken",
                        refresh_token = "RefreshToken"
                    };

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier.ToString(), "1000")
                    };

                    var expectedAccessToken = "AccessTokenValue";
                    var expectedRefreshToken = "RefreshTokenValue";

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                false,
                                false
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                true,
                                true
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT
                            (
                                It.IsAny<IEnumerable<Claim>>(),
                                It.IsAny<DateTime>(),
                                It.IsAny<bool>()
                            )
                        )
                        .Returns(expectedAccessToken);

                    jwtServiceMock
                         .Setup
                         (
                             jwtService => jwtService.GenerateJWT
                             (
                                 It.IsAny<string>(),
                                 It.IsAny<DateTime>(),
                                 true
                             )
                         )
                         .Returns(expectedRefreshToken);

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RefreshTokenAsync(refreshTokenRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokenAsync_RefreshTokenIsNotAuthenticated_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var refreshTokenRequest = new RefreshTokenRequest
                    {
                        access_token = "AccessToken",
                        refresh_token = "RefreshToken"
                    };

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier.ToString(), "1000")
                    };

                    var expectedAccessToken = "AccessTokenValue";
                    var expectedRefreshToken = "RefreshTokenValue";

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                false,
                                false
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                true,
                                true
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claims))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT
                            (
                                It.IsAny<IEnumerable<Claim>>(),
                                It.IsAny<DateTime>(),
                                It.IsAny<bool>()
                            )
                        )
                        .Returns(expectedAccessToken);

                    jwtServiceMock
                         .Setup
                         (
                             jwtService => jwtService.GenerateJWT
                             (
                                 It.IsAny<string>(),
                                 It.IsAny<DateTime>(),
                                 true
                             )
                         )
                         .Returns(expectedRefreshToken);

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RefreshTokenAsync(refreshTokenRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RefreshTokenAsync_NameIdentifierDontMatch_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var refreshTokenRequest = new RefreshTokenRequest
                    {
                        access_token = "AccessToken",
                        refresh_token = "RefreshToken"
                    };

                    var claimsAccess = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier.ToString(), "1000")
                    };

                    var claimsRefresh = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier.ToString(), "2000")
                    };

                    var expectedAccessToken = "AccessTokenValue";
                    var expectedRefreshToken = "RefreshTokenValue";

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                false,
                                false
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claimsAccess))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GetPrincipal
                            (
                                It.IsAny<string>(),
                                true,
                                true
                            )
                        )
                        .Returns
                        (
                            new ClaimsPrincipal(new ClaimsIdentity(claimsRefresh, "Basic"))
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT
                            (
                                It.IsAny<IEnumerable<Claim>>(),
                                It.IsAny<DateTime>(),
                                It.IsAny<bool>()
                            )
                        )
                        .Returns(expectedAccessToken);

                    jwtServiceMock
                         .Setup
                         (
                             jwtService => jwtService.GenerateJWT
                             (
                                 It.IsAny<string>(),
                                 It.IsAny<DateTime>(),
                                 true
                             )
                         )
                         .Returns(expectedRefreshToken);

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RefreshTokenAsync(refreshTokenRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region RequestPasswordResetEmailAsync

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_Runs_RequestPasswordResetEmailAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "Email@Email.com"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            RequestPasswordResetEmailResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as ObjectResult;

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                requestPasswordResetEmailRequest.Email
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "Email@Email.com"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            RequestPasswordResetEmailResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailNotFound_Returns404()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "Email@Email.com"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            RequestPasswordResetEmailResult.EmailNotFound
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(404, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_EmailNotActivated_Returns403AndMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "Email@Email.com"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            RequestPasswordResetEmailResult.EmailNotActivated
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as ObjectResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                    Assert.AreEqual(AccountController.EmailNotActivatedMessage, observed.Value.ToString());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task RequestPasswordResetEmailAsync_NoEmailSentDueToEmailPreference_Returns403AndMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var requestPasswordResetEmailRequest = new RequestPasswordResetEmailRequest
                    {
                        Email = "Email@Email.com"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.RequestPasswordResetEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                            RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest) as ObjectResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                    Assert.AreEqual(AccountController.NoEmailSentDueToEmailPreferenceMessage, observed.Value.ToString());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdatePasswordAsync

        [TestMethod]
        public async Task UpdatePasswordAsync_Runs_RequestPasswordResetEmailAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "A",
                        NewPassword = "B",
                    };

                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             UpdatePasswordResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    await uut.UpdatePasswordAsync(updatePasswordRequest);

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                updatePasswordRequest.ExistingPassword,
                                updatePasswordRequest.NewPassword
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "A",
                        NewPassword = "B",
                    };

                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             UpdatePasswordResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    var observed = await uut.UpdatePasswordAsync(updatePasswordRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_AccountLocked_Returns403()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "A",
                        NewPassword = "B",
                    };

                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             UpdatePasswordResult.AccountLocked
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    var observed = await uut.UpdatePasswordAsync(updatePasswordRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(403, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_InvaildExistingPassword_ReturnsInvaildExistingPaswordAnd400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updatePasswordRequest = new UpdatePasswordRequest
                    {
                        ExistingPassword = "A",
                        NewPassword = "B",
                    };

                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdatePasswordAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             UpdatePasswordResult.InvaildExistingPassword
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    var observed = await uut.UpdatePasswordAsync(updatePasswordRequest) as ObjectResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                    Assert.AreEqual("Invaild Existing Password", observed.Value);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region ResetPasswordAsync

        [TestMethod]
        public async Task ResetPasswordAsync_Runs_ResetPasswordAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "A",
                        NewPassword = "B",
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ResetPasswordResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    await uut.ResetPasswordAsync(resetPasswordRequest);

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                resetPasswordRequest.Token,
                                resetPasswordRequest.NewPassword
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "A",
                        NewPassword = "B",
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ResetPasswordResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(resetPasswordRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ResetPasswordAsync_TokenInvaild_ReturnsTokenExpiredAnd400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var resetPasswordRequest = new ResetPasswordRequest
                    {
                        Token = "A",
                        NewPassword = "B",
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ResetPasswordAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ResetPasswordResult.TokenInvaild
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.ResetPasswordAsync(resetPasswordRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdateEmailPreferenceWithTokenAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_InvaildToken_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.Any,
                        Token = "Token"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceWithTokenAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .ReturnsAsync
                        (
                             UpdateEmailPreferenceWithTokenResult.InvaildToken
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(updateEmailSettingsRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceWithTokenRequest
                    {
                        EmailPreference = EmailPreference.Any,
                        Token = "Token"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceWithTokenAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .ReturnsAsync
                        (
                             UpdateEmailPreferenceWithTokenResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(updateEmailSettingsRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdateEmailPreferenceAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_ResetPasswordAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.Any
                    };
                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .Returns
                        (
                             Task.CompletedTask
                        );


                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    await uut.UpdateEmailPreferenceAsync(updateEmailSettingsRequest);

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                expectedUserId,
                                updateEmailSettingsRequest.EmailPreference
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailSettingsRequest = new UpdateEmailPreferenceRequest
                    {
                        EmailPreference = EmailPreference.Any
                    };

                    var expectedUserId = 1;
                    var claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier.ToString(), expectedUserId.ToString())
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.UpdateEmailPreferenceAsync
                            (
                                It.IsAny<int>(),
                                It.IsAny<EmailPreference>()
                            )
                        )
                        .Returns
                        (
                             Task.CompletedTask
                        );


                    var uut = serviceProvider.GetControllerInstance<AccountController>(claims);

                    //Act
                    var observed = await uut.UpdateEmailPreferenceAsync(updateEmailSettingsRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region ActivateEmailAsync

        [TestMethod]
        public async Task ActivateEmailAsync_Runs_ActivateEmailAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "A"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    await uut.ActivateEmailAsync(activateEmailRequest);

                    //Assert
                    accountManagerMock
                        .Verify(
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                activateEmailRequest.Token
                            ),
                            Times.Once
                        );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_Successful_Returns200()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "A"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.Successful
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(activateEmailRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(200, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_EmailWasAlreadyActivated_Returns400()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "A"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();
                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.EmailWasAlreadyActivated
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(activateEmailRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(400, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ActivateEmailAsync_InvaildToken_Returns401()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailRequest = new ActivateEmailRequest
                    {
                        Token = "A"
                    };

                    var accountManagerMock = serviceProvider.GetMock<IAccountManager>();

                    accountManagerMock
                        .Setup
                        (
                            accountManager => accountManager.ActivateEmailAsync
                            (
                                It.IsAny<string>()
                            )
                        )
                        .ReturnsAsync
                        (
                             ActivateEmailResult.InvaildToken
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = await uut.ActivateEmailAsync(activateEmailRequest) as StatusCodeResult;

                    //Assert
                    Assert.AreEqual(401, observed.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region GetNewJWTTokens

        [TestMethod]
        public async Task GetNewJWTTokens_Runs_ReturnsJWTResponse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = "1000";
                    var expectedAccessToken = "AccessToken123";
                    var expectedRefreshToken = "RefreshToken123";

                    var jwtServiceMock = serviceProvider.GetMock<IJWTService>();

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT(It.IsAny<string>(), It.IsAny<DateTime>(), false)
                        )
                        .Returns
                        (
                            expectedAccessToken
                        );

                    jwtServiceMock
                        .Setup
                        (
                            jwtService => jwtService.GenerateJWT(It.IsAny<string>(), It.IsAny<DateTime>(), true)
                        )
                        .Returns
                        (
                            expectedRefreshToken
                        );

                    var uut = serviceProvider.GetControllerInstance<AccountController>();

                    //Act
                    var observed = uut.GetNewJWTTokens(accountId);

                    //Assert
                    Assert.AreEqual(expectedAccessToken, observed.AccessToken);
                    Assert.AreEqual(AccountController.FIFTEEN_MIN_IN_SECONDS, observed.AccessTokenExpiresIn);
                    Assert.AreEqual(expectedRefreshToken, observed.RefreshToken);
                    Assert.AreEqual(AccountController.TWO_HOURS_IN_SECONDS, observed.RefreshTokenExpiresIn);
                    Assert.AreEqual(AccountController.BearerTokenType, observed.TokenType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<AccountController>();
            serviceCollection.AddSingleton(Mock.Of<IAccountManager>());
            serviceCollection.AddSingleton(Mock.Of<ILoggingService<AccountController>>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());

            serviceCollection.AddSingleton(Mock.Of<IJWTService>());
          
            return serviceCollection;
        }
        #endregion
    }
}
