using DickinsonBros.AccountAPI.Infrastructure.AccountDB.Models;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Options;
using DickinsonBros.AccountAPI.Contracts;
using DickinsonBros.AccountAPI.Infrastructure.Sql;
using DickinsonBros.AccountAPI.Infrastructure.Encryption;

namespace DickinsonBros.AccountAPI.Infrastructure.AccountDB
{
    public class AccountDBService : IAccountDBService
    {
        internal readonly string _dickinsonBrosDBConnectionString;
        internal readonly ISQLService _sqlService;

        internal const string SELECT_ACCOUNTID_BY_PASSWORD_RESET_TOKEN =    "[Account].[SelectAccountIdByPasswordResetToken]";
        internal const string SELECT_ACCOUNT_BY_ACCOUNT_ID =                "[Account].[SelectAccountByAccountId]";
        internal const string SELECT_ACCOUNT_BY_USERNAME =                  "[Account].[SelectAccountByUsername]";
        internal const string SELECT_ACCOUNT_BY_EMAIL =                     "[Account].[SelectAccountByEmail]";
        internal const string INSERT_ACCOUNT =                              "[Account].[Insert]";
        internal const string INSERT_PASSWORD_RESET_TOKEN =                 "[Account].[InsertPasswordResetToken]";
        internal const string INSERT_PASSWORD_ATTEMPT_FAILED =              "[Account].[InsertPasswordAttemptFailed]";
        internal const string UPDATE_PASSWORD =                             "[Account].[UpdatePassword]";
        internal const string UPDATE_EMAIL_PREFERENCES =                    "[Account].[UpdateEmailPreference]";
        internal const string UPDATE_EMAIL_PREFERENCES_WITH_TOKEN =         "[Account].[UpdateEmailPreferenceWithToken]";
        internal const string UPDATE_EMAIL_ACTIVE_WITH_TOKEN =              "[Account].[UpdateEmailActiveWithToken]";
        internal const string DELETE_PASSWORD_RESET_TOKEN =                 "[Account].[DeletePasswordResetToken]";

        public AccountDBService(IOptions<DickinsonBrosDB> dickinsonBrosDB, IEncryptionService encryptionService, ISQLService sqlService)
        {
            _dickinsonBrosDBConnectionString = encryptionService.Decrypt(dickinsonBrosDB.Value.ConnectionString);
            _sqlService = sqlService;
        }

        public async Task<InsertAccountResult> InsertAccountAsync(InsertAccountRequest insertAccountRequest)
        {
            return await _sqlService
                        .QueryFirstAsync<InsertAccountResult>
                         (
                             _dickinsonBrosDBConnectionString,
                             INSERT_ACCOUNT,
                             insertAccountRequest,
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task<Account> SelectAccountByUserNameAsync(string username)
        {
            return await _sqlService
                        .QueryFirstOrDefaultAsync<Account>
                         (
                             _dickinsonBrosDBConnectionString,
                             SELECT_ACCOUNT_BY_USERNAME,
                             new
                             {
                                 Username = username
                             },
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task<Account> SelectAccountByEmailAsync(string email)
        {
            return await _sqlService
                         .QueryFirstOrDefaultAsync<Account>
                         (
                             _dickinsonBrosDBConnectionString,
                             SELECT_ACCOUNT_BY_EMAIL,
                             new
                             {
                                 Email = email
                             },
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task<Account> SelectAccountByAccountIdAsync(int accountId)
        {
            return await _sqlService
                         .QueryFirstOrDefaultAsync<Account>
                          (
                              _dickinsonBrosDBConnectionString,
                              SELECT_ACCOUNT_BY_ACCOUNT_ID,
                              new
                              {
                                  AccountId = accountId
                              },
                              commandType: CommandType.StoredProcedure
                          );
        }

        public async Task InsertPasswordResetTokenAsync(int accountId, string PasswordResetToken)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _dickinsonBrosDBConnectionString,
                      INSERT_PASSWORD_RESET_TOKEN,
                      new
                      {
                          accountId,
                          PasswordResetToken
                      },
                      commandType: CommandType.StoredProcedure
                  );
        }

        public async Task<int?> SelectAccountIdFromPasswordResetTokenAsync(string passwordResetToken)
        {
            return await _sqlService
                         .QueryFirstOrDefaultAsync<int?>
                         (
                             _dickinsonBrosDBConnectionString,
                             SELECT_ACCOUNTID_BY_PASSWORD_RESET_TOKEN,
                             new
                             {
                                 PasswordResetToken = passwordResetToken
                             },
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task DisablePasswordResetTokenAsync(string passwordResetToken)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _dickinsonBrosDBConnectionString,
                      DELETE_PASSWORD_RESET_TOKEN,
                      new
                      {
                          PasswordResetToken = passwordResetToken
                      },
                      commandType: CommandType.StoredProcedure
                  );
        }

        public async Task UpdatePasswordAsync(int accountId, string passwordHash, string salt)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _dickinsonBrosDBConnectionString,
                      UPDATE_PASSWORD,
                      new
                      {
                          accountId,
                          passwordHash,
                          salt
                      },
                      commandType: CommandType.StoredProcedure
                  );
        }



        public async Task UpdateEmailPreferenceAsync(int accountId, EmailPreference emailPreference)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _dickinsonBrosDBConnectionString,
                      UPDATE_EMAIL_PREFERENCES,
                      new
                      {
                          AccountId = accountId,
                          EmailPreference = emailPreference
                      },
                      commandType: CommandType.StoredProcedure
                  );
        }

        public async Task<UpdateEmailPreferenceWithTokenDBResult> UpdateEmailPreferenceWithTokenAsync(string emailPreferenceToken, EmailPreference emailPreference)
        {
            return await _sqlService
               .QueryFirstAsync<UpdateEmailPreferenceWithTokenDBResult>
                   (
                       _dickinsonBrosDBConnectionString,
                       UPDATE_EMAIL_PREFERENCES_WITH_TOKEN,
                       new
                       {
                           emailPreferenceToken,
                           emailPreference
                       },
                       commandType: CommandType.StoredProcedure
                   );
        }

        public async Task<ActivateEmailWithTokenDBResult> ActivateEmailWithTokenAsync(string activateEmailToken)
        {
            return await _sqlService
              .QueryFirstAsync<ActivateEmailWithTokenDBResult>
              (
                  _dickinsonBrosDBConnectionString,
                  UPDATE_EMAIL_ACTIVE_WITH_TOKEN,
                  new
                  {
                      ActivateEmailToken = activateEmailToken
                  },
                  commandType: CommandType.StoredProcedure
              );
        }

        public async Task InsertPasswordAttemptFailedAsync(int accountId)
        {
            await _sqlService
               .ExecuteAsync
               (
                   _dickinsonBrosDBConnectionString,
                   INSERT_PASSWORD_ATTEMPT_FAILED,
                   new
                   {
                       AccountId = accountId
                   },
                   commandType: CommandType.StoredProcedure
               );
        }
    }
}
