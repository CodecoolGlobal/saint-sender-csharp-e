using SaintSender.Backend.Models;

namespace SaintSender.UI.ViewModels
{
    /// <summary>
    /// ViewModel that binds the MailModel model with a window.
    /// </summary>
    public class SendMailViewModel : ViewModelBase
    {
        /// <summary>
        /// MailModel to be bound.
        /// </summary>
        public MailModel Email { get; } = new MailModel();

        /// <summary>
        /// Sets the Email's required fields with default values.
        /// </summary>
        public SendMailViewModel()
        {
            Email.Message = "";
            Email.Subject = "";
            Email.Receiver = "";
        }

        #region Bound Properties

        /// <summary>
        /// Body of the email.
        /// </summary>
        public string Message
        {
            get => Email.Message; set
            {
                Email.Message = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Email address that needs to receive the email.
        /// </summary>
        public string Receiver
        {
            get => Email.Receiver; set
            {
                Email.Receiver = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Subject of your email.
        /// </summary>
        public string Subject
        {
            get => Email.Subject; set
            {
                Email.Subject = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
