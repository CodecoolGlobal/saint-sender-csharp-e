using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    /// <summary>
    /// This class contains user login information and methods for
    /// saving it on the disc.
    /// </summary>
    [Serializable]
    public class ConfigHandler
    {
        private static IsolatedStorageFile _storage =
            IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        /// <summary>
        /// E-mail address of the user we want to log in with.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Password for the user we want to log in with.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Loads the login configuration from local storage by deserializing
        /// a binary file.
        /// </summary>
        /// <returns>The configuration from file.</returns>
        public static ConfigHandler Load(string path)
        {
            ConfigHandler config = null;
            try
            {
                using (FileStream fileStream = _storage.OpenFile(path, FileMode.Open))
                {
                    IFormatter formatter = new BinaryFormatter();
                    config = (ConfigHandler) formatter.Deserialize(fileStream);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Config file not found or unreadable. Generating default config.");
                config = new ConfigHandler();
            }
            return config;
        }

        /// <summary>
        /// Serializes this instance of the login configuration into
        /// a file in the local storage.
        /// </summary>
        public void Save(string path)
        {
            using (FileStream fileStream = _storage.CreateFile(path))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, this);
            }
        }
    }
}
