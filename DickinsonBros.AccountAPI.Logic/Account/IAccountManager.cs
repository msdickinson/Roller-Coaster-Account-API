using System.Threading.Tasks;
using DickinsonBros.AccountAPI.Contracts;
using DickinsonBros.AccountAPI.Logic.Account.Models;

namespace DickinsonBros.AccountAPI.Logic.Account
{
    public interface IAccountManager
    {
        Task<CreateAccountDescriptor> CreateAsync(string username, string password, string email);
        Task<LoginDescriptor> LoginAsync(string username, string password);
        Task<RequestPasswordResetEmailResult> RequestPasswordResetEmailAsync(string email);
        Task<UpdatePasswordResult> UpdatePasswordAsync(int accountId, string existingPassword, string newPassword);
        Task<ResetPasswordResult> ResetPasswordAsync(string token, string newPassword);
        Task<UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(string token, EmailPreference emailPreference);
        Task UpdateEmailPreferenceAsync(int userId, EmailPreference emailPreference);
        Task<ActivateEmailResult> ActivateEmailAsync(string token);
    }
}