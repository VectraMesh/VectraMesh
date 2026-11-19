using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;

namespace VectraMesh.Studio
{
    public class VmsEdit
    {
        private static VmSegment? highlitedSegment;
        private static VmPoint? currentPoint = null;
        private static VmPoint? draggingPoint = null;
        private static System.Windows.Point? prevMousePos = null;

        public static void Start()
        {
            MainWindow.Triangler.ClearEdges();
            VmsEarCut.ClearTriangles();
            MainWindow.Instance.ButtonEraseSegment.Background = Brushes.Transparent;
            MainWindow.Instance.ButtonMoveCorner.Background = Brushes.Transparent;
            MainWindow.Instance.EditPanel.Visibility = Visibility.Visible;
            VmsTopMenu.State = MenuState.Edit;
        }
        public static void Finish()
        {
            highlitedSegment?.UnHighlight();
            MainWindow.Instance.EditPanel.Visibility = Visibility.Hidden;
            StopMoving();
            if (currentPoint is not null)
                currentPoint.Hide();
        }


        public static void EraseSegmentClick()
        {
            VmsTopMenu.State = MenuState.EraseSegment;
            MainWindow.Instance.ButtonEraseSegment.Background = Brushes.LightBlue;
            MainWindow.Instance.ButtonMoveCorner.Background = Brushes.Transparent;
        }
        public static void MoveCornerClick()
        {
            VmsTopMenu.State = MenuState.MoveCorner;
            MainWindow.Instance.ButtonMoveCorner.Background = Brushes.LightBlue;
            MainWindow.Instance.ButtonEraseSegment.Background = Brushes.Transparent;    
        }
        public static void ClearAllClick()
        {
            MainWindow.Instance.ButtonEraseSegment.Background = Brushes.Transparent;
            MainWindow.Instance.ButtonMoveCorner.Background = Brushes.Transparent;
            var result = MessageBox.Show(MainWindow.Instance, "Are you sure you want to clear all segments?", "Clear All", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel, MessageBoxOptions.None);
            if (result == MessageBoxResult.Yes)
            {
                VmsTopMenu.State = MenuState.Edit;
                MainWindow.Triangler.ClearAll();
                VmsEarCut.ClearTriangles();
                VmsTopMenu.ClosePrevState();
            }
        }


        public static void MouseLeftButtonUp_EraseSegment(System.Windows.Point mousePos)
        {
            highlitedSegment = MainWindow.Triangler.CheckSegmentAtPosition((float)mousePos.X, (float)mousePos.Y);
            if (highlitedSegment is not null) highlitedSegment.Erase();
        }
        public static void MouseMove_EraseSegment(System.Windows.Point mousePos)
        {
            highlitedSegment?.UnHighlight();
            highlitedSegment = MainWindow.Triangler.CheckSegmentAtPosition((float)mousePos.X, (float)mousePos.Y);
            if (highlitedSegment is not null) highlitedSegment!.Highlight();
        }


        public static void StopMoving()
        {
            if (draggingPoint is not null)
            {
                if (prevMousePos.HasValue)
                    draggingPoint.SetPosition((float)prevMousePos.Value.X, (float)prevMousePos.Value.Y);
                foreach (var segment in draggingPoint.Segments)
                    segment.Show();
                draggingPoint = null;
            }
            prevMousePos = null;
        }
        public static void MouseLeftButtonUp_MoveCorner(System.Windows.Point mousePos)
        {
            if (draggingPoint is not null)
            {
                if (MainWindow.Triangler.CheckPointAtPosition((float)mousePos.X, (float)mousePos.Y).Count > 1)
                    MessageBox.Show("You can't move corner to the position of another corner.");
                else
                    draggingPoint = null;
            }
        }
        public static void MouseLeftButtonDown_MoveCorner(System.Windows.Point mousePos)
        {
            if (draggingPoint is null)
            {
                draggingPoint = MainWindow.Triangler.CheckPointAtPosition((float)mousePos.X, (float)mousePos.Y).FirstOrDefault();
                prevMousePos = mousePos;
            }
        }
        public static void MouseMove_MoveCorner(System.Windows.Point mousePos)
        {
            if (draggingPoint is not null)
            {
                draggingPoint.SetPosition((float)mousePos.X, (float)mousePos.Y);
                foreach (var segment in draggingPoint.Segments)
                    segment.Show();
            }

            if (currentPoint is not null)
                currentPoint.Hide();

            MainWindow.Instance.MoveCornerCoord.Text = string.Format("X = {0:0.##}, Y = {1:0.##}", mousePos.X, mousePos.Y);

            currentPoint = MainWindow.Triangler.CheckPointAtPosition((float)mousePos.X, (float)mousePos.Y).FirstOrDefault();
            if (currentPoint is not null)
            {
                currentPoint.Show();
                MainWindow.Instance.MoveCornerPointInfo.Text = string.Format("Point {0} ({1:0.##}, {2:0.##})", currentPoint.Index, currentPoint.Position.X, currentPoint.Position.Y);
            }
            else
                MainWindow.Instance.MoveCornerPointInfo.Text = string.Empty;
        }

    }
}
