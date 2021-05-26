using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core
{
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
            foreach (var s in states)
            {
                stackStates.Push(s);
            }
            while (stackStates.Count != 0)
            {
                var t = stackStates.Pop();
                foreach (var e in t.getOutLinks())
                {
                    if (e.getLabel() == 'E' && !setAccessibleStates.Contains(e.getTarget()))
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
            foreach (var s in states)
            {
                foreach (var e in s.getOutLinks())
                {
                    if (e.getLabel() == a && !setAccessibleStates.Contains(e.getTarget()))
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
            foreach (var d in dstates)
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
                    if (b)
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
            while (true)
            {
                var di = UnmarkedState(dstates);
                if (di == -1)
                    break;
                dstates[di].marker = true;
                foreach (var ch in nFA.GetAlphabet())
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
            foreach (var d in dstates)
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
            foreach (var group in groups)
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
            foreach (var group in newGroups)
            {
                bool trueGroup = true;
                //берем первое состояние из каждой группы
                var firsState = group.First().Value;
                foreach (var key in firsState.Keys)
                {
                    if (firsState[key] == state[key])
                        continue;
                    else
                    {
                        var group1 = FindGroup(Groups, firsState[key]);
                        var group2 = FindGroup(Groups, state[key]);
                        if (group1 != group2)
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

            foreach (var state in group)
            {
                if (newGroups.Count == 0)
                {
                    newGroups.Add(new Dictionary<int, Dictionary<char, int>>());
                    newGroups[newGroups.Count - 1].Add(state.Key, state.Value);
                }
                else
                {
                    var subgroup = FindSubGroup(newGroups, groups, state.Value);
                    if (subgroup == null)
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
                foreach (var key in group1[i].Keys)
                {
                    if (!group2[i].ContainsKey(key))
                        return false;
                }
            }

            return true;
        }

        int FindStartState(List<Dictionary<int, Dictionary<char, int>>> groups)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                foreach (var key in groups[i].Keys)
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
                foreach (var state in acceptedStates)
                {
                    if (groups[i].ContainsKey(state))
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
            foreach (var state in acceptedStates)
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
            if (ZeroStateIsACcepted())
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
            for (int i = 0; i < groups.Count; i++)
            {
                foreach (var key in groups[i].Keys)
                {
                    foreach (var value in groups[i][key].Keys)
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
}
