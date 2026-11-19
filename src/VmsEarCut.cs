using EarcutDotNet;
using System.Windows;

namespace VectraMesh.Studio
{
    public class VmsEarCut
    {
        public static List<EarCutMesh> Mesches = new List<EarCutMesh>();

        private static VmPoint? highlitedPoint;

        public static void Start()
        {
            MainWindow.Instance.ButtonAnalyzeEarCutTriangles.Visibility = Visibility.Collapsed;
            MainWindow.Instance.EarCutPanel.Visibility = Visibility.Visible;
            ClearAll();
            MainWindow.Triangler.RecogniseEdges(true);

            if (MainWindow.Triangler.Message != string.Empty)
            {
                MainWindow.Triangler.ShowErrorSegments();
                MainWindow.Instance.EarCutDiagnosticInfo.Text = MainWindow.Triangler.RecognitionInfo;
            }
            else
            {
                Triangulate();
                MainWindow.Instance.ButtonAnalyzeEarCutTriangles.Visibility = Visibility.Visible;
            }
            MainWindow.Instance.EarCutError.Text = MainWindow.Triangler.Message;
        }
        public static void Finish()
        {
            MainWindow.Instance.RecognitionInfo.Text = "";
            MainWindow.Instance.TriangulationInfo.Text = "";
            MainWindow.Instance.TriangulationQuality.Text = "";
            MainWindow.Instance.EarCutError.Text = "";
            MainWindow.Instance.EarCutDiagnosticInfo.Text = "";
            MainWindow.Instance.EarCutQuality.Text = "";
            MainWindow.Instance.EarCutPanel.Visibility = Visibility.Hidden;
        }
        public static void ClearAll()
        {
            MainWindow.Instance.EarCutDiagnosticInfo.Text = "";
            MainWindow.Instance.EarCutQuality.Text = "";
            MainWindow.Instance.EarCutError.Text = "";
            MainWindow.Triangler.ClearErrorSegments();
            MainWindow.Triangler.ClearEdges();
            ClearTriangles();
        }
        public static void ClearTriangles()
        {
            foreach (var mesh in Mesches) 
                foreach (var triangle in mesh.Triangles)
                    triangle.Hide();
            Mesches.Clear();
        }

        private static void Triangulate()
        {
            int trianglesNumber = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var edge in MainWindow.Triangler.AllEdges.Where(el => el.HoleLevel == 0))
            {
                var mesch = new EarCutMesh();
                Mesches.Add(mesch);

                var vertices = new List<double>();
                var holes = new List<int>();
                foreach (var segment in edge.Segments)
                {
                    vertices.Add(segment.Start.Position.X);
                    vertices.Add(segment.Start.Position.Y);
                    mesch.Points.Add(segment.Start);
                }
                //foreach (var hole in edge.Holes)
                //{
                //    var n = vertices.Count / 2;
                //    if (holes.Count > 0)
                //        holes.Add(n - 1);
                //    holes.Add(vertices.Count / 2);
                //    foreach (var segment in edge.Holes[0].Segments)
                //    {
                //        vertices.Add(segment.Start.Position.X);
                //        vertices.Add(segment.Start.Position.Y);
                //        mesch.Points.Add(segment.Start);
                //    }
                //    //holes.Add(vertices.Count / 2 - 1);
                //}
                foreach (var hole in edge.Holes)
                {
                    holes.Add(vertices.Count / 2);
                    foreach (var segment in hole.Segments)
                    {
                        vertices.Add(segment.Start.Position.X);
                        vertices.Add(segment.Start.Position.Y);
                        mesch.Points.Add(segment.Start);
                    }
                }

                var triangles = Earcut.Triangulate(vertices.ToArray(), holes.ToArray(), 2);

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    EarCutTriangle triangle = new EarCutTriangle(mesch.Points[triangles[i]].Index, mesch.Points[triangles[i + 1]].Index, mesch.Points[triangles[i + 2]].Index, vertices[triangles[i] * 2], vertices[triangles[i] * 2 + 1], vertices[triangles[i + 1] * 2], vertices[triangles[i + 1] * 2 + 1], vertices[triangles[i + 2] * 2], vertices[triangles[i + 2] * 2 + 1]);
                    triangle.Show();
                    mesch.Triangles.Add(triangle);
                }
                trianglesNumber += triangles.Length / 3;
            }
            stopwatch.Stop();

            MainWindow.Instance.EarCutDiagnosticInfo.Text = (MainWindow.Triangler.AllEdges.Count > 0) ? MainWindow.Triangler.RecognitionInfo + $"     Triangulation: {stopwatch.ElapsedMilliseconds} ms     Triangles number: {trianglesNumber}" : string.Empty;
        }
        public static void Help()
        {
            MessageBox.Show(
                "EarCut lib doesn't provide recognition of edges or holes.\r\n" +
                "It only performs triangulation based on the provided vertices and holes.\r\n" +
                "Therefore the edges and holes here are provided by VectraMesh\r\n" +
                "Although EarCut lib doesn't limit the number of holes it efficiently supports only one.\r\n"
                );
        }
        public static void AnalyseTriangles()
        {
            foreach (var mesh in Mesches)
                foreach (var triangle in mesh.Triangles)
                    triangle.Analyze();
            var averegeQuality = Mesches.SelectMany(mesh => mesh.Triangles).Average(triangle => triangle.AnalysisValue);
            MainWindow.Instance.EarCutQuality.Text = string.Format("Average Quality: {0:0.##}", averegeQuality);
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
                MainWindow.Instance.EarCutPointInfo.Text = string.Format("Point {0} ({1:0.##}, {2:0.##})", highlitedPoint.Index, highlitedPoint.Position.X, highlitedPoint.Position.Y);
            }
            else
            {
                highlitedPoint?.Hide();
                highlitedPoint = null;
                MainWindow.Instance.EarCutPointInfo.Text = string.Empty;
            }
        }

    }
}
