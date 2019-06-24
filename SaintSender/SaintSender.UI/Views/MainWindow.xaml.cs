using Models.Logic;
using Models.Models;
using SaintSender.UI.Utils;
using System;
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

        public ICommand ChangeSelectedMailCommand { get; set; }

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

        public MainWindow()
        {
            InitializeComponent();
            Debug();
            ChangeSelectedMailCommand = new RelayCommand(ChangeSelectedMail);
            DataContext = this;
        }

        public void Debug()
        {
            MailRepository mailRepo = new MailRepository();
            Console.WriteLine(mailRepo.GetAllMails());

            MailModel mail;
            for (int i = 0; i < 5; i++)
            {
                mail = new MailModel();
                mail.Sender = "sender " + i;
                mail.Message = "msg " + i;
                mail.Subject = "subject " + i;
                mail.Date = DateTime.Now;
                Mails.Add(mail);
            }
        }
    }
}
