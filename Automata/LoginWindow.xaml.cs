using System;
using System.Windows;
using System.Configuration;
using System.Net;
using System.Text;
using System.IO;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void battonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var login = loginTextBox.Text;
                var password = passwordTextBox.Text;
                string address = ConfigurationManager.AppSettings.Get("web-address");
                WebRequest request = WebRequest.Create("http://" + address + "/try_login");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var postData = "login=" + Uri.EscapeDataString(login);
                postData += "&password=" + Uri.EscapeDataString(password);
                var data = Encoding.ASCII.GetBytes(postData);
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if(responseString != "None")
                {
                    errorTextBlock.Text = "Вы успешно авторизировались";
                    MainController.Login(int.Parse(responseString));
                }
                else
                {
                    errorTextBlock.Text = "Неверный логин или пароль";
                }
            }
            catch (Exception)
            {

                errorTextBlock.Text = "Ошибка при попытке авторизации. Попробуйте еще раз";
            }
        }
    }
}
