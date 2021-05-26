using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core
{
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
            foreach (Edge e in outLinks)
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
            foreach (Edge e in outLinks)
            {
                if (e.getLabel() == x)
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
}
