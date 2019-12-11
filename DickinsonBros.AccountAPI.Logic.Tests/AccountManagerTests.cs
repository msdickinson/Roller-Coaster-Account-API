//using DickinsonBros.AccountAPI.Infrastructure.AccountDB;
//using DickinsonBros.UnitTestHelper;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Threading.Tasks;
//using Moq;
//using DickinsonBros.AccountAPI.Infrastructure.AccountDB.Models;
//using DickinsonBros.AccountAPI.Logic.Account;
//using DickinsonBros.AccountAPI.Logic.Account.Models;
//using DickinsonBros.AccountAPI.Infrastructure.Encryption;
//using DickinsonBros.AccountAPI.Infrastructure.EmailSender;
//using DickinsonBros.AccountAPI.Infrastructure.DateTime;
//using DickinsonBros.AccountAPI.Infrastructure.Encryption.Models;
//using DickinsonBros.AccountAPI.Abstractions;
//using System;
//using System.Collections.Generic;
//using DickinsonBros.AccountAPI.Infrastructure.Email.Models;

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
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Hash = hash,
//                            Salt = salt
//                        }
//                    );

//                    //Act
//                    await uutConcrete.LoginAsync(username, password);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify(
//                        accountManager => accountManager.SelectAccountByUserNameAsync
//                        (
//                            username
//                        ),
//                        Times.Once
//                    );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task LoginAsync_AccountNotFound_ReturnsAccountNotFound()
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
//                    var expected = new LoginDescriptor
//                    {
//                        AccountId = null,
//                        Result = LoginResult.Successful
//                    };


//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByUserNameAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            (Contracts.Account)null
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    //Act
//                    var observed = await uutConcrete.LoginAsync(username, password);

//                    //Assert
//                    Assert.IsNull(observed.AccountId);
//                    Assert.AreEqual(LoginResult.AccountNotFound, observed.Result);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task LoginAsync_AccountLocked_ReturnsAccountLocked()
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

//                    var expected = new LoginDescriptor
//                    {
//                        AccountId = null,
//                        Result = LoginResult.Successful
//                    };

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByUserNameAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new Contracts.Account
//                            {
//                                Locked = true
//                            }
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    //Act
//                    var observed = await uutConcrete.LoginAsync(username, password);

//                    //Assert
//                    Assert.AreEqual(LoginResult.AccountLocked, observed.Result);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }


//        [TestMethod]
//        public async Task LoginAsync_AccountExist_CallsEncrypt()
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


//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
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
//                                PasswordHash = hash,
//                                Salt = salt
//                            }
//                        );

//                    var encryptionServiceMock = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceMock
//                    .Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Hash = hash,
//                            Salt = salt
//                        }
//                    );

//                    //Act
//                    await uutConcrete.LoginAsync(username, password);

//                    //Assert
//                    encryptionServiceMock
//                    .Verify(
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            password,
//                            salt
//                        ),
//                        Times.Once
//                    );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task LoginAsync_InvaildPassword_ReturnsInvaildPassword()
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
//                    var expectedAccountId = 1;

//                    var expected = new LoginDescriptor
//                    {
//                        AccountId = null,
//                        Result = LoginResult.Successful
//                    };


//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
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
//                                AccountId = expectedAccountId,
//                                PasswordHash = hash,
//                                Salt = salt
//                            }
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceStub
//                    .Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Hash = "",
//                            Salt = salt
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.LoginAsync(username, password);

//                    //Assert
//                    Assert.AreEqual(LoginResult.InvaildPassword, observed.Result);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task LoginAsync_Successful_ReturnsAccountAndIsSuccessful()
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
//                    var expectedAccountId = 1;

//                    var expected = new LoginDescriptor
//                    {
//                        AccountId = null,
//                        Result = LoginResult.Successful
//                    };


//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
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
//                                AccountId = expectedAccountId,
//                                PasswordHash = hash,
//                                Salt = salt
//                            }
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceStub
//                    .Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Hash = hash,
//                            Salt = salt
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.LoginAsync(username, password);

//                    //Assert
//                    Assert.AreEqual(expectedAccountId, observed.AccountId);
//                    Assert.AreEqual(LoginResult.Successful, observed.Result);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region CreateAsync

//        [TestMethod]
//        public async Task CreateAsync_Runs_CallsEncrypt()
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
//                    var email = (string)null;

//                    var encryptionServiceMock = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceMock.Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult()
//                    );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>()
//                        .Setup
//                        (
//                            guidService => guidService.NewGuid()
//                        )
//                        .Returns
//                        (
//                            new System.Guid()
//                        );

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.InsertAccountAsync
//                            (
//                                It.IsAny<InsertAccountRequest>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new InsertAccountResult
//                            {
//                                DuplicateUser = true,
//                                AccountId = null
//                            }
//                        );

//                    //Act
//                    var observed = await uutConcrete.CreateAsync(username, password, email);

//                    //Assert
//                    encryptionServiceMock
//                    .Verify(
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            password,
//                            null
//                        ),
//                        Times.Once
//                    );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task CreateAsync_Runs_CallsGuidServiceTwice()
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
//                    var email = (string)null;

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceStub.Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult()
//                    );

