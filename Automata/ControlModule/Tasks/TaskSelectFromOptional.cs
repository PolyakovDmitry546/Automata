using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.ControlModule.Tasks
{
    public class TaskSelectFromOptional : Task
    {
        public string TaskText { get; set; }

        public TaskSelectFromOptional(ControlPagesController controller):base(controller)
        {

        }

        public override void CheckAnswer()
        {
            throw new NotImplementedException();
        }
    }
}
