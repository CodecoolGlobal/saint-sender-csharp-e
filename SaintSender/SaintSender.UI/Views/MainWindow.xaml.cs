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
        private ObservableCollection<MailModel> _selectedMailList;
        private MailRepository _repository;

        private SendEmailWindow _send;

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
            SearchCommand = new RelayCommand((obj) => Search(obj), (obj) => obj.ToString().Length > 3 || obj.ToString().Length == 0);
            ShowSendEmailWindowCommand = new RelayCommand(ShowSendEmailWindow, (obj) => CheckForInternetConnection());

            // Check for internet connection
            Task.Run(() => CheckConnection());
        }

        #region ICommands
        public ICommand SaveMailsToStorageCommand { get; set; }
        public ICommand LoadMailsFromStorageCommand { get; set; }
        public ICommand LoadMailsFromServerCommand { get; set; }
        public ICommand OpenSendEmailWindowCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowSendEmailWindowCommand { get; set; }
        #endregion

        #region Properties
        public ObservableCollection<MailModel> Mails { get; set; } = new ObservableCollection<MailModel>();
        public ObservableCollection<MailModel> OfflineMails { get; set; } = new ObservableCollection<MailModel>();
        public ObservableCollection<MailModel> SearchResults { get; set; } = new ObservableCollection<MailModel>();
        public ObservableCollection<MailModel> SelectedMailList
        {
            get => _selectedMailList; set
            {
                _selectedMailList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets and sets whether the application is running in offline mode
        /// </summary>
        public bool OfflineMode { get; set; } = true;

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
                if(value != null)
                {
                    _selectedMail = value.Copy();
                    OnPropertyChanged();
                }
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

        private void ShowSendEmailWindow(object obj)
        {
            _send = new SendEmailWindow(Repository);
            _send.Show();
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
                OfflineMails.Clear();
                MailAddress address = new MailAddress(Repository.login);
                string name = address.User;
                foreach (var item in MailStorage.LoadMails(name))
                {
                    OfflineMails.Add(item);
                }
                SelectedMailList = OfflineMails;
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

        /// <summary>
        /// Searches for emails that contain the given phrase.
        /// </summary>
        /// <param name="obj">Command arg that is the phrase for which to search.</param>
        private void Search(object obj)
        {
            // Convert command arg to string
            var phrase = obj.ToString();

            // If it is empty string reset the inbox view to display all messages
            if (phrase.Length == 0)
            {
                SelectedMailList = Mails;
                return;
            }

            // Create new ObservableCollection that is to be populated with emails that match with given regex
            var foundEmails = new ObservableCollection<MailModel>();
            var reg = new Regex(phrase);

            // Choose the emails whose properties match the regex.
            foreach (var email in Mails)
            {
                if (reg.IsMatch(email.Message) || reg.IsMatch(email.Subject) || reg.IsMatch(email.Sender))
                {
                    foundEmails.Add(email);
                }
            }

            // Set the email list that is to be displayed in InboxView
            SearchResults = foundEmails;
            SelectedMailList = SearchResults;
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
        private void CheckConnection()
        {
            OfflineMode = !CheckForInternetConnection();
        }

        /// <summary>
        /// Tries to connect to test website to verify whether there is internet connection
        /// </summary>
        /// <returns>True if connection was succesful</returns>
        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
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
    }
}
