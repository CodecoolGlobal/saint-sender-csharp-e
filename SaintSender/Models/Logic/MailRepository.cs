﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Models.Models;
using static Google.Apis.Gmail.v1.GmailService;

namespace Models.Logic
{
    public class MailRepository
    {
        private readonly string mailServer, login, password, clientSecret;
        private readonly int port;
        private readonly bool ssl;

        public MailRepository()
        {
            this.mailServer = "imap.gmail.com";
            this.login = "sunyibela@gmail.com";
            this.password = "6HcZbP9Zh439D4n";
            this.clientSecret = "3tIIhaadXBvPHmPG__PdrWTj";
            this.port = 993;
            this.ssl = true;
        }

        public MailRepository(string mailServer, int port, bool ssl, string login, string password)
        {
            this.mailServer = mailServer;
            this.port = port;
            this.ssl = ssl;
            this.login = login;
            this.password = password;
        }

        public IEnumerable<string> GetUnreadMails()
        {
            var messages = new List<string>();

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
                var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message.HtmlBody);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }

        public IEnumerable<string> GetAllMails()
        {
            var messages = new List<string>();

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
                var results = inbox.Search(SearchOptions.All, SearchQuery.NotSeen);
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message.HtmlBody);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }

        public void SendEmail(MailModel email)
        {
            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(email.Sender);
            mailMessage.To.Add(email.GetReceiversString());
            mailMessage.ReplyToList.Add(email.GetReceiversString());
            mailMessage.Subject = email.Subject;
            mailMessage.Body = email.Message;
            mailMessage.IsBodyHtml = false;

            foreach (System.Net.Mail.Attachment attachment in email.Attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }

            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);

            Message message = new Message();
            message.Raw = Encode(mimeMessage.ToString());
            UserCredential credential;

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(clientSecret)))
            {
                string credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @".credentials\gmail-dotnet-quickstart.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new string[] { Scope.GmailSend },
                    "sunyibela@gmail.com",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SaintSender"
            });
            
            
            var request = service.Users.Messages.Send(message, mimeMessage.To.ToString());
            request.Execute();
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
