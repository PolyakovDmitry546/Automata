using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для InformftionPage.xaml
    /// </summary>
    public partial class InformftionPage : Page
    {
        public InformftionPage()
        {
            InitializeComponent();
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "TheoryHTMLPage.html");
            browser.Navigate(uri);
            //LoadDocument();
        }

        private void LoadDocument()
        {
            //using (FileStream fs = File.Open("курсовая.xps", FileMode.Open))
            //{
            //    FlowDocument document = (FlowDocument)XamlReader.Load(fs);

            //    if (document == null)
            //        MessageBox.Show("Ошибка при загрузке документа");
            //    else
            //        documentViewer.Document = document;
            //}
            //XpsDocument doc = new XpsDocument("курсовая.xps", FileAccess.Read);
            //documentViewer.Document = doc.GetFixedDocumentSequence();
            //doc.Close();
        }
    }
}
