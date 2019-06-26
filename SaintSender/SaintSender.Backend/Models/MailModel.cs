﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SaintSender.Backend.Models
{
    [Serializable]
    public class MailModel
    {
        /// <summary>
        /// Gets and sets the subject of the email
        /// </summary>
        public string Subject { get; set; } = "[Error] Subject didn't load";

        /// <summary>
        /// Gets and sets the main body of the email
        /// </summary>
        public string Message { get; set; } = "[Error] Message didn't load";

        /// <summary>
        /// Gets and sets the email of the sender
        /// </summary>
        public string Sender { get; set; } = "[Error] Sender didn't load";

        /// <summary>
        /// Gets and sets when the email was sent
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets and sets whether the email is marked as read or not
        /// </summary>
        public bool Read { get; set; } = false;

        /// <summary>
        /// Initializes a new empty instance of the MailModel
        /// </summary>
        public MailModel() { }

        /// <summary>
        /// Initializes a new instance of the MailModel with parameters
        /// </summary>
        /// <param name="subject">Subject of the email</param>
        /// <param name="message">Main body of the email</param>
        /// <param name="sender">Sender's email address</param>
        /// <param name="date">Date the email was sent</param>
        public MailModel(string subject, string message, string sender, DateTime date)
        {
            Subject = subject;
            Message = message;
            Sender = sender;
            Date = date;
        }

        [NonSerialized]
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/SaintSender";

        /// <summary>
        /// Stores email to isolated storage
        /// </summary>
        /// <param name="user">String containing the beginning of email address example: "name" from "name@a.b"</param>
        public void Save(string user)
        {
            if (!Directory.Exists(path + $@"/mails/{user}"))
            {
                Directory.CreateDirectory(path + $@"/mails/{user}");
            }
            string fileName = $@"{path}/mails/{user}/{Date.ToBinary()}";
            using (FileStream fileStream = File.Create(fileName))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, this);
            }
        }
    }
}
