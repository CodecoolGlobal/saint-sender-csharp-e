using System;
using System.Collections.Generic;

namespace Models.Models
{
    public class MailModel
    {
        public string Subject { get; set; } = "[Error] Subject didn't load";
        public string Message { get; set; } = "[Error] Message didn't load";
        public string Sender { get; set; } = "[Error] Sender didn't load";
        public List<string> Receivers { get; } = new List<string>();
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

        public MailModel(string subject, string message, string sender, DateTime date, List<string> receivers) : this(subject, message, sender, date)
        {
            Receivers.AddRange(receivers);
        }
    }
}
