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
        ControlPagesController controller;
        int timerBalance;
        DispatcherTimer timer;

        public TaskControlPage(ControlPagesController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        public void InitTimer()
        {
            timerBalance = controller.totalTime;

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
                controller.spentTime = controller.totalTime;
                controller.ToResult();
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            controller.PreviousTask();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            controller.NextTask();
        }

        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            controller.spentTime = controller.totalTime - timerBalance;
            timer.Stop();
            controller.ToResult();
        }
    }
}
