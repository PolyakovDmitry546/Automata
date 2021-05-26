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

namespace Automata.ControlModule
{
    /// <summary>
    /// Логика взаимодействия для ResultControlPage.xaml
    /// </summary>
    public partial class ResultControlPage : Page
    {
        ResultControl resultControl;
        public ResultControlPage(ResultControl resultControl)
        {
            InitializeComponent();
            this.resultControl = resultControl;
            DataContext = resultControl;
        }

        private void buttonRestart_Click(object sender, RoutedEventArgs e)
        {
            resultControl.Restart();
        }
    }
}
