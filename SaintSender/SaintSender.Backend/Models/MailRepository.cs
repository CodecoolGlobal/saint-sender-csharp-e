using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Net.Mail;
using System.Net;

namespace SaintSender.Backend.Models
{
    public class MailRepository
    {
        public readonly string login;
        private readonly string password;
        private readonly string mailServer = "imap.gmail.com";
        private readonly int port = 993;
        private readonly bool ssl = true;

        /// <summary>
        /// Initializes a new instance of the MailRepository with hardcoded user for testing
        /// </summary>
        public MailRepository()
        {
            this.login = "sunyibela@gmail.com";
            this.password = "6HcZbP9Zh439D4n";
        }

        /// <summary>
        /// Initializes a new instance of the MailRepository with user information
        /// </summary>
        /// <param name="login">Email of user</param>
        /// <param name="password">Password of user (non hashed)</param>
        public MailRepository(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        /// <summary>
        /// Initializes a new instance of the MailRepository with all customization options
        /// </summary>
        /// <param name="mailServer">Mail server to connect to</param>
        /// <param name="port">Port of the mail server</param>
        /// <param name="ssl">SSL state</param>
        /// <param name="login">Email of user</param>
        /// <param name="password">Password of user (non hashed)</param>
        public MailRepository(string mailServer, int port, bool ssl, string login, string password)
        {
            this.mailServer = mailServer;
            this.port = port;
            this.ssl = ssl;
            this.login = login;
            this.password = password;
        }

        /// <summary>
        /// Gets all mail as an enumerable collection of MailModel
        /// </summary>
        /// <returns>Enumerable collection of MailModels</returns>
        public IEnumerable<MailModel> GetAllMails()
        {
            var mails = new List<MailModel>();

            using (var client = new ImapClient())
            {
                client.Connect(mailServer, port, ssl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(login, password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                var results = inbox.Search(SearchOptions.All, SearchQuery.All);
                foreach (var uniqueId in results.UniqueIds)
                {
                    var mail = new MailModel();

                    MimeMessage t = inbox.GetMessage(uniqueId);

                    mail.Message = t.HtmlBody;
                    mail.Subject = t.Subject;
                    mail.Sender = t.From.Mailboxes.First().Address;
                    mail.Date = t.Date.DateTime;
                    
                    mails.Add(mail);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return mails;
        }

        public void SendEmail(MailModel email)
        {
            var fromAddress = new MailAddress(login, login.Split('@')[0]);
            var toAddress = new MailAddress(email.Receiver, email.Receiver.Split('@')[0]);
            string fromPassword = password;
            string subject = email.Subject;
            string body = email.Message;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public static string Encode(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }
}
