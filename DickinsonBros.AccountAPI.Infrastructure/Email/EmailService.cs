using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using System;
using MimeKit.IO;

namespace DickinsonBros.AccountAPI.Infrastructure.EmailSender
{
    public class EmailService : IEmailService
    {
        public async Task SendActivateEmailAsync(string email, string activateToken, string updateEmailSettingsToken, string username)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Roller Coaster Maker", "NoReply@RollerCoasterMaker.com"));
            message.To.Add(new MailboxAddress(email));
            message.Subject = "Roller Coaster Maker - Activate Email";

            message.Body = new TextPart("plain")
            {
                Text = $@"ActivateEmail 
                        Username: {username}
                        ActivateToken: {activateToken}
                        UpdateEmailSettingsToken: {updateEmailSettingsToken}"
            };

           await SaveToPickupDirectoryAsync(message);
        }

        public async Task SendPasswordResetEmailAsync(string email, string passwordResetToken, Guid emailPreferenceToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Roller Coaster Maker", "NoReply@RollerCoasterMaker.com"));
            message.To.Add(new MailboxAddress(email));
            message.Subject = "Roller Coaster Maker - Reset Password";

            message.Body = new TextPart("plain")
            {
                Text = $@"Reset Password
                        PasswordResetToken: {passwordResetToken}
                        UpdateEmailSettingsToken: {emailPreferenceToken}"
            };

            await SaveToPickupDirectoryAsync(message);
        }

        internal async Task SaveToPickupDirectoryAsync(MimeMessage message)
        {
            var path = Path.Combine("C:/Emails", Guid.NewGuid().ToString() + ".eml");
            Stream stream;

            stream = File.Open(path, FileMode.CreateNew);
            using (stream)
            {
                using (var filtered = new FilteredStream(stream))
                {
                    filtered.Add(new SmtpDataFilter());

                    var options = FormatOptions.Default.Clone();
                    options.NewLineFormat = NewLineFormat.Dos;

                    await message.WriteToAsync(options, filtered);
                    await filtered.FlushAsync();
                    return;
                }
            }
        }
    }


}
