﻿using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using SaintSender.Backend.Models;

namespace SaintSender.Backend.Logic
{
    public class MailRepository
    {
        private readonly string login, password;
        private readonly string mailServer = "imap.gmail.com";
        private readonly int port = 993;
        private readonly bool ssl = true;
        public bool IsLoginCredentialCorrect { get; set; } = false;

        public MailRepository()
        {
            this.login = "sunyibela@gmail.com";
            this.password = "6HcZbP9Zh439D4n";
        }

        public MailRepository(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        public MailRepository(string mailServer, int port, bool ssl, string login, string password)
        {
            this.mailServer = mailServer;
            this.port = port;
            this.ssl = ssl;
            this.login = login;
            this.password = password;
        }

        public bool AreCredentialsCorrect
        {
            get
            {
                bool correctLogin;
                using (var client = new ImapClient())
                {
                    client.Connect(mailServer, port, ssl);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    try
                    {
                        client.Authenticate(login, password);
                        correctLogin = true;
                    }
                    catch (ServiceNotAuthenticatedException)
                    {
                        correctLogin = false;
                    }
                }
                return correctLogin;
            }
        }

        public IEnumerable<MailModel> GetUnreadMails()
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
                    var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
                    foreach (var uniqueId in results.UniqueIds)
                    {
                        MimeMessage t = inbox.GetMessage(uniqueId);

                        var mail = new MailModel(t);

                        mails.Add(mail);

                        //Mark message as read
                        //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                    }
                }
                catch
                {

                }
                finally
                {
                    client.Disconnect(true);
                }

            }

            return mails;
        }

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
                    MimeMessage t = inbox.GetMessage(uniqueId);

                    var mail = new MailModel(t);

                    mails.Add(mail);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return mails;
        }
    }
}
