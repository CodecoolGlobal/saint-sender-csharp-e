using Models.Logic;
using Models.Models;
using SaintSender.UI.Utils;
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
        public MailModel email { get; set; }
        public SendEmailWindow()
        {
            var mr = new MailRepository();
            SendCommand = new RelayCommand((obj) => mr.SendEmail(email));
            InitializeComponent();
            DataContext = this;
        }
    }
}
