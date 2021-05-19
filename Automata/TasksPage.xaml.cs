using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для TasksPage.xaml
    /// </summary>
    public partial class TasksPage : Page
    {
        private GraphTable graphTable;
        private string wordForCheck;
        private string questionWord;

        public TasksPage()
        {
            InitializeComponent();
            DataContext = new GraphPage();
            graphFrame.Navigate(DataContext);
        }

        private DataTable CreateUserGraphTable(GraphTable table)
        {
            var userGraphTabel = new DataTable();
            userGraphTabel.Columns.Add("Состояние");
            foreach(var ch in table.alphabet)
            {
                userGraphTabel.Columns.Add(ch.ToString());
            }

            return userGraphTabel;
        }

        private string GenerateRandomWord(GraphTable graphTable)
        {
            var random = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < random.Next(5, 9); i++)
            {
                stringBuilder.Append(graphTable.alphabet.ElementAt(random.Next(graphTable.alphabet.Count)));
            }

            return stringBuilder.ToString();
        }

        private void buttonNewTask_Click(object sender, RoutedEventArgs e)
        {
            graphTable = new GraphTable();
            var random = new Random();
            graphTable.Generate(random.Next(3, 6), random.Next(3, 7), random.Next(1, 4));
            var graphPage = DataContext as GraphPage;
            graphPage.ShowGraph(graphTable);

            wordForCheck = GenerateRandomWord(graphTable);
            textBlockTask.Text = "Задан конечный автомат-распознаватель диаграммой Мура. " +
                $"Построить таблицу переходов. На входе слово {wordForCheck}. " +
                "В каком состоянии автомат закончит обработку входного слова?";

            transitionTable.DataContext = CreateUserGraphTable(graphTable);

            questionWord = GenerateRandomWord(graphTable);
            textBlockQuestion.Text = $"Распознает ли автомат слово {questionWord}?";
        }

        private async void buttonCheckWord_Click(object sender, RoutedEventArgs e)
        {
            var text = textboxCheckWord.Text;
            var graphPage = DataContext as GraphPage;
            await Task.Run(() => graphPage.CheckWord(text));
        }

        private bool CompareTables(GraphTable graphTable, DataTable userGraphTabel)
        {
            if (graphTable == null || userGraphTabel == null)
                return false;

            if (graphTable.columnsCount != userGraphTabel.Columns.Count - 1)
                return false;

            if (graphTable.rowsCount != userGraphTabel.Rows.Count)
                return false;

            for(int i = 0; i < graphTable.rowsCount; i++)
            {
                if ((String)userGraphTabel.Rows[i]["Состояние"] != i.ToString())
                    return false;
                for(int j = 0; j < graphTable.columnsCount; j++)
                {
                    char ch = (char)((int)'a' + j);
                    if (graphTable.dtran[(i, ch)].ToString() 
                        != (String)userGraphTabel.Rows[i][ch.ToString()])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void SetButtonRight(Button button)
        {
            Task.Run(() =>
            {
                String buttonText = "Проверить";
                Brush backgroundColor = SystemColors.ControlBrush;
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = Brushes.Green;
                    button.Content = "Верно";
                }));
                Thread.Sleep(1000);
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = SystemColors.ControlBrush;
                    button.Content = buttonText;
                }));
            });
        }

        private void SetButtonRight(Button button, string buttonText)
        {
            Task.Run(() =>
            {
                Brush backgroundColor = SystemColors.ControlBrush;
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = Brushes.Green;
                    button.Content = "Верно";
                }));
                Thread.Sleep(1000);
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = SystemColors.ControlBrush;
                    button.Content = buttonText;
                }));
            });
        }

        private void SetButtonWrong(Button button)
        {
            Task.Run(() =>
            {
                String buttonText = "Проверить";
                Brush backgroundColor = SystemColors.ControlBrush;
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = Brushes.Red;
                    button.Content = "Неверно";
                }));
                Thread.Sleep(1000);
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = SystemColors.ControlBrush;
                    button.Content = buttonText;
                }));
            });
        }

        private void SetButtonWrong(Button button, string buttonText)
        {
            Task.Run(() =>
            {
                Brush backgroundColor = SystemColors.ControlBrush;
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = Brushes.Red;
                    button.Content = "Неверно";
                }));
                Thread.Sleep(1000);
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = SystemColors.ControlBrush;
                    button.Content = buttonText;
                }));
            });
        }

        private void buttonCheckTransitionTable_Click(object sender, RoutedEventArgs e)
        {
            var userGraphTabel = transitionTable.DataContext as DataTable;

            if (graphTable == null || userGraphTabel == null)
                return;

            if (CompareTables(graphTable, userGraphTabel))
            {
                SetButtonRight(buttonCheckTransitionTable);
            }
            else
            {
                SetButtonWrong(buttonCheckTransitionTable);
            }
        }

        private int GetState(string word, GraphTable table)
        {
            var state = 0;
            foreach(var ch in word)
            {
                state = table.dtran[(state, ch)];
            }

            return state;
        }

        private void buttonStateCheck_Click(object sender, RoutedEventArgs e)
        {
            var userState = textBoxState.Text;

            if (String.IsNullOrEmpty(wordForCheck))
                return;

            var realyState = GetState(wordForCheck, graphTable);

            if(userState == realyState.ToString())
            {
                SetButtonRight(buttonStateCheck);
            }
            else
            {
                SetButtonWrong(buttonStateCheck);
            }
        }

        private void buttonQuestionYes_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(questionWord))
                return;

            var realyState = GetState(questionWord, graphTable);
            foreach(var state in graphTable.acceptedStates)
            {
                if(state == realyState)
                {
                    SetButtonRight(buttonQuestionYes, "Да");
                    return;
                }
            }

            SetButtonWrong(buttonQuestionYes, "Да");
        }

        private void buttonQuestionNo_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(questionWord))
                return;

            var realyState = GetState(questionWord, graphTable);
            foreach (var state in graphTable.acceptedStates)
            {
                if (state == realyState)
                {
                    SetButtonWrong(buttonQuestionNo, "Нет");
                    return;
                }
            }

            SetButtonRight(buttonQuestionNo, "Нет");
        }
    }
}
