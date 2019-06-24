using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.IsolatedStorage;

namespace Models.Models.Tests
{
    [TestClass()]
    public class ConfigHandlerTests
    {
        [TestMethod()]
        public void SaveAndLoad_GivenAnEmptyConfigHandler_ReturnsEmptyConfigHandler()
        {
            ConfigHandler config = new ConfigHandler();
            config.Save("login_config.bin");

            ConfigHandler loadedConfig = ConfigHandler.Load("login_config.bin");

            bool areEqual = config.Address == loadedConfig.Address && config.Password == loadedConfig.Password;

            Assert.IsTrue(areEqual);
        }

        [TestMethod()]
        public void SaveAndLoad_GivenAnFilledConfig_ReturnsSameFilledConfigHandler()
        {
            ConfigHandler config = new ConfigHandler { Address = "Address", Password = "Password"};
            config.Save("login_config.bin");

            ConfigHandler loadedConfig = ConfigHandler.Load("login_config.bin");

            bool areEqual = config.Address == loadedConfig.Address && config.Password == loadedConfig.Password;

            Assert.IsTrue(areEqual);
        }

        [TestMethod()]
        public void Load_GivenAFileConfigThatDoesNotExist_ReturnsEmptyConfigHandler()
        {
            string noFileName = "no_file.bin";

            IsolatedStorageFile storage =
                IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

            if (storage.FileExists(noFileName))
            {
                storage.DeleteFile(noFileName);
            }

            ConfigHandler loadedConfig = ConfigHandler.Load(noFileName);

            bool isEmpty = loadedConfig.Address == string.Empty && loadedConfig.Password == string.Empty;

            Assert.IsTrue(isEmpty);
        }
    }
}