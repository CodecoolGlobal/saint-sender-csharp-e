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
using MailKit.Security;
using System.Net.Sockets;
using SaintSender.UI.Utils;

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
        /// Gets all email as an enumerable collection of MailModel
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

                try
                {
                    client.Authenticate(login, password);

                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);
                    var results = inbox.Search(SearchQuery.All);
                    foreach (var uniqueId in results)
                    {
                        MimeMessage t = inbox.GetMessage(uniqueId);

                        var mail = new MailModel(t, uniqueId);

                        mails.Add(mail);

                        //Mark message as read
                        //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                    }

                    client.Disconnect(true);
                }
                catch (AuthenticationException)
                {
                    AreCredentialsCorrect = false;
                }
            }

            return mails;
        }

        /// <summary>
        /// Gets latest <paramref name="count"/> number of emails as acollection of MailModel
        /// </summary>
        /// <param name="count">Nubmber of emails to return.</param>
        /// <returns>A collection of MailModels</returns>
        public IEnumerable<MailModel> GetLastMails(int count = 10)
        {
            var mails = new List<MailModel>();
            
            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect(mailServer, port, ssl);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(login, password);

                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);
                    var results = inbox.Search(SearchQuery.All).Reverse().Take(count);
                    foreach (var uniqueId in results)
                    {

                        MimeMessage t = inbox.GetMessage(uniqueId);


                        var mail = new MailModel(t, uniqueId);

                        mails.Add(mail);

                        //Mark message as read
                        //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                    }
                    client.Disconnect(true);
                }
                catch (AuthenticationException)
                {
                    AreCredentialsCorrect = false;
                }
                catch (SocketException)
                {
                    throw new NoInternetConnectionException();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return mails;
        }

        /// <summary>
        /// Last known state of login credentials. Set by <see cref="CheckCredentials"/>.
        /// </summary>
        public bool AreCredentialsCorrect { get; private set; } = false;

        /// <summary>
        /// Checks whether the current credentials are correct.
        /// </summary>
        /// <returns>true if the server properly authenticated the credentials.</returns>
        public bool CheckCredentials()
        {
            bool correct;
            {
                using (var client = new ImapClient())
                {
                    client.Connect(mailServer, port, ssl);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    try
                    {
                        client.Authenticate(login, password);
                        correct = true;
                    }
                    catch (AuthenticationException)
                    {
                        correct = false;
                    }
                }
            }
            AreCredentialsCorrect = correct;
            return correct;
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