//                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();

//                    guidServiceMock
//                        .Setup
//                        (
//                            guidService => guidService.NewGuid()
//                        )
//                        .Returns
//                        (
//                            new System.Guid()
//                        );

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.InsertAccountAsync
//                            (
//                                It.IsAny<InsertAccountRequest>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new InsertAccountResult
//                            {
//                                DuplicateUser = true,
//                                AccountId = null
//                            }
//                        );

//                    //Act
//                    var observed = await uutConcrete.CreateAsync(username, password, email);

//                    //Assert
//                    guidServiceMock
//                    .Verify(
//                        guidService => guidService.NewGuid(),
//                        Times.Exactly(2)
//                    );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task CreateAsync_Runs_CallsInsertAccountAsync()
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
//                    var email = (string)null;
//                    var expectedSalt = "salt";
//                    var expectedHash = "hash";
//                    var emailActivateToken = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
//                    var emailPerferncesToken = new Guid("a3d1628a-1ddf-422e-9871-deb5a9770429");
//                    var emailActivateTokenReturned = false;

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    var observedInsertAccountRequest = (InsertAccountRequest)null;

//                    encryptionServiceStub.Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Salt = expectedSalt,
//                            Hash = expectedHash
//                        }
//                   );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

//                    guidServiceStub
//                        .Setup
//                        (
//                            guidService => guidService.NewGuid()
//                        )
//                        .Returns
//                        (() =>
//                        {
//                            var token = emailActivateTokenReturned ? emailPerferncesToken : emailActivateToken;
//                            emailActivateTokenReturned = true;
//                            return token;
//                        });


//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.InsertAccountAsync
//                            (
//                                It.IsAny<InsertAccountRequest>()
//                            )
//                        )
//                        .Callback((InsertAccountRequest insertAccountRequest) =>
//                        {
//                            observedInsertAccountRequest = insertAccountRequest;
//                        })
//                        .ReturnsAsync
//                        (
//                            new InsertAccountResult
//                            {
//                                DuplicateUser = true,
//                                AccountId = null
//                            }
//                        );

//                    //Act
//                    var observed = await uutConcrete.CreateAsync(username, password, email);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify(
//                        accountDBService => accountDBService.InsertAccountAsync
//                        (
//                            observedInsertAccountRequest
//                        ),
//                        Times.Once
//                    );

//                    Assert.AreEqual(username, observedInsertAccountRequest.Username);
//                    Assert.AreEqual(expectedHash, observedInsertAccountRequest.PasswordHash);
//                    Assert.AreEqual(expectedSalt, observedInsertAccountRequest.Salt);
//                    Assert.AreEqual(email, observedInsertAccountRequest.Email);
//                    Assert.AreEqual(emailActivateToken.ToString(), observedInsertAccountRequest.ActivateEmailToken);
//                    Assert.AreEqual(emailPerferncesToken.ToString(), observedInsertAccountRequest.EmailPreferenceToken);
//                    Assert.AreEqual(EmailPreference.Any, observedInsertAccountRequest.EmailPreference);

//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task CreateAsync_DuplicateUser_ReturnsDuplicateUser()
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
//                    var email = (string)null;
//                    var expectedSalt = "salt";
//                    var expectedHash = "hash";
//                    var emailActivateToken = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
//                    var emailPerferncesToken = new Guid("a3d1628a-1ddf-422e-9871-deb5a9770429");
//                    var emailActivateTokenReturned = false;

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceStub.Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Salt = expectedSalt,
//                            Hash = expectedHash
//                        }
//                    );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

//                    guidServiceStub
//                    .Setup
//                    (
//                        guidService => guidService.NewGuid()
//                    )
//                    .Returns
//                    (() =>
//                    {
//                        var token = emailActivateTokenReturned ? emailPerferncesToken : emailActivateToken;
//                        emailActivateTokenReturned = true;
//                        return token;
//                    });


//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.InsertAccountAsync
//                            (
//                                It.IsAny<InsertAccountRequest>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                           new InsertAccountResult
//                           {
//                               DuplicateUser = true,
//                               AccountId = null
//                           }
//                        );

//                    //Act
//                    var observed = await uutConcrete.CreateAsync(username, password, email);

//                    //Assert
//                    Assert.AreEqual(CreateAccountResult.DuplicateUser, observed.Result);
//                    Assert.IsNull(observed.AccountId);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task CreateAsync_Successful_ReturnsAccountIdAndSuccessful()
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
//                    var email = (string)null;
//                    var expectedSalt = "salt";
//                    var expectedHash = "hash";
//                    var emailActivateToken = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
//                    var emailPerferncesToken = new Guid("a3d1628a-1ddf-422e-9871-deb5a9770429");
//                    var emailActivateTokenReturned = false;
//                    var expectedAccountId = 1;

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceStub.Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Salt = expectedSalt,
//                            Hash = expectedHash
//                        }
//                    );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

