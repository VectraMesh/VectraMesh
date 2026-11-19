using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace VectraMesh.Studio
{

    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public static Canvas PlCanvas;
        public static VmEngine Triangler;
        public static VmsPresenter TrianglerPresenter;

        public MainWindow()
        {
            InitializeComponent();
            DrawCanvas.MouseMove += DrawCanvas_MouseMove;
            DrawCanvas.PreviewMouseLeftButtonUp += DrawCanvas_MouseLeftButtonUp;
            DrawCanvas.PreviewMouseLeftButtonDown += DrawCanvas_MouseLeftButtonDown;
            DrawCanvas.MouseLeave += DrawCanvas_MouseLeave;
            KeyUp += MainWindow_KeyUp;
            VmsTopMenu.PrepareMenu(TopMenu);    
            PlCanvas = DrawCanvas;
            Instance = this;

            Triangler = new VmEngine();
            TrianglerPresenter = new VmsPresenter(DrawCanvas);
            Triangler.Presenter = TrianglerPresenter;
            Triangler.EventSegmentsExist += VmsTopMenu.OnSegmentsExistChanged;
            VmsTopMenu.OnSegmentsExistChanged(false);
        }


        private void DrawCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(DrawCanvas);
            switch (VmsTopMenu.State)
            {
                case MenuState.DrawPolyline:
                    VmsDrawPolyline.MouseLeftButtonUp(mousePos);
                    break;
                case MenuState.EraseSegment:
                    VmsEdit.MouseLeftButtonUp_EraseSegment(mousePos);
                    break;
                case MenuState.MoveCorner:
                    VmsEdit.MouseLeftButtonUp_MoveCorner(mousePos);
                    break;
            }
        }
        private void DrawCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(DrawCanvas);
            switch (VmsTopMenu.State)
            {
                case MenuState.MoveCorner:
                    VmsEdit.MouseLeftButtonDown_MoveCorner(mousePos);
                    break;
            }
        }
        private void DrawCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(DrawCanvas);
            switch (VmsTopMenu.State)
            {
                case MenuState.DrawPolyline:
                    VmsDrawPolyline.MouseMove(mousePos);
                    break;
                case MenuState.EraseSegment:
                    VmsEdit.MouseMove_EraseSegment(mousePos);
                    break;
                case MenuState.MoveCorner:
                    VmsEdit.MouseMove_MoveCorner(mousePos);
                    break;
                case MenuState.Triangulate:
                    VmsEngine.MouseMove(mousePos);
                    break;
                case MenuState.EarCut:
                    VmsEarCut.MouseMove(mousePos);
                    break;
            }
        }
        private void DrawCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            switch (VmsTopMenu.State)
            {
                case MenuState.DrawPolyline:
                    VmsDrawPolyline.StopLine();
                    break;
                case MenuState.MoveCorner:
                    VmsEdit.StopMoving();
                    break;
            }
        }
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                switch (VmsTopMenu.State)
                {
                    case MenuState.DrawPolyline:
                        VmsDrawPolyline.StopLine();
                        break;
                    case MenuState.MoveCorner:
                        VmsEdit.StopMoving();
                        break;
                }
            }
        }


        private void FinishPolyline_Click(object sender, RoutedEventArgs e)
        {
            VmsDrawPolyline.StopLine();
        }
        private void Advertising_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
             "Bezier & Curved Segments – Coming in VectraMesh.Pro\r\n\r\n" +
            "VectraMesh.Pro will extend the geometry engine with native support for quadratic and cubic Bezier curves, circular arcs, and adaptive tessellation.\r\n\r\n"+
            "• Automatic curve-to-mesh discretization with configurable tolerance\r\n" +
            "• Full topology and nested-hole recognition across mixed straight and curved boundaries\r\n" +
            "• Seamless upgrade path (using VectraMesh; stays unchanged)\r\n\r\n" +
            "Follow updates and release announcements on GitHub: https://github.com") ;
        }
        private void EraseSegmentClick(object sender, RoutedEventArgs e)
        {
            VmsEdit.EraseSegmentClick();
        }
        private void MoveCornerClick(object sender, RoutedEventArgs e)
        {
            VmsEdit.MoveCornerClick();
        }
        private void ClearAllClick(object sender, RoutedEventArgs e)
        {
            VmsEdit.ClearAllClick();
        }



        private void OpenBackgroundImage(object sender, RoutedEventArgs e)
        {
            VmsBackgroundImage.OpenBackgroundImage();
        }
        private void EraseBackgroundImage(object sender, RoutedEventArgs e)
        {
            VmsBackgroundImage.EraseBackgroundImage();
        }
        private void TriangulateHolesVms(object sender, RoutedEventArgs e)
        {
            VmsEngine.TriangulateHoles();
        }
        private void AnalyzeVms(object sender, RoutedEventArgs e)
        {
            VmsEngine.AnalyseTriangles();
        }
        private void AnalyzeEarCutTriangles(object sender, RoutedEventArgs e)
        {
            VmsEarCut.AnalyseTriangles();
        }



        private void ButtonSaveCancel_Click(object sender, RoutedEventArgs e)
        {
            SaveGrid.Visibility = Visibility.Collapsed;
            VmsTopMenu.MiSave.Background = Brushes.Transparent;
        }
        private void ButtonSaveSave_Click(object sender, RoutedEventArgs e)
        {
            if (RbPicturePngFormat.IsChecked == true)
                VmsSaver.SavePng();
            else if (RbAllSegments.IsChecked == true)
                VmsSaver.SaveAllSegments();
            else if (RbMeshJsonFormat.IsChecked == true)
                VmsSaver.SaveAllMeshesJson(RbClipboard.IsChecked == true);
            else if (RbMeshWpfFormat.IsChecked == true)
                VmsSaver.SaveAllMeshesWpf(RbClipboard.IsChecked == true);
            else if (RbPolygonsWpf.IsChecked == true)
                VmsSaver.SaveAllPolygonWpf(RbClipboard.IsChecked == true);
            SaveGrid.Visibility = Visibility.Collapsed;
            VmsTopMenu.MiSave.Background = Brushes.Transparent;
        }
        private void SaveMethodChanged(object sender, RoutedEventArgs e)
        {
            if (SaverMiddle is not null)
            {
                if (RbAllSegments.IsChecked == true || RbPicturePngFormat.IsChecked == true)
                    SaverMiddle.Visibility = Visibility.Hidden;
                else
                    SaverMiddle.Visibility = Visibility.Visible;
            }
        }



        private void HelpDrawPolyline(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Click points along desired polyline. Start or stop on new or existing point.\r\n\r\n" +
                "Click Esc or \"Finish polyline\" to stop prolongation.\r\n\r\n" +
                "Prolongation stops by connecting polyline with existing point.\r\n\r\n" +
                "You can stop prolongation by moving the mouse outside of canvas.\r\n\r\n" +
                "You can start a new segment or connect segments by clicking on highlighted corners.\r\n");
        }
		private void HelpEdit(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(
				"By erasing segment:\r\n\t" +
				"Click on highlighted segment to erase it.\r\n\r\n" +
				"By moving corner:\r\n\t" +
				"Drag a corner to new position.\r\n\t" +
				"Click Esc or move out of canvas to return to previous position.\r\n\r\n" +
				"Clearing all removes all segments.\r\n");
		}
        private void HelpAnalyze(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Triangle quality is an Area-to-Edge Ratio (Mean Ratio).\r\n" +
                "It compares the triangle's actual area to the sum of the squares of its side lengths.\r\n\r\n" +
                "By a triangle with all equal sides it is equal 1.\r\n" +
				"By a triangle degenerated to line it is equal 0.\r\n\r\n" +
            "The poorer the quality the more red it is shown after analysis.\r\n" +
			"The better the quality the more blue it is shown after analysis.\r\n");
        }

	}
}

