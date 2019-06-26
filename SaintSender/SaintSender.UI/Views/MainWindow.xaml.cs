using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

#warning probably already done in config branch check if causes problems
        private MailRepository mailRepo = new MailRepository();

        public ICommand ChangeSelectedMailCommand { get; set; }
        public ICommand SaveMailsToStorageCommand { get; set; }
        public ICommand LoadMailsFromStorageCommand { get; set; }

        public ObservableCollection<MailModel> Mails { get; set; } = new ObservableCollection<MailModel>();

        public MailModel SelectedMail
        {
            get => _selectedMail; set
            {
                _selectedMail = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ChangeSelectedMail(object o)
        {
            SelectedMail = Mails[0];
        }

#warning Change to actual user once config is intergrated
        public void SaveMailsToStorage(object o)
        {
            foreach (var item in Mails)
            {
                item.Save("sunyibela");
            }
        }

        /// <summary>
        /// Gets mails from local storage
        /// </summary>
        /// <param name="o"></param>
#warning Change to actual user once config is intergrated
        public void LoadMailsFromStorage(object o)
        {
            Mails.Clear();
            foreach (var item in MailStorage.LoadMails("sunyibela"))
            {
                Mails.Add(item);
            }
        }

        /// <summary>
        /// Get mails from gmail's webserver
        /// </summary>
        /// <param name="o"></param>
        public void RefreshMails(object o)
        {
            Mails.Clear();
            mailRepo = new MailRepository();
            foreach (var item in mailRepo.GetAllMails())
            {
                Mails.Add(item);
            }
        }

        /// <summary>
        /// If user already logged in load mails
        /// </summary>
#warning change to actual check once config is integrated
        private void LoadInitialMails()
        {
            foreach (var item in mailRepo.GetAllMails())
            {
                Mails.Add(item);
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            //LoadInitialMails();
            ChangeSelectedMailCommand = new RelayCommand(ChangeSelectedMail);
            SaveMailsToStorageCommand = new RelayCommand(SaveMailsToStorage);
            LoadMailsFromStorageCommand = new RelayCommand(LoadMailsFromStorage);
        }
    }
}