//                    guidServiceStub
//                    .Setup
//                    (
//                        guidService => guidService.NewGuid()
//                    )
//                    .Returns
//                    (() =>
//                    {
//                        var token = emailActivateTokenReturned ? emailPerferncesToken : emailActivateToken;
//                        emailActivateTokenReturned = true;
//                        return token;
//                    });


//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.InsertAccountAsync
//                            (
//                                It.IsAny<InsertAccountRequest>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                           new InsertAccountResult
//                           {
//                               DuplicateUser = false,
//                               AccountId = expectedAccountId
//                           }
//                        );

//                    //Act
//                    var observed = await uutConcrete.CreateAsync(username, password, email);

//                    //Assert
//                    Assert.AreEqual(CreateAccountResult.Successful, observed.Result);
//                    Assert.AreEqual(expectedAccountId, observed.AccountId);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task CreateAsync_SuccessfulAndHasEmail_CallsSendActivateEmailAsync()
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
//                    var email = "a@test.com";
//                    var expectedSalt = "salt";
//                    var expectedHash = "hash";
//                    var emailActivateToken = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
//                    var updateEmailSettingsToken = new Guid("a3d1628a-1ddf-422e-9871-deb5a9770429");
//                    var emailActivateTokenReturned = false;
//                    var expectedAccountId = 1;
//                    var observedSendActivateEmailRequest = (SendActivateEmailRequest)null;
//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();

//                    encryptionServiceStub.Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        new EncryptResult
//                        {
//                            Salt = expectedSalt,
//                            Hash = expectedHash
//                        }
//                    );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

//                    guidServiceStub
//                    .Setup
//                    (
//                        guidService => guidService.NewGuid()
//                    )
//                    .Returns
//                    (() =>
//                    {
//                        var token = emailActivateTokenReturned ? updateEmailSettingsToken : emailActivateToken;
//                        emailActivateTokenReturned = true;
//                        return token;
//                    });


//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.InsertAccountAsync
//                            (
//                                It.IsAny<InsertAccountRequest>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                           new InsertAccountResult
//                           {
//                               DuplicateUser = false,
//                               AccountId = expectedAccountId
//                           }
//                        );


//                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();

//                    emailServiceMock
//                        .Setup
//                        (
//                            emailService => emailService.SendActivateEmailAsync
//                            (
//                                It.IsAny<SendActivateEmailRequest>()
//                            )
//                        )
//                        .Callback((SendActivateEmailRequest sendActivateEmailRequest) =>
//                        {
//                            observedSendActivateEmailRequest = sendActivateEmailRequest;
//                        });


//                    //Act
//                    var observed = await uutConcrete.CreateAsync(username, password, email);

//                    //Assert
//                    emailServiceMock
//                        .Verify(
//                            emailService => emailService.SendActivateEmailAsync
//                            (
//                                observedSendActivateEmailRequest
//                            ),
//                            Times.Once
//                        );

//                    Assert.AreEqual(email, observedSendActivateEmailRequest.Email);
//                    Assert.AreEqual(updateEmailSettingsToken.ToString(), observedSendActivateEmailRequest.UpdateEmailSettingsToken);
//                    Assert.AreEqual(emailActivateToken.ToString(), observedSendActivateEmailRequest.ActivateToken);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }


//        #endregion

//        #region RequestPasswordResetEmailAsync

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_Runs_CallsSelectAccountByEmailAsync()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var email = (string)null;

//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByEmailAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            (Contracts.Account)null
//                        );

//                    //Act
//                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify(
//                        accountDBService => accountDBService.SelectAccountByEmailAsync
//                        (
//                            email
//                        ),
//                        Times.Once
//                    );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_AccountIsNull_ReturnsEmailNotFound()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var email = (string)null;

//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByEmailAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            (Contracts.Account)null
//                        );

//                    //Act
//                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

//                    //Assert
//                    Assert.AreEqual(RequestPasswordResetEmailResult.EmailNotFound, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_AccountExist_NewGuidCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var email = (string)null;

//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByEmailAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new Contracts.Account
//                            {
//                                Username = ""
//                            }
//                        );

//                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();

//                    guidServiceMock
//                        .Setup
//                        (
//                            guidService => guidService.NewGuid()
//                        )
//                        .Returns
//                        (
//                           new System.Guid()
//                        );

//                    accountDBServiceMock
//                      .Setup
//                      (
//                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
//                          (
//                              It.IsAny<InsertPasswordResetTokenRequest>()
//                          )
//                      )
//                      .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

//                    //Assert
//                    guidServiceMock
//                        .Verify(
//                            guidService => guidService.NewGuid(),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_AccountExist_InsertPasswordResetTokenAsyncCalled()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var email = (string)null;
//                    var expectedAccount = new Contracts.Account
//                    {
//                        AccountId = 1,
//                        ActivateEmailToken = Guid.NewGuid(),
//                        EmailPreferenceToken = Guid.NewGuid()
//                    };
//                    var expectedGuid = Guid.NewGuid();
//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
//                    var insertPasswordResetTokenRequestObserved = (InsertPasswordResetTokenRequest)null;

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByEmailAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            expectedAccount
//                        );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

