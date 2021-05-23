﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Automata.ControlModule
{
    /// <summary>
    /// Логика взаимодействия для StartControlPage.xaml
    /// </summary>
    public partial class StartControlPage : Page
    {
        ControlPage controlPage;

        public StartControlPage(ControlPage controlPage)
        {
            InitializeComponent();
            this.controlPage = controlPage;
        }

        private void buttonStartTest_Click(object sender, RoutedEventArgs e)
        {
            var controlPages = controlPage.DataContext as ControlPages;
            controlPages.ToTask();
        }
    }
}