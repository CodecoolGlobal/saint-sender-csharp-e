using SaintSender.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaintSender.UI.ViewModels
{
    class ConfigHandlerViewModel : ViewModelBase
    {
        public ConfigHandler ConfigHandler { get; set; }

        public string Password {
            get
            {
                return ConfigHandler.Password;
            }
            set
            {
                ConfigHandler.Password = value;
                OnPropertyChanged();
            }
        }

        public string Addresss
        {
            get
            {
                return ConfigHandler.Address;
            }
            set
            {
                ConfigHandler.Address = value;
                OnPropertyChanged();
            }
        }

        ConfigHandlerViewModel(ConfigHandler configHandler)
        {
            ConfigHandler = configHandler;
        }
    }
}
