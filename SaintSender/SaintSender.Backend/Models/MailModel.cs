using MailKit;
using MimeKit;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace SaintSender.Backend.Models
{
    [Serializable]
    public class MailModel
    {
        /// <summary>
        /// Gets and sets the subject of the email
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets and sets the main body of the email
        /// </summary>
        public string Message { get; set; } = "<h1>Welcome to SaintSender!</h1>";

        /// <summary>
        /// Gets and sets the email of the sender
        /// </summary>
        public string Sender { get; set; } = string.Empty;

        /// <summary>
        /// Gets and sets when the email was sent
        /// </summary>
        public string Receiver { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets and sets whether the email is marked as read or not
        /// </summary>
        public bool Read { get; set; } = false;
        /// <summary>
        /// Gets and sets the unique id of the email that is used for comparing two emails.
        /// </summary>
        public UniqueId ID { get => iD; set => iD = value; }

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

        public MailModel(MimeMessage mimeMessage, UniqueId id)
        {
            if(mimeMessage.HtmlBody != null)
            {
                Message = mimeMessage.HtmlBody;
            }
            else
            {
                //Message = "testa";
                Message = ((TextPart)mimeMessage.Body).Text;
            }
            Subject = mimeMessage.Subject;
            Sender = mimeMessage.From.Mailboxes.First().Address;
            Date = mimeMessage.Date.DateTime;
            ID = id;
        }

        public MailModel(MailModel model)
        {
            Message = model.Message;
            Subject = model.Subject;
            Sender = model.Sender;
            Date = model.Date;
            ID = model.ID;
        }

        public override bool Equals(object other)
        {
            if (!(other is MailModel))
            {
                return false;
            }

            if (ID == ((MailModel)other).ID)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ID.ToString().GetHashCode();
        }

        [NonSerialized]
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/SaintSender";
        [NonSerialized]
        private UniqueId iD;

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

        public MailModel Copy()
        {
            return new MailModel(this);
        }
    }
}
