using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VectraMesh.Studio
{
    public class VmsPresenter : IVmPresenter
    {
        private Canvas canvas;

        public double PointRadius = 3;
        public SolidColorBrush PointColor = Brushes.Red;

        public double LineThickness = 1;
        public SolidColorBrush LineColor = Brushes.Gray;
        public double HighlightedLineThickness = 3;
        public SolidColorBrush HighlightedLineColor = Brushes.Orange;
        public double ErrorLineThickness = 2;
        public SolidColorBrush ErrorLineColor = Brushes.Red;

        public SolidColorBrush TriangleFill = new SolidColorBrush(Color.FromRgb(0, 48, 255));
        public SolidColorBrush TriangleStroke = new SolidColorBrush(Color.FromArgb(64, 64, 128, 128));
        public double TriangleStrokeThickness = 1;
        public double TriangleFillOpacityHover = 0.08;
        public double TriangleFillOpacity = 0.1;
        public double TriangleFillOpacityHoverAnalyzed = 0.28;
        public double TriangleFillOpacityAnalyzed = 0.3;


        public VmsPresenter(Canvas canvas)
        {
            this.canvas = canvas;
        }


        public void PointHide(VmPoint point)
        {
            canvas.Children.Remove(point.PresentationObject as Ellipse);
        }

        public object PointShow(VmPoint point)
        {
            Ellipse? ellipse = point.PresentationObject as Ellipse;
            if (ellipse is null)
            {
                ellipse = new Ellipse
                {
                    Width = PointRadius * 2,
                    Height = PointRadius * 2,
                    Fill = PointColor
                };
            }
            if(!canvas.Children.Contains(ellipse))
                canvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, point.Position.X - PointRadius);
            Canvas.SetTop(ellipse, point.Position.Y - PointRadius);

            return ellipse;
        }



        public void SegmentHide(VmSegment segment)
        {
            canvas.Children.Remove(segment.PresentationObject as Line);
        }

        public void SegmentHighlight(VmSegment segment)
        {
            Line? line = segment.PresentationObject as Line;
            if (line is not null)
            {
                line.StrokeThickness = HighlightedLineThickness;
                line.Stroke = HighlightedLineColor;
            }
        }

        public object SegmentShow(VmSegment segment)
        {
            Line? line = segment.PresentationObject as Line;
            if (line is null)
            {
                line = new Line
                {
                    StrokeThickness = LineThickness,
                    Stroke = LineColor
                };
            }
            line.X1 = segment.Start.Position.X;
            line.Y1 = segment.Start.Position.Y;
            line.X2 = segment.End.Position.X;
            line.Y2 = segment.End.Position.Y;

            if (!canvas.Children.Contains(line))
                canvas.Children.Add(line);

            return line;
        }

        public void SegmentUnhighlight(VmSegment segment)
        {
            Line? line = segment.PresentationObject as Line;
            if (line is not null)
            {
                if (segment.IsError)
                {
                    line.StrokeThickness = ErrorLineThickness;
                    line.Stroke = ErrorLineColor;
                }
                else
                {
                    line.Stroke = LineColor;
                    line.StrokeThickness = LineThickness;
                }
            }
        }



        public void TriangleHide(VmTriangle triangle)
        {
            canvas.Children.Remove(triangle.PresentationObject as Polygon);
        }
        public object TriangleShow(VmTriangle triangle)
        {
            Polygon? polygon = triangle.PresentationObject as Polygon;
            if (polygon is null)
            {
                polygon = new Polygon
                {
                    Fill = new SolidColorBrush(TriangleFill.Color),
                    Stroke = TriangleStroke,
                    StrokeThickness = TriangleStrokeThickness
                };
                polygon.Fill.Opacity = TriangleFillOpacity;
                polygon.MouseEnter += (s, e) =>
                {
                        if (triangle.IsAnalyzed)
                            MainWindow.Instance.TriangulateTriangleInfo.Text = string.Format("Triangle {0}  {1}  {2}  quality={3:F2}", triangle.P0.Index, triangle.P1.Index, triangle.P2.Index, triangle.AnalysisValue);
                        else
                        MainWindow.Instance.TriangulateTriangleInfo.Text = string.Format("Triangle {0}  {1}  {2}", triangle.P0.Index, triangle.P1.Index, triangle.P2.Index);
                        polygon.Fill.Opacity = triangle.IsAnalyzed ? TriangleFillOpacityHoverAnalyzed : TriangleFillOpacityHover;
                };
                polygon.MouseLeave += (s, e) =>
                {
                    MainWindow.Instance.TriangulateTriangleInfo.Text = string.Empty;
                    polygon.Fill.Opacity = triangle.IsAnalyzed ? TriangleFillOpacityAnalyzed : TriangleFillOpacity;
                };

            }
            polygon.Points.Add(new System.Windows.Point(triangle.P0.Position.X, triangle.P0.Position.Y));
            polygon.Points.Add(new System.Windows.Point(triangle.P1.Position.X, triangle.P1.Position.Y));
            polygon.Points.Add(new System.Windows.Point(triangle.P2.Position.X, triangle.P2.Position.Y));

            if (!canvas.Children.Contains(polygon))
                canvas.Children.Add(polygon);

            return polygon;
        }
        public void ShowTriangleAnalyzed(VmTriangle triangle)
        {
            Polygon? polygon = triangle.PresentationObject as Polygon;
            if (polygon is not null)
            {
                byte kb = (byte)(255d * triangle.AnalysisValue);
                byte kr = (byte)(255 - kb);
                polygon.Fill = new SolidColorBrush(Color.FromRgb(kr, 48, kb));
                polygon.Fill.Opacity = TriangleFillOpacityAnalyzed;
            }
        }

    }
}
