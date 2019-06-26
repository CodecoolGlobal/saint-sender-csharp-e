using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaintSender.Backend.Models
{
    class MailStorage
    {
        private static IsolatedStorageFile _storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

        /// <summary>
        /// Stores email to isolated storage
        /// </summary>
        /// <param name="mail">MailModel to store</param>
        /// <param name="user">String containing the beginning of email address example: "name" from "name@a.b"</param>
        public static void StoreMail(MailModel mail, string user)
        {
            _storage.CreateFile(@"/mails/"+user+@"/"+mail.Date);
        }
    }
}
