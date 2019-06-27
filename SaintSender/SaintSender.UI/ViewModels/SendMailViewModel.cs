using SaintSender.Backend.Models;

namespace SaintSender.UI.ViewModels
{
    public class SendMailViewModel : ViewModelBase
    {
        public MailModel Email { get; } = new MailModel();
        public SendMailViewModel()
        {
            Email.Message = "";
            Email.Subject = "";
        }
        public string Message
        {
            get => Email.Message; set
            {
                Email.Message = value;
                OnPropertyChanged();
            }
        }

        public string Receiver
        {
            get => Email.Receiver; set
            {
                Email.Receiver = value;
                OnPropertyChanged();
            }
        }

        public string Subject
        {
            get => Email.Subject; set
            {
                Email.Subject = value;
                OnPropertyChanged();
            }
        }
    }
}
