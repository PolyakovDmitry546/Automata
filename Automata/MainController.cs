using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Automata.ControlModule;

namespace Automata
{
    public static class MainController
    {
        public static Frame mainWindowFrame;

        public static InformftionPage TheoryPage = new InformftionPage();
        public static NavigationPage NavigationPage = new NavigationPage();
        public static MainPage MainPage = new MainPage();
        public static ExamplePage ExamplePage = new ExamplePage();
        public static TasksPage TasksPage = new TasksPage();
        public static ControlPage ControlPage = new ControlPage();

        public static int userId = -1;

        static MainController()
        {
            InitUser();
        }

        public static void ToNavigation()
        {
            mainWindowFrame.Navigate(NavigationPage);
        }

        public static void ToExample()
        {
            mainWindowFrame.Navigate(MainPage);
            MainPage.frameContent.Navigate(ExamplePage);
        }

        public static void ToTasks()
        {
            mainWindowFrame.Navigate(MainPage);
            MainPage.frameContent.Navigate(TasksPage);
        }

        public static void ToControl()
        {
            mainWindowFrame.Navigate(MainPage);
            MainPage.frameContent.Navigate(ControlPage);
        }

        public static void ToTheory()
        {
            mainWindowFrame.Navigate(MainPage);
            MainPage.frameContent.Navigate(TheoryPage);
        }

        public static void InitUser()
        {
            string textFromFile = String.Empty;
            try
            {
                using (FileStream fstream = new FileStream("user", FileMode.Open))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    textFromFile = System.Text.Encoding.Default.GetString(array);
                }
            }
            catch
            {
                MainPage.LoginInit();
            }
            if (int.TryParse(textFromFile, out userId))
            {
                MainPage.LogoutInit();
            }
            else
            {
                MainPage.LoginInit();
            }
        }

        public static void Logout()
        {
            userId = -1;
            File.Delete("user");
            MainPage.LoginInit();
        }

        public static void Login(int id)
        {
            userId = id;
            using (FileStream fstream = new FileStream("user", FileMode.Create))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(id.ToString());
                fstream.Write(array, 0, array.Length);
            }
            MainPage.LogoutInit();
        }

        private static string ListToString(List<int> list)
        {
            var sb = new StringBuilder();
            foreach (var task in list)
            {
                sb.Append(task);
                sb.Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public static void SendResult(int totalTime, int elapsedTime,
            int taskCount, List<int> solvedTaskNumbers, List<int> taskTypes)
        {
            if (userId == -1)
                return;
            try
            {
                string address = ConfigurationManager.AppSettings.Get("web-address");
                WebRequest request = WebRequest.Create("http://" + address + "/set_result");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var postData = "id=" + Uri.EscapeDataString(userId.ToString());
                postData += "&elapsed_time=" + Uri.EscapeDataString(elapsedTime.ToString());
                postData += "&total_time=" + Uri.EscapeDataString(totalTime.ToString());
                postData += "&task_count=" + Uri.EscapeDataString(taskCount.ToString());
                postData += "&solved_tasks=" + Uri.EscapeDataString(ListToString(solvedTaskNumbers));
                postData += "&task_types=" + Uri.EscapeDataString(ListToString(taskTypes));
                var data = Encoding.ASCII.GetBytes(postData);
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception)
            {

            }
        }
    }
}