//                    guidServiceStub
//                        .Setup
//                        (
//                            guidService => guidService.NewGuid()
//                        )
//                        .Returns
//                        (
//                           expectedGuid
//                        );

//                    accountDBServiceMock
//                      .Setup
//                      (
//                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
//                          (
//                              It.IsAny<InsertPasswordResetTokenRequest>()
//                          )
//                      )
//                      .Callback((InsertPasswordResetTokenRequest insertPasswordResetTokenRequest) =>
//                      {
//                          insertPasswordResetTokenRequestObserved = insertPasswordResetTokenRequest;
//                      })
//                      .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify(
//                        accountDBService => accountDBService.InsertPasswordResetTokenAsync
//                        (
//                            insertPasswordResetTokenRequestObserved
//                        ),
//                        Times.Once
//                    );
//                    Assert.AreEqual(expectedAccount.AccountId, insertPasswordResetTokenRequestObserved.AccountId);
//                    Assert.AreEqual(expectedGuid.ToString(), insertPasswordResetTokenRequestObserved.PasswordResetToken);

//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_Successful_SendPasswordResetEmail()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var email = (string)null;
//                    var expectedAccount = new Contracts.Account
//                    {
//                        AccountId = 1,
//                        ActivateEmailToken = Guid.NewGuid(),
//                        EmailPreferenceToken = Guid.NewGuid()
//                    };
//                    var passwordResetToken = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
//                    var sendPasswordResetEmailRequestObserved = (SendPasswordResetEmailRequest)null;

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByEmailAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            expectedAccount
//                        );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

//                    guidServiceStub
//                        .Setup
//                        (
//                            guidService => guidService.NewGuid()
//                        )
//                        .Returns
//                        (
//                           passwordResetToken
//                        );

//                    accountDBServiceMock
//                      .Setup
//                      (
//                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
//                          (
//                              It.IsAny<InsertPasswordResetTokenRequest>()
//                          )
//                      )
//                      .Returns(Task.CompletedTask);

//                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();

//                    emailServiceMock
//                        .Setup
//                        (
//                            emailService => emailService.SendPasswordResetEmailAsync
//                            (
//                                It.IsAny<SendPasswordResetEmailRequest>()
//                            )
//                        )
//                        .Callback((SendPasswordResetEmailRequest sendPasswordResetEmailRequest) =>
//                        {
//                            sendPasswordResetEmailRequestObserved = sendPasswordResetEmailRequest;
//                        });


//                    //Act
//                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

//                    //Assert
//                    emailServiceMock
//                    .Verify(
//                        emailService => emailService.SendPasswordResetEmailAsync
//                        (
//                            sendPasswordResetEmailRequestObserved
//                        ),
//                        Times.Once
//                    );
//                    Assert.AreEqual(expectedAccount.Email, sendPasswordResetEmailRequestObserved.Email);
//                    Assert.AreEqual(passwordResetToken.ToString(), sendPasswordResetEmailRequestObserved.PasswordResetToken);
//                    Assert.AreEqual(expectedAccount.EmailPreferenceToken, sendPasswordResetEmailRequestObserved.EmailPreferenceToken);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task RequestPasswordResetEmailAsync_Successful_ReturnsSuccessful()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var email = (string)null;
//                    var expectedAccount = new Contracts.Account
//                    {
//                        AccountId = 1,
//                        ActivateEmailToken = Guid.NewGuid(),
//                        EmailPreferenceToken = Guid.NewGuid()
//                    };
//                    var expectedGuid = new Guid("15d5f597-bcbe-463c-9706-adc33cbacbf8");
//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByEmailAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            expectedAccount
//                        );

//                    var guidServiceStub = serviceProvider.GetMock<IGuidService>();

//                    guidServiceStub
//                        .Setup
//                        (
//                            guidService => guidService.NewGuid()
//                        )
//                        .Returns
//                        (
//                           expectedGuid
//                        );

//                    accountDBServiceStub
//                      .Setup
//                      (
//                          accountDBService => accountDBService.InsertPasswordResetTokenAsync
//                          (
//                              It.IsAny<InsertPasswordResetTokenRequest>()
//                          )
//                      )
//                      .Returns(Task.CompletedTask);

//                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();

//                    emailServiceMock
//                        .Setup
//                        (
//                            emailService => emailService.SendPasswordResetEmailAsync
//                            (
//                                It.IsAny<SendPasswordResetEmailRequest>()
//                            )
//                        );

//                    //Act
//                    var observed = await uutConcrete.RequestPasswordResetEmailAsync(email);

//                    //Assert
//                    Assert.AreEqual(RequestPasswordResetEmailResult.Successful, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }


//        #endregion

//        #region UpdatePasswordAsync

