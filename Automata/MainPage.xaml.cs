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
            MainController.ToTable();
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
    }
}
