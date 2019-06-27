using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SaintSender.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginConfig.xaml
    /// </summary>
    public partial class LoginConfig : Window
    {
        public ConfigHandler Config { get; set; }

        public MailRepository Repository { get; set; }

        public ICommand SignInCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public LoginConfig(ConfigHandler config, MailRepository repo)
        {
            Config = config;
            SignInCommand = new RelayCommand(SignIn);
            CancelCommand = new RelayCommand((o) => Close());
            InitializeComponent();
            DataContext = this;
        }

        private void SignIn(object passwordBox)
        {
            var pb = passwordBox as PasswordBox;
            var newRepo = new MailRepository(Config.Address, pb.Password);
            if (newRepo.CheckCredentials())
            {
                Config.Password = pb.Password;
                Config.Save();
                Repository = newRepo;
                Close();
            }
            else
            {
                MessageBox.Show("The entered credentials are incorrect.");
            }
        }
    }
}
