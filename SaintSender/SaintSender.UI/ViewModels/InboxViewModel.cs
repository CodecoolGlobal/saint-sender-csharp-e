using SaintSender.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaintSender.UI.ViewModels
{
    class InboxViewModel
    {
        public InboxViewModel(MailModel mail)
        {
            Mail = mail;
        }

        public MailModel Mail { get; private set; }

        public string Date
        {
            get
            {
                TimeSpan timeSpan = DateTime.Today - Mail.Date;
                int elapsedDays = DateTime.Today.DayOfYear - Mail.Date.DayOfYear;
                if (elapsedDays == 0)
                {
                    return "Today";
                }
                if (elapsedDays == 1)
                {
                    return "Yesterday";
                }
                if (elapsedDays == 2)
                {
                    return "Two days ago";
                }
                else if (elapsedDays < 8)
                {
                    return "This week";
                }
                return Mail.Date.ToShortDateString();
            }
        }
        public string Subject
        {
            get
            {
                return Mail.Subject;
            }
        }
        public string Sender
        {
            get
            {
                return Mail.Sender;
            }
        }

        
    }
}
