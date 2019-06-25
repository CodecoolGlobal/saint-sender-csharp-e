﻿using SaintSender.Backend.Logic;
using SaintSender.Backend.Models;
using SaintSender.UI.Utils;
using System;
using System.Windows;
using System.Windows.Input;

namespace SaintSender.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginConfig.xaml
    /// </summary>
    public partial class LoginConfig : Window
    {
        public ConfigHandler Config { get; set; }

        public MailRepository Repository { get; set; }

        public ICommand SignInCommand { get; set; }

        public LoginConfig(ConfigHandler config, MailRepository repo)
        {
            Config = config;
            SignInCommand = new RelayCommand(SignIn);
            InitializeComponent();
            DataContext = this;
        }

        private void SignIn(object obj)
        {
            Config.Save();
            Repository = new MailRepository(Config.Address, Config.Password);
        }
    }
}
