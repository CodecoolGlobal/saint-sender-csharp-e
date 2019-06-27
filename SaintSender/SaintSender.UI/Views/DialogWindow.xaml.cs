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
        public ICommand AcceptDiscardCommand { get; set; }
        public ICommand DeclineDiscardCommand { get; set; }
        private Window _closingWindow;
        public DialogWindow(Window window)
        {
            DataContext = this;
            _closingWindow = window;
            AcceptDiscardCommand = new RelayCommand(DiscardMessage);
            DeclineDiscardCommand = new RelayCommand(CancelDiscard);
            InitializeComponent();
        }

        private void CancelDiscard(object obj)
        {
            Close();
        }

        private void DiscardMessage(object obj)
        {
            _closingWindow.Close();
            Close();
        }
    }
}
