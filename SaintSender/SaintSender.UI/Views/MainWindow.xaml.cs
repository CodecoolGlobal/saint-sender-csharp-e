using SaintSender.Backend.Logic;
using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

            Repository = new MailRepository(Config.Address, Config.Password);
            Repository.CheckCredentials();

            MailRefreshTimer.Interval = new TimeSpan(0, 0, 5);
            MailRefreshTimer.Tick += (obj, args) => RefreshMailListAsync();
            MailRefreshTimer.Start();

            // Setup commands
            SignInCommand = new RelayCommand(ShowSignInWindow);
        }

        public ObservableCollection<MailModel> Mails { get; set; } = new ObservableCollection<MailModel>();

        public ICommand SignInCommand { get; set; }

        public ConfigHandler Config { get; set; }

        public MailRepository Repository
        {
            get => _repository;
            set
            {
                _repository = value;
                RefreshMailListAsync();
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

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public async Task Debug()
        {

            MailRepository mailRepo = new MailRepository();
            Mails.Clear();
            var items = await Task<IEnumerable<MailModel>>.Factory.StartNew(mailRepo.GetAllMails);
            foreach (var item in items)
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

        private async Task RefreshMailListAsync()
        {
            IEnumerable<MailModel> mails = await Task< IEnumerable < MailModel >>.Factory.StartNew(Repository.GetAllMails);
            foreach (var item in mails)
            {
                if (!Mails.Contains(item))
                {
                    Mails.Add(item);
                }
            }
        }
    }
}
