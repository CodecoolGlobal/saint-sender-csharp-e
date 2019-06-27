using SaintSender.Backend.Models;
using SaintSender.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SaintSender.UI.Converters
{
    [ValueConversion(typeof(MailModel), typeof(InboxViewModel))]
    class InboxViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is MailModel)
            {
                return new InboxViewModel(value as MailModel);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InboxViewModel)
            {
                return ((InboxViewModel)value).Mail;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