//        [TestMethod]
//        public async Task UpdatePasswordAsync_Runs_CallsSelectAccountByAccountIdAsync()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            (Contracts.Account)null
//                        );

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    accountDBServiceMock
//                        .Verify(
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                accountId
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_AccountNotFound_ReturnsAccountNotFound()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            (Contracts.Account)null
//                        );

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    Assert.AreEqual(UpdatePasswordResult.AccountNotFound, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_AccountLocked_ReturnsAccountNotFound()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new Contracts.Account
//                            {
//                                Locked = true
//                            }
//                        );

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    Assert.AreEqual(UpdatePasswordResult.AccountLocked, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_AccountSelected_EncryptExistingPassword()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var account = new Contracts.Account
//                    {
//                        AccountId = accountId,
//                        PasswordHash = "C",
//                        Salt = "D"
//                    };

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            account
//                        );

//                    var encryptionServiceMock = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceMock
//                        .Setup
//                        (
//                            encryptionService => encryptionService.Encrypt
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .Returns
//                        (
//                            new EncryptResult
//                            {
//                                Hash = "E",
//                                Salt = "D"
//                            }
//                        );

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    encryptionServiceMock
//                        .Verify(
//                            encryptionService => encryptionService.Encrypt
//                            (
//                                existingPassword,
//                                account.Salt
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_InvaildExistingPassword_ReturnsInvaildExistingPassword()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var account = new Contracts.Account
//                    {
//                        AccountId = accountId,
//                        PasswordHash = "C",
//                        Salt = "D"
//                    };

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            account
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceStub
//                        .Setup
//                        (
//                            encryptionService => encryptionService.Encrypt
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .Returns
//                        (
//                            new EncryptResult
//                            {
//                                Hash = "E",
//                                Salt = "D"
//                            }
//                        );

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    Assert.AreEqual(UpdatePasswordResult.InvaildExistingPassword, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_ExistingPasswordsMatch_EncryptNewPassword()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var account = new Contracts.Account
//                    {
//                        AccountId = accountId,
//                        PasswordHash = "E",
//                        Salt = "D"
//                    };

//                    var encryptResultsIndex = 0;
//                    var encryptResults = new List<EncryptResult>
//                    {
//                        new EncryptResult
//                        {
//                            Hash = "E",
//                            Salt = "D"
//                        },
//                        new EncryptResult
//                        {
//                            Hash = "F",
//                            Salt = "G"
//                        }
//                    };

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            account
//                        );

//                    var encryptionServiceMock = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceMock
//                    .Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        () =>
//                        {
//                            var result = encryptResults[encryptResultsIndex];
//                            encryptResultsIndex++;
//                            return result;
//                        });

//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdatePasswordAsync
//                        (
//                            It.IsAny<UpdatePasswordRequest>()
//                        )
//                    )
//                    .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    encryptionServiceMock
//                        .Verify(
//                            encryptionService => encryptionService.Encrypt
//                            (
//                                newPassword,
//                                null
//                            ),
//                            Times.Once
//                        );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_ExistingPasswordsMatch_CallsUpdatedPasswordAsync()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var account = new Contracts.Account
//                    {
//                        AccountId = accountId,
//                        PasswordHash = "E",
//                        Salt = "D"
//                    };

//                    var encryptResultsIndex = 0;
//                    var encryptResults = new List<EncryptResult>
//                    {
//                        new EncryptResult
//                        {
//                            Hash = "E",
//                            Salt = "D"
//                        },
//                        new EncryptResult
//                        {
//                            Hash = "F",
//                            Salt = "G"
//                        }
//                    };

//                    var updatePasswordRequestObserved = (UpdatePasswordRequest)null;

//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            account
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceStub
//                    .Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        () =>
//                        {
//                            var result = encryptResults[encryptResultsIndex];
//                            encryptResultsIndex++;
//                            return result;
//                        });

//                    accountDBServiceMock
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdatePasswordAsync
//                        (
//                            It.IsAny<UpdatePasswordRequest>()
//                        )
//                    )
//                    .Callback((UpdatePasswordRequest updatePasswordRequest) =>
//                    {
//                        updatePasswordRequestObserved = updatePasswordRequest;
//                    })
//                    .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    accountDBServiceMock
//                        .Verify(
//                            accountDBService => accountDBService.UpdatePasswordAsync
//                            (
//                                updatePasswordRequestObserved
//                            ),
//                            Times.Once
//                        );

//                    Assert.AreEqual(accountId, updatePasswordRequestObserved.AccountId);
//                    Assert.AreEqual(encryptResults[1].Hash, updatePasswordRequestObserved.PasswordHash);
//                    Assert.AreEqual(encryptResults[1].Salt, updatePasswordRequestObserved.Salt);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdatePasswordAsync_Successful_ReturnSuccessful()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var accountId = 1;
//                    string existingPassword = "A";
//                    string newPassword = "B";
//                    var account = new Contracts.Account
//                    {
//                        AccountId = accountId,
//                        PasswordHash = "E",
//                        Salt = "D"
//                    };

//                    var encryptResultsIndex = 0;
//                    var encryptResults = new List<EncryptResult>
//                    {
//                        new EncryptResult
//                        {
//                            Hash = "E",
//                            Salt = "D"
//                        },
//                        new EncryptResult
//                        {
//                            Hash = "F",
//                            Salt = "G"
//                        }
//                    };

//                    var updatePasswordRequestObserved = (UpdatePasswordRequest)null;

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountByAccountIdAsync
//                            (
//                                It.IsAny<int>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            account
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceStub
//                    .Setup
//                    (
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            It.IsAny<string>(),
//                            It.IsAny<string>()
//                        )
//                    )
//                    .Returns
//                    (
//                        () =>
//                        {
//                            var result = encryptResults[encryptResultsIndex];
//                            encryptResultsIndex++;
//                            return result;
//                        });

//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdatePasswordAsync
//                        (
//                            It.IsAny<UpdatePasswordRequest>()
//                        )
//                    )
//                    .Callback((UpdatePasswordRequest updatePasswordRequest) =>
//                    {
//                        updatePasswordRequestObserved = updatePasswordRequest;
//                    })
//                    .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.UpdatePasswordAsync(accountId, existingPassword, newPassword);

//                    //Assert
//                    Assert.AreEqual(UpdatePasswordResult.Successful, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }


//        #endregion

//        #region ResetPasswordAsync

//        [TestMethod]
//        public async Task ResetPasswordAsync_Runs_CallsSelectAccountByAccountIdAsync()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    string newPassword = "B";
//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                    .Setup
//                    (
//                        accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
//                        (
//                            It.IsAny<string>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                        new SelectPasswordResetTokenResult
//                        {
//                            TokenExpired = true,
//                            TokenFound = true
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify
//                    (
//                        accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
//                        (
//                            token
//                        ),
//                        Times.Once
//                    );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_TokenNotFound_ReturnsTokenNotFound()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    string newPassword = "B";
//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                    .Setup
//                    (
//                        accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
//                        (
//                            It.IsAny<string>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                        new SelectPasswordResetTokenResult
//                        {
//                            TokenFound = false
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

//                    //Assert
//                    Assert.AreEqual(ResetPasswordResult.TokenNotFound, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_TokenExpired_ReturnsTokenExpired()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    string newPassword = "B";
//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceMock
//                    .Setup
//                    (
//                        accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
//                        (
//                            It.IsAny<string>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                        new SelectPasswordResetTokenResult
//                        {
//                            TokenFound = true,
//                            TokenExpired = true
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

//                    //Assert
//                    Assert.AreEqual(ResetPasswordResult.TokenExpired, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_AccountSelected_CallEncryptNewPassword()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    var accountId = 1;
//                    string newPassword = "A";
//                    var encryptResult = new EncryptResult
//                    {
//                        Hash = "B",
//                        Salt = "C"
//                    };
//                    var passwordObserved = (string)null;
//                    var hashObserved = (string)null;

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new SelectPasswordResetTokenResult
//                            {
//                                TokenFound = true,
//                                TokenExpired = false,
//                                AccountId = accountId
//                            }
//                        );

//                    var encryptionServiceMock = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceMock
//                        .Setup
//                        (
//                            encryptionService => encryptionService.Encrypt
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .Callback((string password, string hash) =>
//                        {
//                            passwordObserved = password;
//                            hashObserved = hash;
//                        })
//                        .Returns
//                        (
//                            encryptResult
//                        );

//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdatePasswordAsync
//                        (
//                            It.IsAny<UpdatePasswordRequest>()
//                        )
//                    )
//                    .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

//                    //Assert
//                    encryptionServiceMock
//                    .Verify(
//                        encryptionService => encryptionService.Encrypt
//                        (
//                            passwordObserved,
//                            hashObserved
//                        ),
//                        Times.Once
//                    );

//                    Assert.AreEqual(newPassword, passwordObserved);
//                    Assert.IsNull(null, hashObserved);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_AccountSelected_CallUpdatePassword()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    var accountId = 1;
//                    string newPassword = "A";
//                    var encryptResult = new EncryptResult
//                    {
//                        Hash = "B",
//                        Salt = "C"
//                    };
//                    var updatePasswordRequestObserved = (UpdatePasswordRequest)null;

//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceMock
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new SelectPasswordResetTokenResult
//                            {
//                                TokenFound = true,
//                                TokenExpired = false,
//                                AccountId = accountId
//                            }
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceStub
//                        .Setup
//                        (
//                            encryptionService => encryptionService.Encrypt
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .Returns
//                        (
//                            encryptResult
//                        );

//                    accountDBServiceMock
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdatePasswordAsync
//                        (
//                            It.IsAny<UpdatePasswordRequest>()
//                        )
//                    )
//                    .Callback((UpdatePasswordRequest updatePasswordRequest) =>
//                    {
//                        updatePasswordRequestObserved = updatePasswordRequest;
//                    })
//                    .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify(
//                        accountDBService => accountDBService.UpdatePasswordAsync
//                        (
//                            updatePasswordRequestObserved
//                        ),
//                        Times.Once
//                    );
//                    Assert.AreEqual(accountId, updatePasswordRequestObserved.AccountId);
//                    Assert.AreEqual(encryptResult.Hash, updatePasswordRequestObserved.PasswordHash);
//                    Assert.AreEqual(encryptResult.Salt, updatePasswordRequestObserved.Salt);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ResetPasswordAsync_Successful_ReturnsSuccessful()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    var accountId = 1;
//                    string newPassword = "A";
//                    var encryptResult = new EncryptResult
//                    {
//                        Hash = "B",
//                        Salt = "C"
//                    };

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                        .Setup
//                        (
//                            accountDBService => accountDBService.SelectAccountIdFromPasswordResetTokenAsync
//                            (
//                                It.IsAny<string>()
//                            )
//                        )
//                        .ReturnsAsync
//                        (
//                            new SelectPasswordResetTokenResult
//                            {
//                                TokenFound = true,
//                                TokenExpired = false,
//                                AccountId = accountId
//                            }
//                        );

//                    var encryptionServiceStub = serviceProvider.GetMock<IEncryptionService>();
//                    encryptionServiceStub
//                        .Setup
//                        (
//                            encryptionService => encryptionService.Encrypt
//                            (
//                                It.IsAny<string>(),
//                                It.IsAny<string>()
//                            )
//                        )
//                        .Returns
//                        (
//                            encryptResult
//                        );

//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdatePasswordAsync
//                        (
//                            It.IsAny<UpdatePasswordRequest>()
//                        )
//                    )
//                    .Returns(Task.CompletedTask);

//                    //Act
//                    var observed = await uutConcrete.ResetPasswordAsync(token, newPassword);

//                    //Assert
//                    Assert.AreEqual(ResetPasswordResult.Successful, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        #endregion

//        #region UpdateEmailSettingsAsync

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_Runs_CallsUpdateEmailPreferencesWithTokenAsync()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    var emailPreference = EmailPreference.Any;
//                    var updateEmailPreferencesWithTokenRequestObserved = (UpdateEmailPreferencesWithTokenRequest)null;

//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceMock
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
//                        (
//                            It.IsAny<UpdateEmailPreferencesWithTokenRequest>()
//                        )
//                    )
//                    .Callback((UpdateEmailPreferencesWithTokenRequest updateEmailPreferencesWithTokenRequest) =>
//                    {
//                        updateEmailPreferencesWithTokenRequestObserved = updateEmailPreferencesWithTokenRequest;
//                    })
//                    .ReturnsAsync
//                    (
//                       new UpdateEmailPreferencesWithTokenDBResult
//                       {
//                           TokenFound = true
//                       }
//                    );

//                    //Act
//                    var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify(
//                        accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
//                        (
//                            updateEmailPreferencesWithTokenRequestObserved
//                        ),
//                        Times.Once
//                    );
//                    Assert.AreEqual(token, updateEmailPreferencesWithTokenRequestObserved.EmailPreferenceToken);
//                    Assert.AreEqual(emailPreference, updateEmailPreferencesWithTokenRequestObserved.EmailPreference);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_TokenNotFound_ReturnsTokenNotFound()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    var emailPreference = EmailPreference.Any;
//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
//                        (
//                            It.IsAny<UpdateEmailPreferencesWithTokenRequest>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                        new UpdateEmailPreferencesWithTokenDBResult
//                        {
//                            TokenFound = false
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference);

//                    //Assert
//                    Assert.AreEqual(UpdateEmailSettingsResult.TokenNotFound, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_TokenExpired_ReturnsTokenExpired()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    var emailPreference = EmailPreference.Any;
//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
//                        (
//                            It.IsAny<UpdateEmailPreferencesWithTokenRequest>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                        new UpdateEmailPreferencesWithTokenDBResult
//                        {
//                            TokenFound = true,
//                            TokenExpired = true
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference);

//                    //Assert
//                    Assert.AreEqual(UpdateEmailSettingsResult.TokenExpired, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task UpdateEmailSettingsAsync_VaildToken_ReturnsSuccessful()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";
//                    var emailPreference = EmailPreference.Any;
//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();

//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
//                        (
//                            It.IsAny<UpdateEmailPreferencesWithTokenRequest>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                        new UpdateEmailPreferencesWithTokenDBResult
//                        {
//                            TokenFound = true,
//                            TokenExpired = false
//                        }
//                    );

//                    //Act
//                    var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference);

//                    //Assert
//                    Assert.AreEqual(UpdateEmailSettingsResult.Successful, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }
//        #endregion

//        #region ActivateEmailAsync

//        [TestMethod]
//        public async Task ActivateEmailAsync_Runs_CallsActivateEmailWithTokenAsync()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";

//                    var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceMock
//                    .Setup
//                    (
//                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
//                        (
//                            It.IsAny<string>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                       new ActivateEmailWithTokenDBResult
//                       {
//                           TokenFound = false
//                       }
//                    );

//                    //Act
//                    var observed = await uutConcrete.ActivateEmailAsync(token);

//                    //Assert
//                    accountDBServiceMock
//                    .Verify(
//                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
//                        (
//                            token
//                        ),
//                        Times.Once
//                    );
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ActivateEmailAsync_TokenNotFound_ReturnsTokenNotFound()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
//                        (
//                            It.IsAny<string>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                       new ActivateEmailWithTokenDBResult
//                       {
//                           TokenFound = false
//                       }
//                    );

//                    //Act
//                    var observed = await uutConcrete.ActivateEmailAsync(token);

//                    //Assert
//                    Assert.AreEqual(ActivateEmailResult.TokenNotFound, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ActivateEmailAsync_TokenExpired_ReturnsTokenExpired()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
//                        (
//                            It.IsAny<string>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                       new ActivateEmailWithTokenDBResult
//                       {
//                           TokenFound = true,
//                           TokenExpired = true
//                       }
//                    );

//                    //Act
//                    var observed = await uutConcrete.ActivateEmailAsync(token);

//                    //Assert
//                    Assert.AreEqual(ActivateEmailResult.TokenExpired, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        [TestMethod]
//        public async Task ActivateEmailAsync_VaildToken_ReturnsSuccessful()
//        {
//            await RunDependencyInjectedTestAsync
//            (
//                async (serviceProvider) =>
//                {
//                    //Setup
//                    var uut = serviceProvider.GetRequiredService<IAccountManager>();
//                    var uutConcrete = (AccountManager)uut;
//                    var token = "1";

//                    var accountDBServiceStub = serviceProvider.GetMock<IAccountDBService>();
//                    accountDBServiceStub
//                    .Setup
//                    (
//                        accountDBService => accountDBService.ActivateEmailWithTokenAsync
//                        (
//                            It.IsAny<string>()
//                        )
//                    )
//                    .ReturnsAsync
//                    (
//                       new ActivateEmailWithTokenDBResult
//                       {
//                           TokenFound = true,
//                           TokenExpired = false
//                       }
//                    );

//                    //Act
//                    var observed = await uutConcrete.ActivateEmailAsync(token);

//                    //Assert
//                    Assert.AreEqual(ActivateEmailResult.Successful, observed);
//                },
//                serviceCollection => ConfigureServices(serviceCollection)
//            );
//        }

//        //[TestMethod]
//        //public async Task UpdateEmailSettingsAsync_TokenExpired_ReturnsTokenExpired()
//        //{
//        //    await RunDependencyInjectedTestAsync
//        //    (
//        //        async (serviceProvider) =>
//        //        {
//        //            //Setup
//        //            var uut = serviceProvider.GetRequiredService<IAccountManager>();
//        //            var uutConcrete = (AccountManager)uut;
//        //            var correlationId = "CollerationId";
//        //            var token = "1";
//        //            var emailPreference = EmailPreference.Any;
//        //            var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//        //            accountDBServiceMock
//        //            .Setup
//        //            (
//        //                accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
//        //                (
//        //                    It.IsAny<UpdateEmailPreferencesWithTokenRequest>(),
//        //                    It.IsAny<string>()
//        //                )
//        //            )
//        //            .ReturnsAsync
//        //            (
//        //                new UpdateEmailPreferencesWithTokenDBResult
//        //                {
//        //                    TokenFound = true,
//        //                    TokenExpired = true
//        //                }
//        //            );

//        //            //Act
//        //            var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference, correlationId);

//        //            //Assert
//        //            Assert.AreEqual(UpdateEmailSettingsResult.TokenExpired, observed);
//        //        },
//        //        serviceCollection => ConfigureServices(serviceCollection)
//        //    );
//        //}

//        //[TestMethod]
//        //public async Task UpdateEmailSettingsAsync_VaildToken_ReturnsSuccessful()
//        //{
//        //    await RunDependencyInjectedTestAsync
//        //    (
//        //        async (serviceProvider) =>
//        //        {
//        //            //Setup
//        //            var uut = serviceProvider.GetRequiredService<IAccountManager>();
//        //            var uutConcrete = (AccountManager)uut;
//        //            var correlationId = "CollerationId";
//        //            var token = "1";
//        //            var emailPreference = EmailPreference.Any;
//        //            var accountDBServiceMock = serviceProvider.GetMock<IAccountDBService>();

//        //            accountDBServiceMock
//        //            .Setup
//        //            (
//        //                accountDBService => accountDBService.UpdateEmailPreferencesWithTokenAsync
//        //                (
//        //                    It.IsAny<UpdateEmailPreferencesWithTokenRequest>(),
//        //                    It.IsAny<string>()
//        //                )
//        //            )
//        //            .ReturnsAsync
//        //            (
//        //                new UpdateEmailPreferencesWithTokenDBResult
//        //                {
//        //                    TokenFound = true,
//        //                    TokenExpired = false
//        //                }
//        //            );

//        //            //Act
//        //            var observed = await uutConcrete.UpdateEmailSettingsAsync(token, emailPreference, correlationId);

//        //            //Assert
//        //            Assert.AreEqual(UpdateEmailSettingsResult.Successful, observed);
//        //        },
//        //        serviceCollection => ConfigureServices(serviceCollection)
//        //    );
//        //}

//        #endregion

//        #region Helpers
//        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
//        {
//            serviceCollection.AddSingleton<IAccountManager, AccountManager>();
//            serviceCollection.AddSingleton(Mock.Of<IAccountDBService>());
//            serviceCollection.AddSingleton(Mock.Of<IEncryptionService>());
//            serviceCollection.AddSingleton(Mock.Of<IEmailService>());
//            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
//            return serviceCollection;
//        }
//        #endregion
//    }
//}
