using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.ControlModule.Tasks
{
    public class TaskSelectState:Task
    {
        public string TaskText { get; set; }
        public GraphTable GraphTable { get; set; }
        public string AnswerText { get; set; }

        private string rightAnswer;

        public TaskSelectState(ControlPagesController controller) : base(controller)
        {
            InitTask();
            Page = new TaskSelectStatePage(this);
        }

        private void InitTask()
        {
            GraphTable = new GraphTable();
            var random = new Random();
            GraphTable.RandomGenerate();
            var word = GraphTable.GenerateWord();
            TaskText = "Задан конечный автомат-распознаватель диаграммой Мура. " +
                $"В каком состояние автомат закончит распознавание слова {word}?";
            rightAnswer = GraphTable.GetState(word).ToString();
        }

        public override void CheckAnswer()
        {
            if (AnswerText == rightAnswer)
                CorrectAnswer = true;
            else
                CorrectAnswer = false;
            Controller.Answer();
        }
    }
}
