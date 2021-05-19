using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Shapes;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new Pages();
        }

        private void buttonTasks_Click(object sender, RoutedEventArgs e)
        {
            var pages = DataContext as Pages;
            pages.CurrentPage = pages.TasksPage;
        }

        private void buttonMain_Click(object sender, RoutedEventArgs e)
        {
            var pages = DataContext as Pages;
            pages.CurrentPage = pages.NavigationPage;
        }

        private void buttonExamples_Click(object sender, RoutedEventArgs e)
        {
            var pages = DataContext as Pages;
            pages.CurrentPage = pages.TablePage;
        }

        private void buttonTheory_Click(object sender, RoutedEventArgs e)
        {
            var pages = DataContext as Pages;
            pages.CurrentPage = pages.InformationPage;
        }
    }

    public class Pages:INotifyPropertyChanged
    {
        private Page currentPage;
        //private DrawingPage drawingPage;
        public InformftionPage InformationPage { get; }
        public TablePage TablePage { get; }
        public TasksPage TasksPage { get; }
        public NavigationPage NavigationPage { get; }

        public Page CurrentPage
        {
            get { return currentPage; }
            set
            {
                if(value!=currentPage)
                {
                    currentPage = value;
                    NotifyPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public Pages()
        {
            //drawingPage = new DrawingPage();
            InformationPage = new InformftionPage();
            NavigationPage = new NavigationPage();
            TablePage = new TablePage();
            TasksPage = new TasksPage();
            CurrentPage = NavigationPage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
