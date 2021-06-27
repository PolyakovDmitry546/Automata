using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Automata.Core;

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

        private void buttonNewTask_Click(object sender, RoutedEventArgs e)
        {
            graphTable = new GraphTable();
            var random = new Random();
            graphTable.RandomGenerate();
            var graphPage = DataContext as GraphPage;
            graphPage.ShowGraph(graphTable);

            var i = random.Next(0, 2);
            if (i == 0)
            {
                wordForCheck = graphTable.GenerateWord();
                textBlockTask.Text = "Задан конечный автомат-распознаватель диаграммой Мура. " +
                    $"Построить таблицу переходов.\nНа входе слово {wordForCheck}. " +
                    "В каком состоянии автомат закончит обработку входного слова?";

                questionWord = graphTable.GenerateWord();
                textBlockQuestion.Text = $"Распознает ли автомат слово {questionWord}?";
            }
            else
            {
                graphTable.ConvertToAdditional();
                wordForCheck = graphTable.GenerateWord();
                textBlockTask.Text = "Задан конечный автомат-распознаватель диаграммой Мура. " +
                    $"Построить таблицу переходов дополнительного автомата.\nНа входе слово {wordForCheck}. " +
                    "В каком состоянии дополнительный автомат закончит обработку входного слова?";

                questionWord = graphTable.GenerateWord();
                textBlockQuestion.Text = $"Распознает ли дополнительный автомат слово {questionWord}?";
            }
            transitionTable.DataContext = CreateUserGraphTable(graphTable);
        }

        private async void buttonCheckWord_Click(object sender, RoutedEventArgs e)
        {
            var text = textboxCheckWord.Text;
            var graphPage = DataContext as GraphPage;
            await Task.Run(() => graphPage.CheckWord(text));
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

            if (graphTable.Compare(userGraphTabel))
            {
                SetButtonRight(buttonCheckTransitionTable);
            }
            else
            {
                SetButtonWrong(buttonCheckTransitionTable);
            }
        }

        private void buttonStateCheck_Click(object sender, RoutedEventArgs e)
        {
            var userState = textBoxState.Text;

            if (String.IsNullOrEmpty(wordForCheck))
                return;

            var realyState = graphTable.GetState(wordForCheck);

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

            if (graphTable.Recognize(questionWord))
            {
                SetButtonRight(buttonQuestionYes, "Да");
            }
            else
            {
                SetButtonWrong(buttonQuestionYes, "Да");
            }
        }

        private void buttonQuestionNo_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(questionWord))
                return;

            if (graphTable.Recognize(questionWord))
            {
                SetButtonWrong(buttonQuestionNo, "Нет");
            }
            else
            {
                SetButtonRight(buttonQuestionNo, "Нет");
            }
        }
    }
}
