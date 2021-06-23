using System.Windows.Controls;

namespace Automata.ControlModule.Tasks
{
    public abstract class Task
    {
        public ControlPagesController Controller { get; }
        public Page Page { get; set; }
        public bool CorrectAnswer { get; set; }
        public abstract void CheckAnswer();

        public int type;

        public Task(ControlPagesController controller)
        {
            CorrectAnswer = false;
            Controller = controller;
        }
    }
}
