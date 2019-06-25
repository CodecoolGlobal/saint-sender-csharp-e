using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SaintSender.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ICommand SignInCommand { get; set; }

        public ConfigHandler Config { get; set; }

        public MainWindow()
        {
            SignInCommand = new RelayCommand(ShowSignInWindow);
            Config = ConfigHandler.Load();
            InitializeComponent();
            DataContext = this;
        }

        private void ShowSignInWindow(object param)
        {
            new LoginConfig(Config).Show();
        }
    }
}
