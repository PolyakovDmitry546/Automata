﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Automata.ControlModule.Tasks;

namespace Automata.ControlModule
{
    public class ControlPagesController
    {
        private Frame frame;
        private StartControlPage startControlPage;
        private TaskControlPage taskControlPage;
        private ResultControlPage resultControlPage;

        private int currentTaskIndex;
        private int CurrentTaskIndex
        {
            get
            {
                return currentTaskIndex;
            }
            set
            {
                if(value == 0)
                {
                    taskControlPage.buttonBack.IsEnabled = false;
                }
                else
                {
                    taskControlPage.buttonBack.IsEnabled = true;
                }
                if(value == tasks.Count - 1)
                {
                    taskControlPage.buttonNext.IsEnabled = false;
                }
                else
                {
                    taskControlPage.buttonNext.IsEnabled = true;
                }
                currentTaskIndex = value;
            }
        }
        private List<Task> tasks;

        public ControlPagesController(Frame frame)
        {
            this.frame = frame;
            startControlPage = new StartControlPage(this);
            taskControlPage = new TaskControlPage(this);
            resultControlPage = new ResultControlPage(this);
        }

        private void GenerateTasks()
        {
            tasks = new List<Task>();
            taskControlPage.stackPanelTasks.Children.Clear();
            var random = new Random();
            for (int i = 0; i < 5; i++)
            {
                var n = random.Next(2);
                if (n == 0)
                    tasks.Add(new TaskConstructTransitionMatrix(this));
                else
                    tasks.Add(new TaskSelectState(this));
                var button = new Button()
                {
                    Content = $"Задание {i + 1}",
                    Tag = i
                };
                button.Click += new RoutedEventHandler(buttonTask_Click);
                taskControlPage.stackPanelTasks.Children.Add(button);
            }
        }

        private void buttonTask_Click(object sender, RoutedEventArgs e)
        {
            ToTask((int)(sender as Button).Tag);
        }

        public void ToStart()
        {
            frame.Navigate(startControlPage);
        }

        public void ToTask()
        {
            GenerateTasks();
            CurrentTaskIndex = 0;
            frame.Navigate(taskControlPage);
            ToTask(CurrentTaskIndex);
        }

        public void ToTask(int taskIndex)
        {
            if(taskIndex >=0 && taskIndex <= tasks.Count)
            {
                CurrentTaskIndex = taskIndex;
                taskControlPage.frameTask.Navigate(tasks[taskIndex].Page);
            }
        }

        public void ToResult()
        {
            frame.Navigate(resultControlPage);
        }

        public void NextTask()
        {
            if (CurrentTaskIndex < tasks.Count - 1)
            {
                CurrentTaskIndex += 1;
                taskControlPage.frameTask.Navigate(tasks[CurrentTaskIndex].Page);
            }
        }

        public void PreviousTask()
        {
            if (CurrentTaskIndex > 0)
            {
                CurrentTaskIndex -= 1;
                taskControlPage.frameTask.Navigate(tasks[CurrentTaskIndex].Page);
            }
        }

        public void Answer()
        {
            for(int i = 0; i < taskControlPage.stackPanelTasks.Children.Count; i++)
            {
                if(i == currentTaskIndex)
                {
                    (taskControlPage.stackPanelTasks.Children[i] as Button).Background =
                        new SolidColorBrush(Color.FromRgb(15, 225, 15));
                }
            }
        }
    }
}