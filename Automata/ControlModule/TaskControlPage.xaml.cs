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
using System.Windows.Threading;

namespace Automata.ControlModule
{
    /// <summary>
    /// Логика взаимодействия для TaskControlPage.xaml
    /// </summary>
    public partial class TaskControlPage : Page
    {
        int timerBalance;
        DispatcherTimer timer;

        public TaskControlPage()
        {
            InitializeComponent();
            InitTimer();
        }

        void InitTimer()
        {
            timerBalance = 60;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();
        }

        void TimerTick(object sender, EventArgs e)
        {
            if (timerBalance >= 0)
            {
                labelTimer.Content = String.Format("{0:00}:{1:00}",timerBalance / 60, timerBalance % 60);
                timerBalance -= 1;
            }
            else
            {
                timer.Stop();
            }
        }
    }
}
