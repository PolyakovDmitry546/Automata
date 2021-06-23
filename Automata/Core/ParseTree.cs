using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Core
{
    public class BinaryTree
    {
        private char value;
        private BinaryTree leftNode;
        private BinaryTree rightNode;

        public BinaryTree(char value)
        {
            this.value = value;
        }

        public BinaryTree GetLeftChild()
        {
            return leftNode;
        }

        public void SetLeftChild(BinaryTree tree)
        {
            leftNode = tree;
        }

        public BinaryTree GetRightChild()
        {
            return rightNode;
        }

        public void SetRightChild(BinaryTree tree)
        {
            rightNode = tree;
        }

        public void SetValue(char value)
        {
            this.value = value;
        }

        public char GetValue()
        {
            return value;
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
                if (Char.IsLetterOrDigit(s[i]) && Char.IsLower(s[i]) || s[i] == '(')
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

        static List<(int, int)> GetOperationPriority(string s)
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

        static int FindMinPriorityIndex(int start, int end, List<(int, int)> priorities)
        {
            var minP = int.MaxValue;
            var minI = -1;
            for (int i = 0; i < priorities.Count; i++)
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
