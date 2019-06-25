﻿using System;

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
    }
}
