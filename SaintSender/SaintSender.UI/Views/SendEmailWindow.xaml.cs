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
        #region ICommands
        /// <summary>
        /// Command that sends the newly created email.
        /// </summary>
        public ICommand SendCommand { get; set; }
        /// <summary>
        /// Command that closes the window.
        /// </summary>
        public ICommand CloseCommand { get; set; }
        #endregion

        /// <summary>
        /// Connects MailModel and this window.
        /// </summary>
        public SendMailViewModel EmailVM { get; set; } = new SendMailViewModel();

        /// <summary>
        /// Creates and instance of this object.
        /// </summary>
        /// <param name="mr">MailRepository that is defined in MainWindow.cs</param>
        public SendEmailWindow(MailRepository mr)
        {
            // mr nullcheck
            if (mr == null)
            {
                MessageBox.Show("Please login first!");
                return;
            }

            // Commands
            SendCommand = new RelayCommand((obj) => Send(mr));
            CloseCommand = new RelayCommand((obj) => Exit());

            InitializeComponent();
            DataContext = this;
        }

        #region Command Methods

        /// <summary>
        /// Checks if the user typed anything in the fields. If they did, a confirmation window appears
        /// asking the user if they want to close this window while loosing all they typed or not.
        /// If the user didn't type anything then closes window.
        /// </summary>
        private void Exit()
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
        }

        /// <summary>
        /// Checks if given email address is valid. If it is not a Messagebox informs the user to type in a valid one.
        /// If it is then sends the Email with the given message and subject to the given email address.
        /// </summary>
        /// <param name="mr">Mail repository that contains the users credentials.</param>
        private void Send(MailRepository mr)
        {
            var reg = new Regex(@"^\w+@(\w.)+[a-z]{2,3}$");
            if (!reg.IsMatch(EmailVM.Email.Receiver))
            {
                MessageBox.Show("Wrong email address.");
                return;
            }
            mr.SendEmail(EmailVM.Email);
            Close();
        }
        #endregion

    }
}
