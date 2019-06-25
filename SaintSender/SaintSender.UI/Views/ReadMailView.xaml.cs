using SaintSender.Backend.Logic;
using System.Linq;
using System.Windows.Controls;

namespace SaintSender.UI.Views
{
    /// <summary>
    /// Interaction logic for ReadMailView.xaml
    /// </summary>
    public partial class ReadMailView : UserControl
    {
        public ReadMailView()
        {
            InitializeComponent();


            MailRepository mailRepo = new MailRepository();
            var t = mailRepo.GetAllMails();
            browserMessageDisplay.NavigateToString(t.First());
        }
    }
}
