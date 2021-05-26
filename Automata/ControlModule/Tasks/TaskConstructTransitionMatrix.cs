using System;
using System.Data;
using Automata.Core;

namespace Automata.ControlModule.Tasks
{
    public class TaskConstructTransitionMatrix:Task
    {
        public string TaskText { get; set; }

        public DataTable UserTransitionTable { get; set; }

        public GraphTable GraphTable { get; set; }

        public TaskConstructTransitionMatrix(ControlPagesController controller):base(controller)
        {
            InitTask();
            Page = new TaskCounstructTransitionMatrixPage(this);
        }

        private void InitTask()
        {
            TaskText = "Задан конечный автомат-распознаватель диаграммой Мура. " +
                $"Построить таблицу переходов.";
            GraphTable = new GraphTable();
            var random = new Random();
            GraphTable.RandomGenerate();
            UserTransitionTable = CreateUserTransitionTable(GraphTable);
        }

        public override void CheckAnswer()
        {
            if(GraphTable.Compare(UserTransitionTable))
            {
                CorrectAnswer = true;
            }
            else
            {
                CorrectAnswer = false;
            }
            Controller.Answer();
        }

        private DataTable CreateUserTransitionTable(GraphTable table)
        {
            var userGraphTabel = new DataTable();
            userGraphTabel.Columns.Add("Состояние");
            foreach (var ch in table.alphabet)
            {
                userGraphTabel.Columns.Add(ch.ToString());
            }

            return userGraphTabel;
        }
    }
}
