using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using SaintSender.UI.ViewModels;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
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

            SendCommand = new RelayCommand((obj) => {
                var reg = new Regex(@"^\w+@(\w.)+[a-z]{2,3}$");
                if (!reg.IsMatch(EmailVM.Email.Receiver))
                {
                    MessageBox.Show("Wrong email address.");
                    return;
                }
                mr.SendEmail(EmailVM.Email);
                Close();
            });

            CloseCommand = new RelayCommand((obj) =>
            {
                var confirmWindow = new DialogWindow(this);
                confirmWindow.Show();
                
            });

            InitializeComponent();
            DataContext = this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            
        }
    }
}
