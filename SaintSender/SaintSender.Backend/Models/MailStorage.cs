using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SaintSender.Backend.Models
{
    public class MailStorage
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/SaintSender";

        /// <summary>
        /// Loads all stored mail for user
        /// </summary>
        /// <param name="user">String containing the beginning of email address example: "name" from "name@a.b"</param>
        /// <returns>List of MailModel</returns>
        public static List<MailModel> LoadMails(string user)
        {
            List<MailModel> mails = new List<MailModel>();
            try
            {
                var files = Directory.GetFiles(path + $@"/mails/{user}");
                foreach (var file in files)
                {
                    using (FileStream fileStream = File.Open(file, FileMode.Open))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        mails.Add((MailModel)formatter.Deserialize(fileStream));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"No stored mails were found for user: {user}\nException type: {e.GetType()}");
            }
            return mails;
        }
    }
}
