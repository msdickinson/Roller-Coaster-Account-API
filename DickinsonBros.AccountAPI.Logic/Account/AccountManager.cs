using DickinsonBros.AccountAPI.Contracts;
using DickinsonBros.AccountAPI.Infrastructure.AccountDB;
using DickinsonBros.AccountAPI.Infrastructure.AccountDB.Models;
using DickinsonBros.AccountAPI.Infrastructure.DateTime;
using DickinsonBros.AccountAPI.Infrastructure.EmailSender;
using DickinsonBros.AccountAPI.Infrastructure.PasswordEncryption;
using DickinsonBros.AccountAPI.Logic.Account.Models;
using System.Threading.Tasks;

namespace DickinsonBros.AccountAPI.Logic.Account
{
    public class AccountManager : IAccountManager
    {
        internal readonly IAccountDBService _accountDBService;
        internal readonly IPasswordEncryptionService _passwordEncryptionService;
        internal readonly IEmailService _emailService;
        internal readonly IGuidService _guidService;

        public AccountManager
        (
            IAccountDBService accountDBService,
            IPasswordEncryptionService passwordEncryptionService,
            IEmailService emailService,
            IGuidService guidService
        )
        {
            _accountDBService = accountDBService;
            _passwordEncryptionService = passwordEncryptionService;
            _emailService = emailService;
            _guidService = guidService;
        }

        public async Task<LoginDescriptor> LoginAsync(string username, string password)
        {
            var loginDescriptor = new LoginDescriptor();

            var account = await _accountDBService.SelectAccountByUserNameAsync(username);

            if (account == null)
            {
                loginDescriptor.Result = LoginResult.AccountNotFound;
                return loginDescriptor;
            }

            if (account.Locked)
            {
                loginDescriptor.Result = LoginResult.AccountLocked;
                return loginDescriptor;
            }

            var encryptResult = _passwordEncryptionService.Encrypt(password, account.Salt);

            if (account.PasswordHash != encryptResult.Hash)
            {
                loginDescriptor.Result = LoginResult.InvaildPassword;
                return loginDescriptor;
            }

            loginDescriptor.Result = LoginResult.Successful;
            loginDescriptor.AccountId = account.AccountId;

            return loginDescriptor;
        }

        public async Task<CreateAccountDescriptor> CreateAsync(string username, string password, string email)
        {
            var createAccountDescriptor = new CreateAccountDescriptor();

            var encryptResult = _passwordEncryptionService.Encrypt(password);

            var activateEmailToken = _guidService.NewGuid().ToString();
            var emailPreferenceToken = _guidService.NewGuid().ToString();

            var insertAccountResult = await 
                _accountDBService.InsertAccountAsync
                (
                    new InsertAccountRequest
                    {
                        Username = username,
                        PasswordHash = encryptResult.Hash,
                        Salt = encryptResult.Salt,
                        ActivateEmailToken = activateEmailToken,
                        EmailPreferenceToken = emailPreferenceToken,
                        Email = email,
                        EmailPreference = EmailPreference.Any

                    }
               );

            if (insertAccountResult.DuplicateUser)
            {
                createAccountDescriptor.Result = CreateAccountResult.DuplicateUser;
                return createAccountDescriptor;
            }

            if(email != null)
            {
                await _emailService.SendActivateEmailAsync(email, username, activateEmailToken, emailPreferenceToken);
            }

            //Service Bus, Or Que To N Services, Or call N Services., Or Single Que that will call all the services.

            createAccountDescriptor.Result = CreateAccountResult.Successful;
            createAccountDescriptor.AccountId = insertAccountResult.AccountId;

            return createAccountDescriptor;
        }

        public async Task<RequestPasswordResetEmailResult> RequestPasswordResetEmailAsync(string email)
        {
            var account = await _accountDBService.SelectAccountByEmailAsync(email);

            if (account == null)
            {
                return RequestPasswordResetEmailResult.EmailNotFound;
            }

            if(!account.EmailActivated)
            {
                return RequestPasswordResetEmailResult.EmailNotActivated;
            }

            if(account.EmailPreference == EmailPreference.None)
            {
                return RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference;
            }

            string passwordResetToken = _guidService.NewGuid().ToString();
            await _accountDBService.InsertPasswordResetTokenAsync(account.AccountId, passwordResetToken);

            await _emailService.SendPasswordResetEmailAsync(account.Email, passwordResetToken, account.EmailPreferenceToken);
            return RequestPasswordResetEmailResult.Successful;
        }

        public async Task<UpdatePasswordResult> UpdatePasswordAsync(int accountId, string existingPassword, string newPassword)
        {
            var account = await _accountDBService.SelectAccountByAccountIdAsync(accountId);

            if (account.Locked)
            {
                return UpdatePasswordResult.AccountLocked;
            }

            var encryptResult = _passwordEncryptionService.Encrypt(existingPassword, account.Salt);

            if(account.PasswordHash != encryptResult.Hash)
            {
                await _accountDBService.InsertPasswordAttemptFailedAsync(accountId);

                return UpdatePasswordResult.InvaildExistingPassword;
            }

            var newEncryptResult = _passwordEncryptionService.Encrypt(newPassword);

            await _accountDBService.UpdatePasswordAsync(account.AccountId, newEncryptResult.Hash, newEncryptResult.Salt);

            return UpdatePasswordResult.Successful;
        }

        public async Task<ResetPasswordResult> ResetPasswordAsync(string token, string newPassword)
        {
            var accountId = await _accountDBService.SelectAccountIdFromPasswordResetTokenAsync(token);

            if (accountId == null)
            {
                return ResetPasswordResult.TokenInvaild;
            }

            var newEncryptResult = _passwordEncryptionService.Encrypt(newPassword);

            await _accountDBService.UpdatePasswordAsync((int)accountId, newEncryptResult.Hash, newEncryptResult.Salt);

            return ResetPasswordResult.Successful;
        }

        public async Task<UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(string emailPreferenceToken, EmailPreference emailPreference)
        {
            var updateEmailPreferenceWithTokenResult = await _accountDBService.UpdateEmailPreferenceWithTokenAsync(emailPreferenceToken, emailPreference);

            if(!updateEmailPreferenceWithTokenResult.VaildToken)
            {
                return UpdateEmailPreferenceWithTokenResult.InvaildToken;
            }

            return UpdateEmailPreferenceWithTokenResult.Successful;
        }

        public async Task UpdateEmailPreferenceAsync(int accountId, EmailPreference emailPreference)
        {
            await _accountDBService.UpdateEmailPreferenceAsync(accountId, emailPreference);
        }

        public async Task<ActivateEmailResult> ActivateEmailAsync(string token)
        {
            var activateEmailWithTokenResult = await _accountDBService.ActivateEmailWithTokenAsync(token);

            if (!activateEmailWithTokenResult.VaildToken)
            {
                return ActivateEmailResult.InvaildToken;
            }

            if (activateEmailWithTokenResult.EmailWasAlreadyActivated)
            {
                return ActivateEmailResult.EmailWasAlreadyActivated;
            }

            return ActivateEmailResult.Successful;
        }
    }
}
