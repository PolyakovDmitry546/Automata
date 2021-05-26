using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.ControlModule.Tasks
{
    public class TaskSelectRecognize : Task
    {
        public string TaskText { get; set; }
        public GraphTable GraphTable { get; set; }

        private bool rightAnswer;
        public bool YesChecked { get; set; }
        public bool NoChecked { get; set; }

        public TaskSelectRecognize(ControlPagesController controller) : base(controller)
        {
            InitTask();
            Page = new TaskSelectRecognizePage(this);
        }

        private void InitTask()
        {
            GraphTable = new GraphTable();
            var random = new Random();
            GraphTable.RandomGenerate();
            var word = GraphTable.GenerateWord();
            TaskText = "Задан конечный автомат-распознаватель диаграммой Мура. " +
                $"Распознает ли автомат слово {word}?";
            rightAnswer = GraphTable.Recognize(word);
        }

        public override void CheckAnswer()
        {
            if (YesChecked || NoChecked)
            {
                if (rightAnswer && YesChecked || !rightAnswer && NoChecked)
                    CorrectAnswer = true;
                else
                    CorrectAnswer = false;
            }
            else
                CorrectAnswer = false;

            Controller.Answer();
        }
    }
}
