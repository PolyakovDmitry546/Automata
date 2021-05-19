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
using System.Windows.Shapes;

namespace Automata
{
    /// <summary>
    /// Логика взаимодействия для DrawingPage.xaml
    /// </summary>
    public partial class DrawingPage : Page
    {
        public DrawingPage()
        {
            InitializeComponent();
            Drawing();
        }

        private bool isDragging = false;
        private DrawingVisual selectedVisual;
        private Vector clickOffset;

        public void Drawing()
        {
            var visual = new VisualSquare();
            visual.Brush = drawingBrush;
            visual.DrawingPen = drawingPen;
            visual.SelectedDrawingBrush = selectedDrawingBrush;
            visual.Draw(new Point(100, 100), false);
            drawingSurface.AddVisual(visual);
            var visual2 = new VisualEllipse(100,100,30,30);
            visual2.Brush = drawingBrush;
            visual2.DrawingPen = drawingPen;
            visual2.SelectedDrawingBrush = selectedDrawingBrush;
            visual2.Draw(new Point(100, 100), false);
            drawingSurface.AddVisual(visual2);
        }

        private void DrawingSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pointClicked = e.GetPosition(drawingSurface);
            DrawingVisual visual = drawingSurface.GetVisual(pointClicked);
            if (visual != null)
            {
                Point topLeftCorner = new Point(visual.ContentBounds.TopLeft.X + drawingPen.Thickness / 2,
                    visual.ContentBounds.TopLeft.Y + drawingPen.Thickness / 2);
                ((VisualFigure)visual).Draw(topLeftCorner, true);
                clickOffset = topLeftCorner - pointClicked;
                isDragging = true;
                if (selectedVisual != null && selectedVisual != visual)
                {
                    ClearSelection();
                }
                selectedVisual = visual;
            }
        }

        private Brush drawingBrush = Brushes.Aqua;
        private Brush selectedDrawingBrush = Brushes.Yellow;
        private Pen drawingPen = new Pen(Brushes.SteelBlue, 3);
        private Size squareSize = new Size(30, 30);

        private void ClearSelection()
        {
            Point topLeftCorner = new Point(selectedVisual.ContentBounds.TopLeft.X + drawingPen.Thickness / 2,
                    selectedVisual.ContentBounds.TopLeft.Y + drawingPen.Thickness / 2);
            ((VisualFigure)selectedVisual).Draw(topLeftCorner, false);
            selectedVisual = null;
        }

        private void DrawingSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point pointDragged = e.GetPosition(drawingSurface) + clickOffset;
                ((VisualFigure)selectedVisual).Draw(pointDragged, true);
            }
        }
    }

    public class DrawingCanvas : Canvas
    {
        private List<Visual> visuals = new List<Visual>();

        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }
        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);
            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }
        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, point);
            return hitTestResult.VisualHit as DrawingVisual;
        }
    }

    public class VisualFigure : DrawingVisual
    {
        public VisualFigure() { }
        public Brush Brush { get; set; }
        public Brush SelectedDrawingBrush { get; set; }
        public Pen DrawingPen { get; set; }

        public virtual void Draw(Point topLeftCorner, bool isSelected) { }
        public virtual void Draw() { }
    }

    public class VisualEllipse : VisualFigure
    {
        double radiusX, radiusY;
        double centerX, centerY;
        List<Marker> markers = new List<Marker>();
        public VisualEllipse(double centerX, double centerY, double radiusX, double radiusY)
        {
            this.centerX = centerX;
            this.centerY = centerY;
            this.radiusX = radiusX;
            this.radiusY = radiusY;
            for (int i = 0; i < 12; i++)
            {
                markers.Add(new Marker(centerX + Math.Cos(i * Math.PI / 6.0) * radiusX, centerY + Math.Sin(i * Math.PI / 6.0) * radiusY));
            }
        }

        public double RadiusX
        {
            set { radiusX = value; }
            get { return radiusX; }
        }
        public double RadiusY
        {
            set { radiusY = value; }
            get { return radiusY; }
        }

        public override void Draw(Point topLeftCorner, bool isSelected)
        {
            using (DrawingContext dc = RenderOpen())
            { 
                if (isSelected)
                    Brush = SelectedDrawingBrush;
                centerX = topLeftCorner.X + radiusX;
                centerY = topLeftCorner.Y + radiusY;
                dc.DrawEllipse(Brush, DrawingPen, new Point(centerX, centerY), radiusX, radiusY);
                SetMarkers(dc);
            }
            
        }

        private void SetMarkers(DrawingContext dc)
        {
            for (int i = 0; i < markers.Count; i++) 
            {
                markers[i].X = centerX + Math.Cos(i * Math.PI / 6.0) * radiusX;
                markers[i].Y = centerY + Math.Sin(i * Math.PI / 6.0) * radiusY;
                markers[i].Draw(dc);
            }
        }
    }

    public class VisualSquare : VisualFigure
    {
        private Size squareSize = new Size(30, 30);
        public VisualSquare() { }
        public override void Draw(Point topLeftCorner, bool isSelected)
        {
            using (DrawingContext dc = RenderOpen())
            {
                if (isSelected)
                    Brush = SelectedDrawingBrush;
                dc.DrawRectangle(Brush, DrawingPen, new Rect(topLeftCorner, squareSize));
            }
        }
    }

    public class Marker : DrawingVisual
    {
        double x, y;

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public Marker(double X, double Y)
        {
            x = X;
            y = Y;
            
        }

        public void Draw(DrawingContext dc)
        {
            
            dc.DrawEllipse(Brushes.Black, new Pen(Brushes.Black, 2), new Point(x, y), 5, 5);
            
        }
    }
}
