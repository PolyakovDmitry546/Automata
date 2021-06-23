using System;
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

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void buttonExamples_Click(object sender, RoutedEventArgs e)
        {
            MainController.ToExample();
        }

        private void buttonTasks_Click(object sender, RoutedEventArgs e)
        {
            MainController.ToTasks();
        }

        private void buttonControl_Click(object sender, RoutedEventArgs e)
        {
            MainController.ToControl();
        }

        private void buttonTheory_Click(object sender, RoutedEventArgs e)
        {
            MainController.ToTheory();
        }

        private void buttonMain_Click(object sender, RoutedEventArgs e)
        {
            MainController.ToNavigation();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            var window = new LoginWindow();
            window.ShowDialog();
        }

        private void buttonSignin_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegisterWindow();
            window.Register();
            window.ShowDialog();
        }

        public void LogoutInit()
        {
            loginStackPanel.Children.Clear();
            var logoutButton = new Button();
            logoutButton.Content = "Выйти";
            logoutButton.Click += buttonLogout_Click;
            logoutButton.Width = 120;
            logoutButton.Margin = new Thickness(5, 3, 5, 3);
            loginStackPanel.Children.Add(logoutButton);
        }

        public void LoginInit()
        {
            loginStackPanel.Children.Clear();
            var signinButton = new Button();
            signinButton.Content = "Зарегистрироваться";
            signinButton.Click += buttonSignin_Click;
            signinButton.Width = 120;
            signinButton.Margin = new Thickness(5, 3, 5, 3);
            loginStackPanel.Children.Add(signinButton);
            var loginButton = new Button();
            loginButton.Content = "Войти";
            loginButton.Click += buttonLogin_Click;
            loginButton.Width = 120;
            loginButton.Margin = new Thickness(5, 3, 5, 3);
            loginStackPanel.Children.Add(loginButton);
        }

        private void buttonLogout_Click(object sender, RoutedEventArgs e)
        {
            MainController.Logout();
        }
    }
}
