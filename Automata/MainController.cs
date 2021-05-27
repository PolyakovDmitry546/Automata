using System;
using System.Collections.Generic;
using System.Linq;
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
        public static TablePage TablePage = new TablePage();
        public static TasksPage TasksPage = new TasksPage();
        public static ControlPage ControlPage = new ControlPage();

        public static void ToNavigation()
        {
            mainWindowFrame.Navigate(NavigationPage);
        }

        public static void ToTable()
        {
            mainWindowFrame.Navigate(MainPage);
            MainPage.frameContent.Navigate(TablePage);
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
    }
}
