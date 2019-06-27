using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using SaintSender.UI.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SaintSender.UI.Views
{
    /// <summary>
    /// Interaction logic for SendEmailWindow.xaml
    /// </summary>
    public partial class SendEmailWindow : Window
    {
        public ICommand SendCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public SendMailViewModel EmailVM { get; set; } = new SendMailViewModel();
        public SendEmailWindow(MailRepository mr)
        {
            if (mr == null)
            {
                MessageBox.Show("Please login first!");
                return;
            }

            SendCommand = new RelayCommand(Send(mr));

            CloseCommand = new RelayCommand(Exit());

            InitializeComponent();
            DataContext = this;
        }

        private System.Action<object> Exit()
        {
            return (obj) =>
            {
                if (EmailVM.Message.Length > 0 || EmailVM.Subject.Length > 0 || EmailVM.Receiver.Length > 0)
                {
                    var confirmWindow = new DialogWindow(this);
                    confirmWindow.Show();
                }
                else
                {
                    Close();
                }
            };
        }

        private System.Action<object> Send(MailRepository mr)
        {
            return (obj) =>
            {
                var reg = new Regex(@"^\w+@(\w.)+[a-z]{2,3}$");
                if (!reg.IsMatch(EmailVM.Email.Receiver))
                {
                    MessageBox.Show("Wrong email address.");
                    return;
                }
                mr.SendEmail(EmailVM.Email);
                Close();
            };
        }
    }
}
