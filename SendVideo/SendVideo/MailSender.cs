using SendVideo.Configuration;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SendVideo
{
    public class MailSender
    {
        private readonly SmtpServer server;

        public MailSender(SmtpServer server)
        {
            this.server = server;
        }

        public async Task SendVideo(FileInfo video, Recipient recipient, string body)
        {
            using (var client = new SmtpClient(server.Address, server.Port))
            {
                client.EnableSsl = server.UseSsl;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(server.Username, server.Password);
                client.SendCompleted += OnSendCompleted;

                using (var attachment = new Attachment(video.FullName, recipient.ContentType))
                {
                    attachment.Name = recipient.AttachmentName;
                    using (var message = new MailMessage(server.From, recipient.Address))
                    {
                        message.Subject = server.Subject ?? string.Empty;
                        message.Body = body ?? string.Empty;
                        message.Attachments.Add(attachment);
                        await this.SendInternalAsync(client, message);
                    }
                }
            }
        }

        private Task SendInternalAsync(SmtpClient client, MailMessage message)
        {
            var tcs = new TaskCompletionSource<bool>();
            client.SendAsync(message, tcs);
            return tcs.Task;
        }

        private void OnSendCompleted(object sender, AsyncCompletedEventArgs args)
        {
            var tcs = (TaskCompletionSource<bool>)args.UserState;
            if (args.Cancelled)
            {
                tcs.TrySetCanceled();
            }
            else if (args.Error != null)
            {
                tcs.TrySetException(args.Error);
            }
            else
            {
                tcs.TrySetResult(true);
            }
        }
    }
}
