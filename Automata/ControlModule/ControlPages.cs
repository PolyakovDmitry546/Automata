using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Automata.ControlModule
{
    class ControlPages: INotifyPropertyChanged
    {
        public StartControlPage StartControlPage { get; }
        public TaskControlPage TaskControlPage { get; }
        ControlPage ControlPage { get; }

        Page currentPage;
        public Page CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (value != currentPage)
                {
                    currentPage = value;
                    NotifyPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public ControlPages(ControlPage controlPage)
        {
            ControlPage = controlPage;
            StartControlPage = new StartControlPage(ControlPage);
            CurrentPage = StartControlPage;
        }

        public void ToTask()
        {
            CurrentPage = new TaskControlPage();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
