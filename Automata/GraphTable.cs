using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    public class GraphTable
    {
        public Dictionary<(int, char), int> dtran;
        public List<int> acceptedStates;
        public SortedSet<char> alphabet;
        public int rowsCount;
        public int columnsCount;

        public GraphTable()
        {
            this.dtran = new Dictionary<(int, char), int>();
            this.acceptedStates = new List<int>();
            this.alphabet = new SortedSet<char>();
            this.rowsCount = 0;
            this.columnsCount = 0;
        }

        public GraphTable(Dictionary<(int, char), int> dtran, List<int> acceptedStates, SortedSet<char> alphabet)
        {
            this.dtran = dtran;
            this.acceptedStates = acceptedStates;
            this.alphabet = alphabet;
            this.columnsCount = alphabet.Count;
            this.rowsCount = dtran.Count / alphabet.Count;
        }

        public void Generate(int alphabetSize, int numberState, int numberAcceptedState)
        {
            this.dtran = new Dictionary<(int, char), int>();
            this.acceptedStates = new List<int>();
            this.alphabet = new SortedSet<char>();

            var rand = new Random();
            int a = (int)'a';

            for (int i = 0; i < alphabetSize; i++)
            {
                alphabet.Add((char)(a + i));
            }

            var notAcceptedStates = new List<int>();
            for (int i = 0; i < numberState; i++)
            {
                foreach(var j in alphabet)
                {
                    dtran[(i, j)] = rand.Next(numberState);
                }
                notAcceptedStates.Add(i);
            }

            for(int i = 0; i < numberAcceptedState; i++)
            {
                int number = rand.Next(notAcceptedStates.Count);
                acceptedStates.Add(notAcceptedStates[number]);
                notAcceptedStates.RemoveAt(number);
            }

            this.columnsCount = alphabet.Count;
            this.rowsCount = dtran.Count / alphabet.Count;
        }
    }
}
