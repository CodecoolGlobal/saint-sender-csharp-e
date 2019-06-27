using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
        private LoginConfig _loginConfigWindow;
        private ObservableCollection<MailModel> selectedMailList;
        private MailRepository _repository;

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
            SaveMailsToStorageCommand = new RelayCommand(SaveMailsToStorage);
            LoadMailsFromStorageCommand = new RelayCommand(LoadMailsFromStorage);
            LoadMailsFromServerCommand = new RelayCommand(LoadMailsFromServer);
            SearchCommand = new RelayCommand((obj) =>
            {
                Search(obj.ToString());
                SelectedMailList = SearchResults;
            }, (obj) => obj.ToString().Length > 3);
        }
        #region ICommands
        public ICommand SaveMailsToStorageCommand { get; set; }
        public ICommand LoadMailsFromStorageCommand { get; set; }
        public ICommand LoadMailsFromServerCommand { get; set; }
        public ICommand OpenSendEmailWindowCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        #endregion

        #region Properties
        public ObservableCollection<MailModel> Mails { get; set; } = new ObservableCollection<MailModel>();
        public ObservableCollection<MailModel> SearchResults { get; set; } = new ObservableCollection<MailModel>();
        public ObservableCollection<MailModel> SelectedMailList
        {
            get => selectedMailList; set
            {
                selectedMailList = value;
                OnPropertyChanged();
            }
        }

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

        #endregion

        #region Property change handler
        public DispatcherTimer MailRefreshTimer { get; private set; } = new DispatcherTimer();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Command methods

        private void ShowSignInWindow(object param)
        {
            if (!_loginConfigWindow.IsLoaded)
            {
                _loginConfigWindow = GetLoginConfig();
            }
            _loginConfigWindow.Show();
        }

        /// <summary>
        /// Saves mails to storage for the current user
        /// </summary>
        /// <param name="o"></param>
        public void SaveMailsToStorage(object o)
        {
            MailAddress address = new MailAddress(Repository.login);
            string name = address.User;
            foreach (var item in Mails)
            {
                item.Save(name);
            }
        }

        /// <summary>
        /// Gets mails from local storage for the current user
        /// </summary>
        /// <param name="o"></param>
        public void LoadMailsFromStorage(object o)
        {
            try
            {
                Mails.Clear();
                MailAddress address = new MailAddress(Repository.login);
                string name = address.User;
                foreach (var item in MailStorage.LoadMails(name))
                {
                    Mails.Add(item);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("No user logged in");
            }
        }

        /// <summary>
        /// Gets mails from remote server for the current user
        /// </summary>
        /// <param name="o"></param>
        public void LoadMailsFromServer(object o)
        {
            try
            {
                RefreshMailListAsync();
                SelectedMailList = Mails;
            }
            catch (Exception)
            {
                Console.WriteLine("No user logged in");
            }
        }

        /// <summary>
        /// Get mails from gmail's webserver
        /// </summary>
        /// <param name="o"></param>
        public void RefreshMails(object o)
        {
            Mails.Clear();
            foreach (var item in Repository.GetAllMails())
            {
                Mails.Add(item);
            }
        }

        private void showSendEmailWindow(object obj)
        {
            sew = new SendEmailWindow(Repository);
            sew.Show();
        }

        #endregion

        private LoginConfig GetLoginConfig()
        {
            return new LoginConfig(Config, Repository);
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Tries to connect to test website to verify whether there is internet connection
        /// </summary>
        /// <returns>True if connection was succesful</returns>
        private static bool CheckForInternetConnection()
        {
            return true;
        }

        public async Task Debug()
        {

            MailRepository mailRepo = new MailRepository();
            Mails.Clear();
            var items = await Task<IEnumerable<MailModel>>.Factory.StartNew(() => mailRepo.GetLastMails(10));
            foreach (var item in items)
            {
                Mails.Add(item);
            }
        }

        private async Task RefreshMailListAsync()
        {
            IEnumerable<MailModel> mails = await Task< IEnumerable < MailModel >>.Factory.StartNew(() => Repository.GetLastMails(10));
            foreach (var item in mails)
            {
                if (!Mails.Contains(item))
                {
                    Mails.Add(item);
                }
            }
        }

        private void Search(object obj)
        {
            var phrase = obj.ToString();
            if (phrase.Length == 0)
            {
                SelectedMailList = Mails;
                return;
            }
            
            var foundEmails = new ObservableCollection<MailModel>();
            var reg = new Regex(phrase);
            foreach (var email in Mails)
            {
                if (reg.IsMatch(email.Message) || reg.IsMatch(email.Subject) || reg.IsMatch(email.Sender))
                {
                    foundEmails.Add(email);
                }
            }

            SearchResults = foundEmails;
            SelectedMailList = SearchResults;
        }
    }
}
