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
using Automata.Core;

namespace Automata.ControlModule.Tasks
{
    /// <summary>
    /// Логика взаимодействия для TaskSelectStatePage.xaml
    /// </summary>
    public partial class TaskSelectStatePage : Page
    {
        private TaskSelectState taskData;
        public TaskSelectStatePage(TaskSelectState taskData)
        {
            InitializeComponent();
            this.taskData = taskData;
            DataContext = taskData;
            var graphPage = new GraphPage();
            frameGraph.Navigate(graphPage);
            graphPage.ShowGraph(taskData.GraphTable);
        }

        private void buttonAnswer_Click(object sender, RoutedEventArgs e)
        {
            taskData.CheckAnswer();
        }
    }
}
