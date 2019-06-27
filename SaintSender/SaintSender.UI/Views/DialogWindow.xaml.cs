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
            DeclineDiscardCommand = new RelayCommand((obj) => Close());
            AcceptDiscardCommand = new RelayCommand(DiscardMessage);
            InitializeComponent();
        }

        private void DiscardMessage(object obj)
        {
            _closingWindow.Close();
            Close();
        }
    }
}
