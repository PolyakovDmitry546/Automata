using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Automata.Core
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

        public void RandomGenerate()
        {
            var random = new Random();
            Generate(random.Next(3, 6), random.Next(3, 7), random.Next(1, 4));
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

        public bool Compare(DataTable dataTable)
        {
            if (dataTable == null)
                return false;

            if (columnsCount != dataTable.Columns.Count - 1)
                return false;

            if (rowsCount != dataTable.Rows.Count)
                return false;

            for (int i = 0; i < rowsCount; i++)
            {
                if ((String)dataTable.Rows[i]["Состояние"] != i.ToString())
                    return false;
                for (int j = 0; j < columnsCount; j++)
                {
                    char ch = (char)((int)'a' + j);
                    if (dtran[(i, ch)].ToString()
                        != (String)dataTable.Rows[i][ch.ToString()])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Генерирует случайное слово из символов алфавита.
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLenght"></param>
        /// <returns>
        /// Слово в виде строки.
        /// </returns>
        public string GenerateWord(int minLength = 5, int maxLenght = 9)
        {
            var random = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < random.Next(minLength, maxLenght); i++)
            {
                stringBuilder.Append(alphabet.ElementAt(random.Next(alphabet.Count)));
            }

            return stringBuilder.ToString();
        }

        public int GetState(string word)
        {
            var state = 0;
            foreach (var ch in word)
            {
                state = dtran[(state, ch)];
            }

            return state;
        }

        public bool Recognize(string word)
        {
            var state = GetState(word);
            foreach (var s in acceptedStates)
            {
                if (s == state)
                {
                    return true;
                }
            }

            return false;
        }

        public void ConvertToAdditional()
        {
            var newAcceptedStates = new List<int>(rowsCount);
            for(int i = 0; i < rowsCount; i++)
            {
                newAcceptedStates.Add(i);
            }
            foreach(var i in acceptedStates)
            {
                newAcceptedStates.Remove(i);
            }
            acceptedStates = newAcceptedStates;
        }
    }
}
