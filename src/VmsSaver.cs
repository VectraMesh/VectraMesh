using System.Text;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace VectraMesh.Studio
{
    public class VmsSaver
    {
        public static void Start()
        {
            MainWindow.Instance.SavePanel.Visibility = Visibility.Visible;
            MainWindow.Instance.SaveInfo.Text = string.Empty;
            MainWindow.Instance.SaveGrid.Visibility = System.Windows.Visibility.Visible;
            VmsTopMenu.State = MenuState.Saving;
        }
        public static void Finish()
        {
            MainWindow.Instance.SavePanel.Visibility = Visibility.Hidden;
        }


        public static void SavePng()
        {
            if (MainWindow.Triangler.AllPoints.Count == 0)
            {
                System.Windows.MessageBox.Show("Canvas is empty. Nothing to save.");
                return;
            }


            var width = (int)MainWindow.Instance.DrawCanvas.ActualWidth;
            var height = (int)MainWindow.Instance.DrawCanvas.ActualHeight;

            var drawingVisual = new System.Windows.Media.DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                // Draw white background
                drawingContext.DrawRectangle(
                    System.Windows.Media.Brushes.White,
                    null,
                    new System.Windows.Rect(0, 0, width, height));

                // Render the canvas content on top
                var visualBrush = new System.Windows.Media.VisualBrush(MainWindow.Instance.DrawCanvas);
                drawingContext.DrawRectangle(
                    visualBrush,
                    null,
                    new System.Windows.Rect(0, 0, width, height));
            }

            var renderTargetBitmap = new System.Windows.Media.Imaging.RenderTargetBitmap(
                width, height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            var pngEncoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            pngEncoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(renderTargetBitmap));

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save Canvas as PNG"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = System.IO.File.Create(saveFileDialog.FileName))
                {
                    pngEncoder.Save(stream);
                    MainWindow.Instance.SaveInfo.Text = "Saved";
                }
            }
        }
        public static void SaveAllSegments()
        {
            if (MainWindow.Triangler.AllPoints.Count == 0)
            {
                System.Windows.MessageBox.Show("Nothing to save.");
                return;
            }

            var data = new
            {
                Points = MainWindow.Triangler.AllPoints.Select(p => new[] { p.Position.X, p.Position.Y }).ToList(),
                Segments = MainWindow.Triangler.AllSegments.Select(s => new[] { s.S, s.E }).ToList()
            };
            string lines = System.Text.Json.JsonSerializer.Serialize(data);

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON File|*.json",
                Title = "Save all segments as JSON"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = System.IO.File.Create(saveFileDialog.FileName))
                {
                    var writer = new System.IO.StreamWriter(stream);
                    writer.Write(lines);
                    writer.Flush();
                    MainWindow.Instance.SaveInfo.Text = "Saved";
                }
            }
        }
        public static void SaveAllMeshesJson(bool copyToClipboard)
        {
            if (MainWindow.Triangler.AllEdges.Count == 0 && VmsEarCut.Mesches.Count == 0)
            {
                System.Windows.MessageBox.Show("Nothing to save.");
                return;
            }

            string lines;
            if (VmsEarCut.Mesches.Count == 0)
            {
                var data = new
                {
                    Points = MainWindow.Triangler.AllPoints.Select(p => new[] { p.Position.X, p.Position.Y }).ToList(),
                    Meshes = MainWindow.Triangler.AllEdges.Where(m => m.Triangles.Count > 0).OrderBy(e => e.HoleLevel).Select(e => new { Level = e.HoleLevel, Triangles = e.Triangles.Select(t => new[] { t.P0.Index, t.P1.Index, t.P2.Index }).ToList() }).ToList()
                };

                lines = System.Text.Json.JsonSerializer.Serialize(data).Replace("\"Meshes\":", "\r\n\"Meshes\":").Replace("{\"Level\":", "\r\n\t{\"Level\":");
            }
            else
            {
                var data = new
                {
                    Points = MainWindow.Triangler.AllPoints.Select(p => new[] { p.Position.X, p.Position.Y }).ToList(),
                    Meshes = VmsEarCut.Mesches.Select(e => new { Triangles = e.Triangles.Select(t => new[] { t.I0, t.I1, t.I2 }).ToList() }).ToList()
                };

                lines = System.Text.Json.JsonSerializer.Serialize(data).Replace("\"Meshes\":", "\r\n\"Meshes\":").Replace("{\"Triangles\":", "\r\n\t{\"Triangles\":");
            }


            if (copyToClipboard)
            {
                System.Windows.Clipboard.SetText(lines);
                MainWindow.Instance.SaveInfo.Text = "Saved";
                return;
            }
            else
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON File|*.json",
                    Title = "Save all meshes as JSON"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var stream = System.IO.File.Create(saveFileDialog.FileName))
                    {
                        var writer = new System.IO.StreamWriter(stream);
                        writer.Write(lines);
                        writer.Flush();
                        MainWindow.Instance.SaveInfo.Text = "Saved";
                    }
                }
            }
        }
        public static void SaveAllMeshesWpf(bool copyToClipboard)
        {
            if (MainWindow.Triangler.AllEdges.Count == 0 && VmsEarCut.Mesches.Count == 0)
            {
                System.Windows.MessageBox.Show("Nothing to save.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            if (VmsEarCut.Mesches.Count == 0)
            {
                foreach (var edge in MainWindow.Triangler.AllEdges.Where(el => el.Triangles.Count > 0).OrderBy(el => el.HoleLevel))
                {
                    Dictionary<int, int> pointsDict = new Dictionary<int, int>();
                    foreach (var triangle in edge.Triangles)
                    {
                        if (!pointsDict.ContainsKey(triangle.P0.Index)) pointsDict[triangle.P0.Index] = pointsDict.Count;
                        if (!pointsDict.ContainsKey(triangle.P1.Index)) pointsDict[triangle.P1.Index] = pointsDict.Count;
                        if (!pointsDict.ContainsKey(triangle.P2.Index)) pointsDict[triangle.P2.Index] = pointsDict.Count;
                    }

                    sb.AppendLine($"Level = {edge.HoleLevel}");

                    string positions = string.Join(" ", pointsDict.Keys.Select(k => $"{MainWindow.Triangler.AllPoints[k].Position.X},{MainWindow.Triangler.AllPoints[k].Position.Y},0"));
                    string indices = string.Join(" ", edge.Triangles.Select(t => $"{pointsDict[t.P0.Index]},{pointsDict[t.P1.Index]},{pointsDict[t.P2.Index]}"));
                    sb.AppendLine($"<MeshGeometry3D Positions=\"{positions}\" TriangleIndices=\"{indices}\" />");
                }
            }
            else
            {
                foreach (var mesh in VmsEarCut.Mesches)
                {
                    Dictionary<int, int> pointsDict = new Dictionary<int, int>();
                    foreach (var triangle in mesh.Triangles)
                    {
                        if (!pointsDict.ContainsKey(triangle.I0)) pointsDict[triangle.I0] = pointsDict.Count;
                        if (!pointsDict.ContainsKey(triangle.I1)) pointsDict[triangle.I1] = pointsDict.Count;
                        if (!pointsDict.ContainsKey(triangle.I2)) pointsDict[triangle.I2] = pointsDict.Count;
                    }

                    string positions = string.Join(" ", pointsDict.Keys.Select(k => $"{MainWindow.Triangler.AllPoints[k].Position.X},{MainWindow.Triangler.AllPoints[k].Position.Y},0"));
                    string indices = string.Join(" ", mesh.Triangles.Select(t => $"{pointsDict[t.I0]},{pointsDict[t.I1]},{pointsDict[t.I2]}"));
                    sb.AppendLine($"<MeshGeometry3D Positions=\"{positions}\" TriangleIndices=\"{indices}\" />");
                }
            }


            if (copyToClipboard)
            {
                System.Windows.Clipboard.SetText(sb.ToString());
                MainWindow.Instance.SaveInfo.Text = "Saved";
                return;
            }
            else
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text File|*.txt",
                    Title = "Save all meshes as WPF format"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var stream = System.IO.File.Create(saveFileDialog.FileName))
                    {
                        var writer = new System.IO.StreamWriter(stream);
                        writer.Write(sb.ToString());
                        writer.Flush();
                        MainWindow.Instance.SaveInfo.Text = "Saved";
                    }
                }
            }
        }
        public static void SaveAllPolygonWpf(bool copyToClipboard)
        {
            if (MainWindow.Triangler.AllEdges.Count == 0)
            {
                System.Windows.MessageBox.Show("Nothing to save.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var edge in MainWindow.Triangler.AllEdges.OrderBy(el => el.HoleLevel))
            {
                sb.AppendLine($"Level = {edge.HoleLevel}");

                string points = string.Join(" ", edge.Segments.Select(s => $"{s.Start.Position.X},{s.Start.Position.Y}"));
                sb.AppendLine($"\t<Polygon\r\n\t\tPoints=\"{points}\"\r\n\t/>");
            }

            if (copyToClipboard)
            {
                System.Windows.Clipboard.SetText(sb.ToString());
                MainWindow.Instance.SaveInfo.Text = "Saved";
            }
            else
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text File|*.txt",
                    Title = "Save all polygons as WPF format"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var stream = System.IO.File.Create(saveFileDialog.FileName))
                    {
                        var writer = new System.IO.StreamWriter(stream);
                        writer.Write(sb);
                        writer.Flush();
                        MainWindow.Instance.SaveInfo.Text = "Saved";
                    }
                }
            }
        }


        public static void Load()
        {
            if (MainWindow.Triangler.AllSegments.Count == 0 || MessageBox.Show(MainWindow.Instance, "It will erase all current segments. Are you sure you want to continue?", "Load Segments", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel, MessageBoxOptions.None) == MessageBoxResult.Yes)
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON File|*.json",
                    Title = "Load segments from JSON"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    VmsTopMenu.State = MenuState.None;
                    MainWindow.Triangler.ClearAll();
                    VmsEarCut.ClearTriangles();
                    VmsTopMenu.ClosePrevState();


                    string json = System.IO.File.ReadAllText(openFileDialog.FileName);
                    string result = LoadSegmentsJson(json);
                    if (!string.IsNullOrEmpty(result))
                        System.Windows.MessageBox.Show(result);
                }
            }
        }

        public static string LoadSegmentsJson(string json)
        {
            try
            {
                MainWindow.Triangler.ClearAll();

                var jsonDoc = System.Text.Json.JsonDocument.Parse(json);
                var root = jsonDoc.RootElement;

                // Parse points
                var pointsArray = root.GetProperty("Points");
                var pointsList = new List<VmPoint>();

                foreach (var pointElement in pointsArray.EnumerateArray())
                {
                    var coords = pointElement.EnumerateArray().ToList();
                    float x = coords[0].GetSingle();
                    float y = coords[1].GetSingle();
                    var point = MainWindow.Triangler.AddPoint(x, y);
                    pointsList.Add(point);
                }

                // Parse segments
                var segmentsArray = root.GetProperty("Segments");

                foreach (var segmentElement in segmentsArray.EnumerateArray())
                {
                    var indices = segmentElement.EnumerateArray().ToList();
                    int startIndex = indices[0].GetInt32();
                    int endIndex = indices[1].GetInt32();

                    var startPoint = pointsList[startIndex];
                    var endPoint = pointsList[endIndex];
                    var seg = MainWindow.Triangler.AddSegment(startPoint, endPoint);
                    seg.Show();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error loading JSON: {ex.Message}";
            }
        }

    }
}
