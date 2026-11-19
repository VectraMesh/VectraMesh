using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace VectraMesh.Studio
{
    public class VmsEngine
    {
        private static VmPoint? highlitedPoint;

        public static void Start()
        {
            MainWindow.Instance.ButtonTriangulateHolesVms.Visibility = Visibility.Collapsed;
            MainWindow.Instance.ButtonAnalyzeVms.Visibility = Visibility.Collapsed;
            MainWindow.Instance.TriangulatePanel.Visibility = Visibility.Visible;
            ClearAll();
            VmsEarCut.ClearTriangles();
            MainWindow.Triangler.RecogniseEdges();
            if(MainWindow.Triangler.AllEdges.Any(el => el.Holes.Count > 0))
                MainWindow.Instance.ButtonTriangulateHolesVms.Visibility = Visibility.Visible;

            if (MainWindow.Triangler.Message != string.Empty)
                MainWindow.Triangler.ShowErrorSegments();
            else
            {
                MainWindow.Triangler.TriangulateEdges();
                MainWindow.Instance.ButtonAnalyzeVms.Visibility = Visibility.Visible;
            }
            MainWindow.Instance.TriangulateError.Text = MainWindow.Triangler.Message;
            MainWindow.Instance.RecognitionInfo.Text = MainWindow.Triangler.RecognitionInfo;
            MainWindow.Instance.TriangulationInfo.Text = MainWindow.Triangler.TriangulationInfo;
        }
        public static void Finish()
        {
            MainWindow.Instance.TriangulationQuality.Text = "";
            MainWindow.Instance.TriangulationInfo.Text = "";
            MainWindow.Instance.RecognitionInfo.Text = "";
            MainWindow.Instance.TriangulateError.Text = "";
            MainWindow.Instance.TriangulatePanel.Visibility = Visibility.Hidden;
        }
        public static void ClearAll()
        {
            MainWindow.Instance.TriangulationInfo.Text = "";
			MainWindow.Instance.TriangulationQuality.Text = "";
			MainWindow.Instance.RecognitionInfo.Text = "";
            MainWindow.Instance.TriangulateError.Text = "";
            MainWindow.Triangler.ClearErrorSegments();
            MainWindow.Triangler.ClearEdges();
        }
        public static void MouseMove(System.Windows.Point mousePos)
        {
            var currentPoint = MainWindow.Triangler.CheckPointAtPosition((float)mousePos.X, (float)mousePos.Y).FirstOrDefault();

            if (currentPoint is not null)
            {
                if (highlitedPoint != currentPoint)
                    highlitedPoint?.Hide();
                highlitedPoint = currentPoint;
                highlitedPoint.Show();
                MainWindow.Instance.TriangulatePointInfo.Text = string.Format("Point {0} ({1:0.##}, {2:0.##})", highlitedPoint.Index, highlitedPoint.Position.X, highlitedPoint.Position.Y);
            }
            else
            {
                highlitedPoint?.Hide();
                highlitedPoint = null;
                MainWindow.Instance.TriangulatePointInfo.Text = string.Empty;
            }
        }
        public static void Help()
        {
            MessageBox.Show("VectraMesh provides recognition and triangulation of unlimited number of edges including nested holes.\r\n");
        }
        public static void TriangulateHoles()
        {
            MainWindow.Triangler.ClearTriangles();
            MainWindow.Triangler.TriangulateEdges(true);
            MainWindow.Instance.TriangulateError.Text = MainWindow.Triangler.Message;
            MainWindow.Instance.TriangulationInfo.Text = MainWindow.Triangler.TriangulationInfo;
            MainWindow.Instance.RecognitionInfo.Text = MainWindow.Triangler.RecognitionInfo;
        }
        public static void AnalyseTriangles()
        {
            foreach (var triangle in MainWindow.Triangler.AllEdges.SelectMany(el => el.Triangles)) triangle.Analyze();
            var averegeQuality = MainWindow.Triangler.AllEdges.SelectMany(el => el.Triangles).Average(triangle => triangle.AnalysisValue);
			MainWindow.Instance.TriangulationQuality.Text = string.Format("Average Quality: {0:0.##}", averegeQuality);
		}

	}
}

