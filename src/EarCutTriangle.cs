using System.Windows.Media;
using System.Windows.Shapes;

namespace VectraMesh.Studio
{
    public class EarCutTriangle
    {
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }

        public double X0 { get; private set; }
        public double Y0 { get; private set; }
        public double X1 { get; private set; }
        public double Y1 { get; private set; }
        public double X2 { get; private set; }
        public double Y2 { get; private set; }

        private bool isAnalyzed = false;
        public double AnalysisValue = 0;


        private SolidColorBrush triangleFill = new SolidColorBrush(MainWindow.TrianglerPresenter.TriangleFill.Color);
        private Polygon? polygon;


        internal EarCutTriangle(int i0, int i1, int i2, double x0, double y0, double x1, double y1, double x2, double y2    )
        {
            I0 = i0;
            I1 = i1;
            I2 = i2;
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public void Show()
        {
            isAnalyzed = false;
            polygon = new Polygon();
            polygon.Points.Add(new System.Windows.Point(X0, Y0));
            polygon.Points.Add(new System.Windows.Point(X1, Y1));
            polygon.Points.Add(new System.Windows.Point(X2, Y2));

            polygon.Fill = triangleFill;
            polygon.Stroke = MainWindow.TrianglerPresenter.TriangleStroke;
            polygon.StrokeThickness = MainWindow.TrianglerPresenter.TriangleStrokeThickness;
            polygon.Fill.Opacity = MainWindow.TrianglerPresenter.TriangleFillOpacity;

            MainWindow.Instance.DrawCanvas.Children.Add(polygon);

            polygon.MouseEnter += (s, e) =>
            {
                MainWindow.Instance.EarCutTriangleInfo.Text = string.Format("Triangle {0}  {1}  {2}", I0, I1, I2);
                if (isAnalyzed)
                    MainWindow.Instance.EarCutTriangleInfo.Text = string.Format("Triangle {0}  {1}  {2}  quality={3:F2}", I0, I1, I2, AnalysisValue);
                else
                    MainWindow.Instance.EarCutTriangleInfo.Text = string.Format("Triangle {0}  {1}  {2}", I0, I1, I2);
                polygon.Fill.Opacity = isAnalyzed ? MainWindow.TrianglerPresenter.TriangleFillOpacityHoverAnalyzed : MainWindow.TrianglerPresenter.TriangleFillOpacityHover;
            };
            polygon.MouseLeave += (s, e) =>
            {
                MainWindow.Instance.EarCutTriangleInfo.Text = string.Empty;
                polygon.Fill.Opacity = isAnalyzed ? MainWindow.TrianglerPresenter.TriangleFillOpacityAnalyzed : MainWindow.TrianglerPresenter.TriangleFillOpacity;
            };
        }
        public void Hide()
        {
            MainWindow.Instance.DrawCanvas.Children.Remove(polygon);
        }
        public void Analyze()
        {
            var l0 = (X1 - X0) * (X1 - X0) + (Y1 - Y0) * (Y1 - Y0);
            var l1 = (X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1);
            var l2 = (X0 - X2) * (X0 - X2) + (Y0 - Y2) * (Y0 - Y2);

            var ll0 = Math.Sqrt(l0);
            var ll1 = Math.Sqrt(l1);
            var ll2 = Math.Sqrt(l2);

            var s = (ll0 + ll1 + ll2) / 2;
            var a = Math.Sqrt(s * Math.Abs(s - ll0) * Math.Abs(s - ll1) * Math.Abs(s - ll2));

            var sq = l0 + l1 + l2;

            AnalysisValue = 6.92 * a / sq;


            byte kb = (byte)(255d * AnalysisValue);
            byte kr = (byte)(255 - kb);


            polygon.Fill = new SolidColorBrush(Color.FromRgb(kr, 48, kb));
            polygon.Fill.Opacity = MainWindow.TrianglerPresenter.TriangleFillOpacityAnalyzed;
            isAnalyzed = true;
        }
    }
    public class EarCutMesh
    {
        public List<EarCutTriangle> Triangles = new List<EarCutTriangle>();
        public List<VmPoint> Points = new List<VmPoint>();
    }

}
