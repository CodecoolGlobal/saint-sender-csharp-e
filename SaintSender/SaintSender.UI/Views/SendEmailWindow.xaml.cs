using SaintSender.Backend.Logic;
using SaintSender.UI.Utils;
using SaintSender.UI.ViewModels;
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
        public SendEmailWindow()
        {
            var mr = new MailRepository();

            SendCommand = new RelayCommand((obj) => {
                mr.SendEmail(EmailVM.Email);
                Close();
            });

            CloseCommand = new RelayCommand((obj) => Close());

            InitializeComponent();
            DataContext = this;
        }
    }
}
