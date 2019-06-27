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
        InboxViewModel(MailModel mail)
        {
            Mail = mail;
        }

        public MailModel Mail { get; private set; }

        public DateTime Date
        {
            get
            {
                return Mail.Date;
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
