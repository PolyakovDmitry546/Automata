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
using Automata.Core;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        GraphPage noMinimazeGraphPage;
        GraphPage minimazeGraphPage;
        DataTable ATable;

        public TablePage()
        {
            InitializeComponent();
            noMinimazeGraphPage = new GraphPage();
            minimazeGraphPage = new GraphPage();
            frameNoMinimazeGraph.Navigate(noMinimazeGraphPage);
            frameMinimazeGraph.Navigate(minimazeGraphPage);
        }

        private void CreateGraphNoMinimaze(GraphTable table)
        {
            noMinimazeGraphPage.ShowGraph(table);
        }

        private void CreateGraphMinimaze(GraphTable table)
        {
            minimazeGraphPage.ShowGraph(table);
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
            }
            catch(Exception)
            { }
        }

        private void RegexButtonCheckWord_Click(object sender, RoutedEventArgs e)
        {
            var word = wordRegexTextBox.Text;
            Task.Run(() => noMinimazeGraphPage.CheckWord(word));
            Task.Run(() => minimazeGraphPage.CheckWord(word));
        }
    }


    public class Edge
    {

        private char label;
        private State source;
        private State target;

        /**
         * Constructor.
         *
         * @param source
         * @param target
         *
         */
        public Edge(State source, State target, char label)
        {
            this.source = source;
            this.target = target;
            this.label = label;
        }

        /**
         * Return the source of this edge.
         */
        public State getSource()
        {
            return source;
        }

        /**
         * Return the target of this edge.
         */
        public State getTarget()
        {
            return target;
        }

        /**
         * Return the label.
         */
        public char getLabel()
        {
            return label;
        }

    }

    public class State
    {

        private bool acceptState = false;
        private int number = -1;
        private List<Edge> outLinks = new List<Edge>();
        private List<Edge> inLinks = new List<Edge>();

        /**
         * Validates whether the this state is an accept state.
         *
         * @return the isAccept
         */

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public bool isAccept()
        {
            return acceptState;
        }

        /**
         * Set this as an accept state.
         */
        public void makeAccept()
        {
            this.acceptState = true;
        }

        /**
         * Set as a non-accept state.
         */
        public void notAccept()
        {
            this.acceptState = false;
        }

        /**
         * Returns the list of this state's outgoing links (edges).
         *
         * @return the outLinks
         */
        public List<Edge> getOutLinks()
        {
            return outLinks;
        }

        /**
         * Add a link (edge) to the list of outgoing links (edges).
         *
         * @param e the outgoing edge to be added.
         */
        public void addOutLink(Edge e)
        {
            this.outLinks.Add(e);
        }

        /**
         * Returns the list of this state's incoming links (edges).
         *
         * @return the inLinks
         */
        public List<Edge> getInLinks()
        {
            return inLinks;
        }

        /**
         * Add a link (edge) to the list of incoming links.
         *
         * @param e the incoming edge to be added.
         */
        public void addInLink(Edge e)
        {
            this.inLinks.Add(e);
        }

        /**
         * Checks whether this state contains an outgoing link (edge) labeled x.
         *
         * @param x edge to be checked.
         * @return
         */
        public bool containsOutLink(char x)
        {
            foreach(Edge e in outLinks)
            {
                if (e.getLabel() == x)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Makes a transition from the current state to the next state through input
         * character x (edge label), and returns the new state.
         *
         * @param x the edge label through which the transition will be made
         * @return new state
         */
        public State transition(char x)
        {

            State ret = new State();

            foreach (Edge e in outLinks)
            {
                if (e.getLabel() == x)
                {
                    ret = e.getTarget();
                }
            }
            return ret;
        }

        /**
         * Returns the source of the edge whose label is passed as an argument.
         *
         * @param x edge label
         * @return source of the edge x that is an inLink for the currentState
         */
        public State sourceOf(char x)
        {

            State ret = new State();

            foreach (Edge e in inLinks)
            {
                if (e.getLabel() == x)
                {
                    ret = e.getSource();
                }
            }
            return ret;
        }

        /**
         * Removes outLink.
         *
         * @param x edge label whose corresponding edge will be removed.
         *
         */
        public void removeOutLink(char x)
        {
            foreach(Edge e in outLinks)
            {
                if(e.getLabel() == x)
                {
                    outLinks.Remove(e);
                }
            }
        }

    }

    public class NFA
    {
        private State initialState;
        private State currentState;
        private int countState = 0;
        SortedSet<char> alphabet = new SortedSet<char>();

        /**
         * Constructor.
         *
         * Creates an empty NFA by instantiating a start state.
         */
        public NFA()
        {
            initialState = new State();
            currentState = initialState;
        }

        public SortedSet<char> GetAlphabet()
        {
            return alphabet;
        }

        public void AddChar(char ch)
        {
            alphabet.Add(ch);
        }
        /**
         * Returns initialState.
         */
        public State getInitialState()
        {
            return initialState;
        }

        /**
         * Returns currentState.
         */
        public State getCurrentState()
        {
            return currentState;
        }

        /**
         * Removes the initial state. Used when connecting a capturing group to the
         * rest of the automaton.
         */
        public void removeInitial()
        {
            initialState = null;
        }

        public void Concat(NFA nFA)
        {
            Edge epsilon = new Edge(currentState, nFA.getInitialState(), 'E');
            currentState.addOutLink(epsilon);
            nFA.getInitialState().addInLink(epsilon);
            currentState.notAccept();
            nFA.removeInitial();
            currentState = nFA.getCurrentState();
        }

        public void alternation(NFA nfa)
        {
            State s1 = new State();
            State s2 = new State();
            Edge epsilon1 = new Edge(s1, initialState, 'E');
            Edge epsilon2 = new Edge(s1, nfa.getInitialState(), 'E');
            s1.addOutLink(epsilon1);
            s1.addOutLink(epsilon2);
            initialState.addInLink(epsilon1);
            nfa.getInitialState().addInLink(epsilon2);
            initialState = s1;
            nfa.removeInitial();
            s2.makeAccept();
            Edge epsilon3 = new Edge(currentState, s2, 'E');
            Edge epsilon4 = new Edge(nfa.getCurrentState(), s2, 'E');
            currentState.addOutLink(epsilon3);
            s2.addInLink(epsilon3);
            nfa.getCurrentState().addOutLink(epsilon4);
            s2.addInLink(epsilon4);
            nfa.getCurrentState().notAccept();
            currentState.notAccept();
            currentState = s2;
        }

        public void BaseElem(char x)
        {
            State s = new State();
            Edge edge = new Edge(initialState, s, x);
            initialState.addOutLink(edge);
            s.addInLink(edge);
            initialState.notAccept();
            s.makeAccept();
            currentState = s;
        }

        public void KleeneStar()
        {
            Edge edge = new Edge(currentState, initialState, 'E');
            currentState.addOutLink(edge);
            initialState.addInLink(edge);
            State start = new State();
            edge = new Edge(start, initialState, 'E');
            start.addOutLink(edge);
            initialState.addInLink(edge);
            initialState = start;
            State finish = new State();
            edge = new Edge(currentState, finish, 'E');
            currentState.addOutLink(edge);
            finish.addInLink(edge);
            currentState.notAccept();
            finish.makeAccept();
            currentState = finish;
            edge = new Edge(start, finish, 'E');
            start.addOutLink(edge);
            finish.addInLink(edge);
        }

        void NumberState(State state)
        {
            if (state.Number == -1)
            {
                state.Number = countState;
                countState++;
            }
            foreach (var e in state.getOutLinks())
            {
                if (e.getTarget().Number == -1)
                    NumberState(e.getTarget());
            }
        }

        public void NumberAllStates()
        {
            NumberState(initialState);
        }
    }

    public class NFACreator
    {
        public static NFA Create(BinaryTree tree)
        {
            if (tree == null)
                return null;
            if (tree.GetValue() == '.')
            {
                var leftNFA = Create(tree.GetLeftChild());
                var rightNFA = Create(tree.GetRightChild());
                leftNFA.Concat(rightNFA);
                foreach (var e in rightNFA.GetAlphabet())
                    leftNFA.AddChar(e);
                return leftNFA;
            }
            else if (tree.GetValue() == '|')
            {
                var leftNFA = Create(tree.GetLeftChild());
                var rightNFA = Create(tree.GetRightChild());
                leftNFA.alternation(rightNFA);
                foreach (var e in rightNFA.GetAlphabet())
                    leftNFA.AddChar(e);
                return leftNFA;
            }
            else if (tree.GetValue() == '*')
            {
                var leftNFA = Create(tree.GetLeftChild());
                leftNFA.KleeneStar();
                return leftNFA;
            }
            else
            {
                var nfa = new NFA();
                nfa.BaseElem(tree.GetValue());
                nfa.AddChar(tree.GetValue());
                return nfa;
            }
        }
    }

    public class Dstate
    {
        public int number;
        public bool marker;
        public List<State> states;
    }

    public class DFA
    {
        public List<Dstate> dstates;
        public Dictionary<(int, char), int> dtran;
        public List<int> acceptedStates;

        /// <summary>
        /// Возвращает множество состояний НКА, достижимых из состояния state при одном e-переходе
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static List<State> EClosure(State state)
        {
            var states = new List<State>();
            states.Add(state);
            return EClosure(states);
        }

        /// <summary>
        /// Возвращает множество состояний НКА, достижимых из множества состояния states при одном e-переходе
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        public static List<State> EClosure(List<State> states)
        {
            List<State> setAccessibleStates = new List<State>(states);
            Stack<State> stackStates = new Stack<State>();
            foreach(var s in states)
            {
                stackStates.Push(s);
            }
            while (stackStates.Count != 0) 
            {
                var t = stackStates.Pop();
                foreach(var e in t.getOutLinks())
                {
                    if(e.getLabel() == 'E' && !setAccessibleStates.Contains(e.getTarget()))
                    {
                        setAccessibleStates.Add(e.getTarget());
                        stackStates.Push(e.getTarget());
                    }
                }
            }
            setAccessibleStates.Sort(delegate (State x, State y)
            {
                if (x.Number == y.Number)
                    return 0;
                else if (x.Number > y.Number)
                    return 1;
                else return -1;
            });
            return setAccessibleStates;
        }

        /// <summary>
        /// Возвращает множество состояний НКА, достижимых из множества состояния states при входном символе a
        /// </summary>
        /// <param name="states"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static List<State> Move(List<State> states, char a)
        {
            List<State> setAccessibleStates = new List<State>();
            foreach(var s in states)
            {
                foreach(var e in s.getOutLinks())
                {
                    if(e.getLabel() == a && !setAccessibleStates.Contains(e.getTarget()))
                    {
                        setAccessibleStates.Add(e.getTarget());
                    }
                }
            }
            return setAccessibleStates;
        }

        /// <summary>
        /// Возвращает индекс первого непомеченного состояния из множества dstates
        /// </summary>
        /// <param name="dstates"></param>
        /// <returns></returns>
        private int UnmarkedState(List<Dstate> dstates)
        {
            int i = 0;
            foreach(var d in dstates)
            {
                if (d.marker == false)
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Возвращает номер состояния dstate если оно есть в dstates иначе -1
        /// </summary>
        /// <param name="dstates"></param>
        /// <param name="dstate"></param>
        /// <returns></returns>
        private int HasDState(List<Dstate> dstates, Dstate dstate)
        {
            foreach (var d in dstates)
            {
                if (d.states.Count == dstate.states.Count)
                {
                    bool b = true;
                    for (int i = 0; i < d.states.Count; i++)
                    {
                        if (d.states[i].Number != dstate.states[i].Number)
                        {
                            b = false;
                            break;
                        }
                    }
                    if(b)
                        return d.number;
                }
            }
            return -1;
        }


        /// <summary>
        /// Строит детерминированный конечный автомат по недетерминированному кончечному автомату nFA
        /// </summary>
        /// <param name="nFA"></param>
        public void Create(NFA nFA)
        {
            dstates = new List<Dstate>();
            dtran = new Dictionary<(int, char), int>();
            acceptedStates = new List<int>();
            Dstate dstate = new Dstate()
            {
                number = 0,
                marker = false,
                states = EClosure(nFA.getInitialState())
            };
            int countState = 1;
            dstates.Add(dstate);
            while(true)
            {
                var di = UnmarkedState(dstates);
                if (di == -1)
                    break;
                dstates[di].marker = true;
                foreach(var ch in nFA.GetAlphabet())
                {
                    dstate = new Dstate()
                    {
                        states = EClosure(Move(dstates[di].states, ch))
                    };
                    dstate.number = HasDState(dstates, dstate);
                    if (dstate.number == -1)
                    {
                        dstate.marker = false;
                        dstate.number = countState;
                        countState++;
                        dstates.Add(dstate);
                    }
                    dtran[(dstates[di].number, ch)] = dstate.number;
                }
            }
            foreach(var d in dstates)
            {
                foreach (var d1 in d.states)
                {
                    if (d1.isAccept())
                    {
                        acceptedStates.Add(d.number);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет есть ли в acceptedStates состояние с номером numberState
        /// </summary>
        /// <param name="numberState"></param>
        /// <returns></returns>
        private bool FindStateInAcceptedStates(int numberState)
        {
            foreach (var number in acceptedStates)
                if (number == numberState)
                    return true;

            return false;
        }

        /// <summary>
        /// Возвращает группу в которую входит состояние numberState, если ничего не найдено null
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="numberState"></param>
        /// <returns></returns>
        private Dictionary<int, Dictionary<char, int>> FindGroup(List<Dictionary<int, Dictionary<char, int>>> groups, int numberState)
        {
            foreach(var group in groups)
            {
                if (group.ContainsKey(numberState))
                    return group;
            }

            return null;
        }

        /// <summary>
        /// Возвращает подходящую для состояния подгруппу если такая есть, иначе null
        /// </summary>
        /// <param name="newGroups"></param>
        /// <param name="Groups"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private Dictionary<int, Dictionary<char, int>> FindSubGroup(List<Dictionary<int, Dictionary<char, int>>> newGroups,
            List<Dictionary<int, Dictionary<char, int>>> Groups, Dictionary<char, int> state)
        {
            foreach(var group in newGroups)
            {
                bool trueGroup = true;
                //берем первое состояние из каждой группы
                var firsState = group.First().Value;
                foreach(var key in firsState.Keys)
                {
                    if (firsState[key] == state[key])
                        continue;
                    else
                    {
                        var group1 = FindGroup(Groups, firsState[key]);
                        var group2 = FindGroup(Groups, state[key]);
                        if(group1 != group2)
                        {
                            trueGroup = false;
                            break;
                        }
                    }
                }

                if (trueGroup)
                    return group;
            }

            return null;
        }

        /// <summary>
        /// Разделяет группы на подгруппы
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private List<Dictionary<int, Dictionary<char, int>>> SeparationGroups(List<Dictionary<int, Dictionary<char, int>>> groups,
            Dictionary<int, Dictionary<char, int>> group)
        {
            var newGroups = new List<Dictionary<int, Dictionary<char, int>>>();

            foreach(var state in group)
            {
                if(newGroups.Count == 0)
                {
                    newGroups.Add(new Dictionary<int, Dictionary<char, int>>());
                    newGroups[newGroups.Count - 1].Add(state.Key, state.Value);
                }
                else
                {
                    var subgroup = FindSubGroup(newGroups, groups, state.Value);
                    if(subgroup == null)
                    {
                        newGroups.Add(new Dictionary<int, Dictionary<char, int>>());
                        newGroups[newGroups.Count - 1].Add(state.Key, state.Value);
                    }
                    else
                    {
                        subgroup.Add(state.Key, state.Value);
                    }
                }
            }

            return newGroups;
        }

        /// <summary>
        /// Возвращает true если группы одинаковые
        /// </summary>
        /// <param name="group1"></param>
        /// <param name="group2"></param>
        /// <returns></returns>
        bool CompareGroups(List<Dictionary<int, Dictionary<char, int>>> group1, List<Dictionary<int, Dictionary<char, int>>> group2)
        {
            if (group1.Count != group2.Count)
                return false;
            for (int i = 0; i < group1.Count; i++)
            {
                if (group1[i].Count != group2[i].Count)
                    return false;
            }

            for (int i = 0; i < group1.Count; i++)
            {
                foreach(var key in group1[i].Keys)
                {
                    if (!group2[i].ContainsKey(key))
                        return false;
                }
            }

            return true;
        }

        int FindStartState(List<Dictionary<int, Dictionary<char, int>>> groups)
        {
            for(int i = 0; i < groups.Count; i++)
            {
                foreach(var key in groups[i].Keys)
                {
                    if (key == 0)
                        return i;
                }
            }

            return -1;
        }

        List<int> FindAcceptedtStates(List<Dictionary<int, Dictionary<char, int>>> groups)
        {
            var newAcceptedStates = new List<int>();
            for (int i = 0; i < groups.Count; i++)
            {
                foreach(var state in acceptedStates)
                {
                    if(groups[i].ContainsKey(state))
                    {
                        newAcceptedStates.Add(i);
                        break;
                    }
                }
            }

            return newAcceptedStates;
        }

        int FindNewState(List<Dictionary<int, Dictionary<char, int>>> groups, int state)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                foreach (var key in groups[i].Keys)
                {
                    if (key == state)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        bool ZeroStateIsACcepted()
        {
            foreach(var state in acceptedStates)
            {
                if (state == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Мнимизирует детерминированный конечный автомат
        /// </summary>
        /// <param name="alphabet"></param>
        public void Minmization()
        {

            //множество завершающих состояний
            Dictionary<int, Dictionary<char, int>> acceptedStates = new Dictionary<int, Dictionary<char, int>>();

            //множество незавершающих состояний
            Dictionary<int, Dictionary<char, int>> notAcceptedStates = new Dictionary<int, Dictionary<char, int>>();

            //разделение всех состояний на множества завершаюших и не завершающих состояний
            foreach (var elem in dtran)
                if (FindStateInAcceptedStates(elem.Key.Item1))
                {
                    if (!acceptedStates.ContainsKey(elem.Key.Item1))
                    {
                        acceptedStates.Add(elem.Key.Item1, new Dictionary<char, int>());
                    }
                    acceptedStates[elem.Key.Item1].Add(elem.Key.Item2, elem.Value);
                }
                else
                {
                    if (!notAcceptedStates.ContainsKey(elem.Key.Item1))
                    {
                        notAcceptedStates.Add(elem.Key.Item1, new Dictionary<char, int>());
                    }
                    notAcceptedStates[elem.Key.Item1].Add(elem.Key.Item2, elem.Value);
                }

            //группы состояний
            var groups = new List<Dictionary<int, Dictionary<char, int>>>();
            var newGroups = new List<Dictionary<int, Dictionary<char, int>>>();

            //проверка ноль завершающие или нет состояние
            //нужно чтобы ноль остался в нулевом состоянии
            if(ZeroStateIsACcepted())
            {
                //добавление первых двух групп состояний
                newGroups.Add(acceptedStates);
                newGroups.Add(notAcceptedStates);
            }
            else
            {
                //добавление первых двух групп состояний
                newGroups.Add(notAcceptedStates);
                newGroups.Add(acceptedStates);
            }

            //разбиваем пока groups и new groups не равны
            while (!CompareGroups(groups, newGroups))
            {
                groups = newGroups;
                newGroups = new List<Dictionary<int, Dictionary<char, int>>>();

                foreach (var group in groups)
                {
                    var subGroups = SeparationGroups(groups, group);
                    foreach (var subGroup in subGroups)
                        newGroups.Add(subGroup);
                }
            }

            groups = newGroups;

            //построение новых состояний
            var newDtran = new Dictionary<(int, char), int>();
            var newAcceptedStates = FindAcceptedtStates(groups);
            var startState = FindStartState(groups);
            for(int i = 0; i < groups.Count; i++)
            { 
                foreach(var key in groups[i].Keys)
                {
                    foreach(var value in groups[i][key].Keys)
                    {
                        newDtran[(i, value)] = FindNewState(groups, groups[i][key][value]);
                    }
                    break;
                }
                
            }

            this.acceptedStates = newAcceptedStates;
            dtran = newDtran;
        }
    }

    public static class TreeCreator
    {
        static List<(int, int)> allPriorities;
        static string str;

        static string ConcatMarker(string s)
        {
            StringBuilder newS = new StringBuilder();
            newS.Append(s[0]);
            for (int i = 1; i < s.Length; i++) 
            {
                if(Char.IsLetterOrDigit(s[i]) && Char.IsLower(s[i]) || s[i] == '(')
                {
                    if (Char.IsLetterOrDigit(s[i - 1]) && Char.IsLower(s[i - 1]) || s[i - 1] == ')' || s[i - 1] == '*')
                    {
                        newS.Append('.');
                    }
                }
                newS.Append(s[i]);
            }
            return newS.ToString();
        }

        static List<(int,int)> GetOperationPriority(string s)
        {
            int priority = 0;
            var priorities = new List<(int, int)>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(')
                    priority += 3;
                else if (s[i] == ')')
                    priority -= 3;
                else if (s[i] == '|')
                {
                    priorities.Add((i, priority));
                }
                else if (s[i] == '.')
                {
                    priorities.Add((i, priority + 1));
                }
                else if (s[i] == '*')
                {
                    priorities.Add((i, priority + 2));
                }
            }
            return priorities;
        }

        static int FindMinPriorityIndex(int start, int end, List<(int,int)> priorities)
        {
            var minP = int.MaxValue;
            var minI = -1;
            for(int i = 0; i < priorities.Count; i++)
            {
                if (priorities[i].Item1 < start)
                    continue;
                if (priorities[i].Item1 > end)
                    break;
                if (priorities[i].Item2 < minP) 
                {
                    minP = priorities[i].Item2;
                    minI = i;
                }
            }
            return minI;
        }

        static BinaryTree CreateTreeBranch(int start, int end)
        {
            if (start > end)
                return null;
            var separatorIndex = FindMinPriorityIndex(start, end, allPriorities);
            if (separatorIndex == -1)
            {
                return new BinaryTree(str.Substring(start, end - start + 1).Trim(new char[] { '(', ')' })[0]);
            }
            var separator = allPriorities[separatorIndex].Item1;
            allPriorities.RemoveAt(separatorIndex);
            var tree = new BinaryTree(str[separator]);
            tree.SetLeftChild(CreateTreeBranch(start, separator - 1));
            tree.SetRightChild(CreateTreeBranch(separator + 1, end));
            return tree;
        }

        public static BinaryTree CreateTree(string sRegex)
        {
            str = ConcatMarker(sRegex.Trim());
            allPriorities = GetOperationPriority(str);
            return CreateTreeBranch(0, str.Length - 1);
        }
    }
}
