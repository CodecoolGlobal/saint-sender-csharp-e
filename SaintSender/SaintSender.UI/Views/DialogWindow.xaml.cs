using SaintSender.UI.Utils;
using System.Windows;
using System.Windows.Input;

namespace SaintSender.UI.Views
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        #region ICommands

        /// <summary>
        /// Command that closes the given window and this dialog as well.
        /// </summary>
        public ICommand AcceptDiscardCommand { get; set; }
        /// <summary>
        /// Command that prevents the given window from closing and closes this dialog.
        /// </summary>
        public ICommand DeclineDiscardCommand { get; set; }

        #endregion

        /// <summary>
        /// Creates a closing confirmation dialog window.
        /// </summary>
        /// <param name="window">Window whose closing need to be confirmed.</param>
        public DialogWindow(Window window)
        {
            DataContext = this;
            DeclineDiscardCommand = new RelayCommand((obj) => Close());
            AcceptDiscardCommand = new RelayCommand((obj) => DiscardMessage(window));
            InitializeComponent();
        }

        #region Command Methods

        /// <summary>
        /// Closes both the dialog and the given window.
        /// </summary>
        /// <param name="window"></param>
        private void DiscardMessage(Window window)
        {
            window.Close();
            Close();
        }

        #endregion
    }
}
