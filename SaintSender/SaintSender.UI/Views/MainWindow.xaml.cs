using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        #region ICommands
        public ICommand SaveMailsToStorageCommand { get; set; }
        public ICommand LoadMailsFromStorageCommand { get; set; }
        public ICommand LoadMailsFromServerCommand { get; set; }
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

        /// <summary>
        /// Gets and sets whether the application is running in offline mode
        /// </summary>
        public bool OfflineMode { get; set; } = true;

        public ICommand SignInCommand { get; set; }

        public ConfigHandler Config { get; set; }

        public MailRepository Repository { get; set; }

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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Command methods
        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

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
                Mails.Clear();
                foreach (var item in Repository.GetAllMails())
                {
                    Mails.Add(item);
                }
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
        #endregion

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            Unloaded += MainWindow_Unloaded;

            // Initialize login config window
            Config = ConfigHandler.Load();
            _loginConfigWindow = GetLoginConfig();

            // Debug
#warning debug repo, remove once login fixed
            Repository = new MailRepository();
            SelectedMailList = Mails;

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

            // Check for internet connection
            Task.Run(() => CheckConnection());
        }

        private LoginConfig GetLoginConfig()
        {
            return new LoginConfig(Config, Repository);
        }

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

        private void Search(string phrase)
        {
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
