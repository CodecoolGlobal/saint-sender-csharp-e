using MimeKit;
using System;
using System.Linq;

namespace SaintSender.Backend.Models
{
    public class MailModel
    {
        public string Subject { get; set; } = "[Error] Subject didn't load";
        public string Message { get; set; } = "[Error] Message didn't load";
        public string Sender { get; set; } = "[Error] Sender didn't load";
        public DateTime Date { get; set; }
        public bool Read { get; set; } = false;

        public MailModel() { }

        public MailModel(string subject, string message, string sender, DateTime date)
        {
            Subject = subject;
            Message = message;
            Sender = sender;
            Date = date;
        }

        public MailModel(MimeMessage mimeMessage)
        {
            Message = mimeMessage.HtmlBody;
            Subject = mimeMessage.Subject;
            Sender = mimeMessage.From.Mailboxes.First().Address;
            Date = mimeMessage.Date.DateTime;
        }
    }
}
