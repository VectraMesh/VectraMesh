using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VectraMesh.Studio
{
    public class VmsDrawPolyline
    {
        private static System.Windows.Point? prevMousePos = null;
        private static VmPoint? startPoint = null;
        private static Line? drawingLine = null;
        private static VmPoint? currentPoint = null;

        public static void Start()
        {
            MainWindow.Triangler.ClearEdges();
            VmsEarCut.ClearTriangles();
            MainWindow.Instance.DrawPolylinePointInfo.Text = string.Empty;
            MainWindow.Instance.DrawPolylineCoord.Text = string.Empty;
            MainWindow.Instance.DrawPolylinePanel.Visibility = Visibility.Visible;
            VmsTopMenu.State = MenuState.DrawPolyline;
        }
        public static void Finish()
        {
            StopLine();
            if (currentPoint is not null)
                currentPoint.Hide();
            MainWindow.Instance.DrawPolylinePanel.Visibility = Visibility.Hidden;
            VmsTopMenu.State = MenuState.None; 
        }
        public static void StopLine()
        {
            prevMousePos = null;
            startPoint = null;
            if (drawingLine is not null)
                MainWindow.PlCanvas.Children.Remove(drawingLine);
            drawingLine = null;
        }
        public static void MouseLeftButtonUp(System.Windows.Point mousePos)
        {
            if(prevMousePos is null)
            {
                startPoint = MainWindow.Triangler.CheckPointAtPosition((float)mousePos.X, (float)mousePos.Y).FirstOrDefault();
                if(startPoint is null) 
                    prevMousePos = mousePos;
                else 
                    prevMousePos = new System.Windows.Point(startPoint.Position.X, startPoint.Position.Y);

                drawingLine = new Line()
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 1,
                        X1 = prevMousePos.Value.X,
                        Y1 = prevMousePos.Value.Y,
                        X2 = prevMousePos.Value.X,
                        Y2 = prevMousePos.Value.Y
                    };
                MainWindow.PlCanvas.Children.Add(drawingLine);
            }
            else
            {
                if (prevMousePos != mousePos)
                {
                    if (startPoint is null)
                        startPoint = MainWindow.Triangler.AddPoint((float)prevMousePos.Value.X, (float)prevMousePos.Value.Y);

                    VmPoint? endPoint = MainWindow.Triangler.CheckPointAtPosition((float)mousePos.X, (float)mousePos.Y).FirstOrDefault();
                    if (endPoint is null)
                    {
                        endPoint = MainWindow.Triangler.AddPoint((float)mousePos.X, (float)mousePos.Y);
                        prevMousePos = mousePos;
                        var seg = MainWindow.Triangler.AddSegment(startPoint, endPoint);
                        startPoint = endPoint;
                        seg.Show();
                        drawingLine.X1 = prevMousePos.Value.X;
                        drawingLine.Y1 = prevMousePos.Value.Y;
                        drawingLine.X2 = prevMousePos.Value.X;
                        drawingLine.Y2 = prevMousePos.Value.Y;
                    }
                    else if(startPoint != endPoint)
                    {
                        prevMousePos = null;
                        var seg = MainWindow.Triangler.AddSegment(startPoint, endPoint);
                        startPoint = null;
                        seg.Show();
                        StopLine();
                    }
                    //else
                    //    prevMousePos = new System.Windows.Point(endPoint.Position.X, endPoint.Position.Y);
                }
            }
        }
        public static void MouseMove(System.Windows.Point mousePos)
        {
            if (drawingLine is not null)
            {
                drawingLine.X2 = mousePos.X;
                drawingLine.Y2 = mousePos.Y;
            }
            if (currentPoint is not null)
                currentPoint.Hide();
            currentPoint = MainWindow.Triangler.CheckPointAtPosition((float)mousePos.X, (float)mousePos.Y).FirstOrDefault();

            MainWindow.Instance.DrawPolylineCoord.Text = string.Format("X = {0:0.##}, Y = {1:0.##}", mousePos.X, mousePos.Y);

            if (currentPoint is not null)
            {
                currentPoint.Show();
                MainWindow.Instance.DrawPolylinePointInfo.Text = string.Format("Point {0} ({1:0.##}, {2:0.##})", currentPoint.Index, currentPoint.Position.X, currentPoint.Position.Y);
            }
            else
                MainWindow.Instance.DrawPolylinePointInfo.Text = string.Empty;
        }
    }
}
