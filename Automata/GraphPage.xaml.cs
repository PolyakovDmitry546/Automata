using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using Microsoft.Msagl.Drawing;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для GraphPage.xaml
    /// </summary>
    public partial class GraphPage : Page
    {
        public GraphPage()
        {
            InitializeComponent();
        }

        private List<Microsoft.Msagl.Drawing.Edge> notBlackEdges = new List<Microsoft.Msagl.Drawing.Edge>();
        private List<Node> notBlackNodes = new List<Node>();

        public Graph CreateGraph(GraphTable table)
        {
            Graph graph = new Graph();

            for (int i = 0; i < table.rowsCount; i++)
            {
                foreach (var symbol in table.alphabet)
                {
                    bool addNewEdge = true;
                    foreach (var edge in graph.Edges)
                    {
                        if (edge.Source == i.ToString() && edge.Target == table.dtran[(i, symbol)].ToString())
                        {
                            edge.LabelText += "," + symbol.ToString();
                            addNewEdge = false;
                        }
                    }
                    if (addNewEdge)
                        graph.AddEdge(i.ToString(), symbol.ToString(), table.dtran[(i, symbol)].ToString());
                }
            }

            foreach (var node in graph.Nodes)
            {
                bool isAcepted = false;
                foreach (var st in table.acceptedStates)
                {
                    if (st.ToString() == node.LabelText)
                    {
                        node.Attr.Shape = Shape.DoubleCircle;
                        //node.LabelText += "*";
                        isAcepted = true;
                        break;
                    }
                }
                if (!isAcepted)
                    node.Attr.Shape = Shape.Circle;
            }

            graph.Attr.LayerDirection = LayerDirection.LR;

            return graph;
        }

        public void ShowGraph(GraphTable table)
        {
            notBlackEdges = new List<Microsoft.Msagl.Drawing.Edge>();
            notBlackNodes = new List<Node>();

            var graph = CreateGraph(table);
            graphControl.Graph = null;
            graphControl.Graph = graph;
        }

        public void CheckWord(string word)
        {
            Graph graph = null;
            Dispatcher.Invoke((Action)(() =>
            {
                graph = graphControl.Graph;
            }));

            if (graph == null)
                return;

            Dispatcher.Invoke((Action)(() =>
            {
                foreach (var edge in notBlackEdges)
                {
                    edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Black;
                }
                foreach (var nd in notBlackNodes)
                {
                    nd.Attr.FillColor = Microsoft.Msagl.Drawing.Color.White;
                }
            }));

            var node = graph.FindNode("0");
            foreach (var c in word)
            {
                bool selfEdge = true;
                foreach (var edge in node.OutEdges)
                {
                    if (edge.LabelText.Contains(c))
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                        }));
                        node = edge.TargetNode;
                        selfEdge = false;
                        System.Threading.Thread.Sleep(1500);
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            edge.Attr.Color = Microsoft.Msagl.Drawing.Color.YellowGreen;
                        }));

                        notBlackEdges.Add(edge);
                        break;
                    }
                }
                if (selfEdge)
                {
                    foreach (var edge in node.SelfEdges)
                    {
                        if (edge.LabelText.Contains(c))
                        {
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                            }));
                            System.Threading.Thread.Sleep(1500);
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                edge.Attr.Color = Microsoft.Msagl.Drawing.Color.YellowGreen;
                            }));

                            notBlackEdges.Add(edge);
                            break;
                        }
                    }
                }
            }
            Dispatcher.Invoke((Action)(() =>
            {
                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Green;
            }));
            notBlackNodes.Add(node);
        }
    }
}
