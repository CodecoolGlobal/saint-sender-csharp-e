using SaintSender.Backend.Logic;
using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SaintSender.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MailModel _selectedMail = new MailModel();
        private MailRepository _repository;
        private LoginConfig _loginConfigWindow;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            Unloaded += MainWindow_Unloaded;

            // Initialize login config window
            Config = ConfigHandler.Load();
            _loginConfigWindow = GetLoginConfig();

            _repository = new MailRepository(Config.Address, Config.Password);

            MailRefreshTimer.Interval = new TimeSpan(0, 0, 5);
            MailRefreshTimer.Tick += (obj, args) => RefreshMailList();
            MailRefreshTimer.Start();

            // Setup commands
            SignInCommand = new RelayCommand(ShowSignInWindow);
            ChangeSelectedMailCommand = new RelayCommand(ChangeSelectedMail);
        }

        public ICommand ChangeSelectedMailCommand { get; set; }

        public ObservableCollection<MailModel> Mails { get; set; } = new ObservableCollection<MailModel>();

        public ICommand SignInCommand { get; set; }

        public ConfigHandler Config { get; set; }

        public MailRepository Repository
        {
            get => _repository;
            set
            {
                _repository = value;
                RefreshMailList();
            }
        }

        public MailModel SelectedMail
        {
            get => _selectedMail; set
            {
                _selectedMail = value;
                OnPropertyChanged();
            }
        }

        public DispatcherTimer MailRefreshTimer { get; private set; } = new DispatcherTimer();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ChangeSelectedMail(object o)
        {
            SelectedMail = Mails[0];
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public void Debug()
        {

            MailRepository mailRepo = new MailRepository();
            Mails.Clear();
            foreach (var item in mailRepo.GetAllMails())
            {
                Mails.Add(item);
            }
        }

        private void ShowSignInWindow(object param)
        {
            if (!_loginConfigWindow.IsLoaded)
            {
                _loginConfigWindow = GetLoginConfig();
            }
            _loginConfigWindow.Show();
        }

        private LoginConfig GetLoginConfig()
        {
            return new LoginConfig(Config, Repository);
        }

        private void RefreshMailList()
        {
            Mails.Clear();
            foreach (var item in Repository.GetAllMails())
            {
                Mails.Add(item);
            }
            MessageBox.Show("refreshing mails");
        }
    }
}
