using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.ControlModule
{
    public class ResultControl
    {
        private ControlPagesController controller;
        public int TotalTime { get; set; }
        public int SpentTime { get; set; }
        public int NumberTasks { get; set; }
        public int NumberSolvedTasks { get; set; }

        public string TaskResultText { get; set; }
        public string TimeResultText { get; set; }
        public string SolvedTasksText { get; set; }

        private List<int> solvedTaskNumbers;

        public ResultControl(ControlPagesController controller)
        {
            this.controller = controller;
        }

        public ResultControl(ControlPagesController controller, int totalTime,
            int spentTime, int numberTasks, int numberSolvedTasks, List<int> solvedTaskNumbers)
        {
            this.controller = controller;
            TotalTime = totalTime;
            SpentTime = spentTime;
            NumberTasks = numberTasks;
            NumberSolvedTasks = numberSolvedTasks;
            this.solvedTaskNumbers = solvedTaskNumbers;
        }

        public void FormResult()
        {
            TaskResultText = $"Решено {NumberSolvedTasks} из {NumberTasks}.";
            TimeResultText = String.Format("На прохождение теста потрачено {0:00}:{1:00} минут из {2:00}:{3:00}.",
                SpentTime / 60, SpentTime % 60, TotalTime / 60, TotalTime % 60);
            SolvedTasksText = "Решены задачи под номерами: ";
            foreach (var task in solvedTaskNumbers)
            {
                SolvedTasksText += $"{task} ";
            }
        }

        public void Restart()
        {
            controller.Restart();
        }
    }
}
