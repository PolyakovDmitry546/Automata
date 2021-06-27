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
using System.Windows.Media;
using Automata.Core;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class ExamplePage : Page
    {
        GraphPage noMinimazeGraphPage;
        GraphPage minimazeGraphPage;
        GraphPage additionalAutomataGraphPage;
        DataTable ATable;

        public ExamplePage()
        {
            InitializeComponent();
            noMinimazeGraphPage = new GraphPage();
            minimazeGraphPage = new GraphPage();
            additionalAutomataGraphPage = new GraphPage();
            frameNoMinimazeGraph.Navigate(noMinimazeGraphPage);
            frameMinimazeGraph.Navigate(minimazeGraphPage);
            frameAdditionalGraph.Navigate(additionalAutomataGraphPage);
        }

        private void CreateGraphNoMinimaze(GraphTable table)
        {
            noMinimazeGraphPage.ShowGraph(table);
        }

        private void CreateGraphMinimaze(GraphTable table)
        {
            minimazeGraphPage.ShowGraph(table);
        }

        private void CreateGraphAdditionalAutomata(GraphTable table)
        {
            additionalAutomataGraphPage.ShowGraph(table);
        }

        private void SetButtonWrongSymbol(Button button)
        {
            Task.Run(() =>
            {
                String buttonText = "Построить автомат";
                Brush backgroundColor = SystemColors.ControlBrush;
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = Brushes.Red;
                    button.Content = "Некорректный символ";
                }));
                Thread.Sleep(1000);
                Dispatcher.Invoke((Action)(() =>
                {
                    button.Background = SystemColors.ControlBrush;
                    button.Content = buttonText;
                }));
            });
        }

        private void RegexButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var expression = RegexTextBox.Text;
                if (expression == "")
                {
                    return;
                }
                foreach (var ch in expression)
                {
                    if (!(ch == '*' || ch == '|' || ch == '(' || ch == ')' || (ch >= 'a' && ch <= 'z')))
                    {
                        SetButtonWrongSymbol(RegexButton);
                        return;
                    }
                }

                AutomataTable.Columns.Clear();
                //AutomataTable.ItemsSource = null;
                //AutomataTable.Items.Refresh();
                var t = TreeCreator.CreateTree(expression);
                NFA testNfa = NFACreator.Create(t);
                testNfa.NumberAllStates();
                DFA dFA = new DFA();
                dFA.Create(testNfa);

                var graphTableNoMin = new GraphTable(dFA.dtran, dFA.acceptedStates, testNfa.GetAlphabet());
                CreateGraphNoMinimaze(graphTableNoMin);

                dFA.Minmization();
                ATable = new DataTable();
                ATable.Columns.Add("Состояние");
                foreach (var ch in testNfa.GetAlphabet())
                {
                    ATable.Columns.Add(ch.ToString());
                }
                foreach (DataColumn col in ATable.Columns)
                {
                    AutomataTable.Columns.Add(new DataGridTextColumn()
                    {
                        Header = col.ColumnName,
                        Binding = new Binding(String.Format("[{0}]", col.ColumnName))
                    });
                }
                AutomataTable.DataContext = ATable;

                //построение таблицы автомата
                for (int i = 0; i < dFA.dtran.Count / testNfa.GetAlphabet().Count; i++)
                {
                    string str = i.ToString();
                    foreach (var st in dFA.acceptedStates)
                    {
                        if (st == i)
                        {
                            str = i + "*";
                            break;
                        }
                    }
                    var item = new List<string>();
                    item.Add(str);
                    foreach (var symbol in testNfa.GetAlphabet())
                    {
                        item.Add(dFA.dtran[(i, symbol)].ToString());
                    }
                    ATable.Rows.Add(item.ToArray());
                }

                var graphTableMin = new GraphTable(dFA.dtran, dFA.acceptedStates, testNfa.GetAlphabet());
                CreateGraphMinimaze(graphTableMin);

                var graphTableAdditionalAutomata = new GraphTable(dFA.dtran, dFA.acceptedStates, testNfa.GetAlphabet());
                graphTableAdditionalAutomata.ConvertToAdditional();
                CreateGraphAdditionalAutomata(graphTableAdditionalAutomata);
            }
            catch(Exception)
            { }
        }

        private void RegexButtonCheckWord_Click(object sender, RoutedEventArgs e)
        {
            var word = wordRegexTextBox.Text;
            Task.Run(() => noMinimazeGraphPage.CheckWord(word));
            Task.Run(() => minimazeGraphPage.CheckWord(word));
            Task.Run(() => additionalAutomataGraphPage.CheckWord(word));
        }
    }
}
