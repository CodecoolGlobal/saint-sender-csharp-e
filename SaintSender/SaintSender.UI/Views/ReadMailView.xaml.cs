﻿using SaintSender.Backend.Logic;
using System;
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

            //browserMessageDisplay.NavigateToString(((MainWindow) DataContext).SelectedMail.Message);
        }
    }
}
