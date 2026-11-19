using System.Windows.Controls;
using System.Windows.Media;

namespace VectraMesh.Studio
{
    public class VmsTopMenu
    {
        public static Menu TopMenu;
        public static MenuItem MiDrawPolyine;
        public static MenuItem MiEdit;
        public static MenuItem MiBackgroundImage;
        public static MenuItem MiTriangulate;
        public static MenuItem MiEarCut;
        public static MenuItem MiSave;
        private static MenuItem MiLoad;
        public static MenuState State { get; set; } = MenuState.None;
        private static SolidColorBrush highlithedMenuBrush = new SolidColorBrush(Color.FromRgb(192, 221, 235));

        public static void PrepareMenu(Menu menu)
        {
            TopMenu = menu;

            MiDrawPolyine = new MenuItem() { Header = "Draw polyline" };
            MiDrawPolyine.Click += DrawPollyine_Click;
            TopMenu.Items.Add(MiDrawPolyine);

            MiEdit = new MenuItem() { Header = "Edit" };
            MiEdit.Click += Edit_Click;
            TopMenu.Items.Add(MiEdit);

            MiBackgroundImage = new MenuItem() { Header = "Load/Unload sample image" };
            MiBackgroundImage.Click += MiBackgroundImage_Click;
            TopMenu.Items.Add(MiBackgroundImage);

            MiTriangulate = new MenuItem() { Header = "Triangulate VectraMesh" };
            MiTriangulate.Click += MiTriangulate_Click;
            TopMenu.Items.Add(MiTriangulate);

            MiEarCut = new MenuItem() { Header = "Triangulate EarCut" };
            MiEarCut.Click += MiEarCut_Click;
            TopMenu.Items.Add(MiEarCut);

            MiSave = new MenuItem() { Header = "Save" };
            MiSave.Click += MiSave_Click;
            TopMenu.Items.Add(MiSave);

            MiLoad = new MenuItem() { Header = "Load" };
            MiLoad.Click += MiLoad_Click;
            TopMenu.Items.Add(MiLoad);

        }

        public static void OnSegmentsExistChanged(bool segmentsExist)
        {
            MiEdit.IsEnabled = segmentsExist;
            MiTriangulate.IsEnabled = segmentsExist;
            MiEarCut.IsEnabled = segmentsExist;
            MiSave.IsEnabled = segmentsExist;
        }

        private static void DrawPollyine_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClosePrevState();
            VmsDrawPolyline.Start();
            MiDrawPolyine.Background = highlithedMenuBrush;
        }
        private static void Edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClosePrevState();
            VmsEdit.Start();
            MiEdit.Background = highlithedMenuBrush;
        }
        private static void MiBackgroundImage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClosePrevState();
            State = MenuState.BackgroundImage;
            VmsBackgroundImage.Start();
            MiBackgroundImage.Background = highlithedMenuBrush;
        }
        private static void MiTriangulate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClosePrevState();
            State = MenuState.Triangulate;
            VmsEngine.Start();
            MiTriangulate.Background = highlithedMenuBrush;
        }
        private static void MiEarCut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClosePrevState();
            State = MenuState.EarCut;
            VmsEarCut.Start();
            MiEarCut.Background = highlithedMenuBrush;
        }
        private static void MiSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClosePrevState();
            MiSave.Background = highlithedMenuBrush;
            VmsSaver.Start();
        }
        private static void MiLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClosePrevState();
            State = MenuState.Loading;
            VmsSaver.Load();
        }

        public static void ClosePrevState()
        {
            switch (State)
            {
                case MenuState.DrawPolyline:
                    VmsDrawPolyline.Finish();
                    MiDrawPolyine.Background = Brushes.Transparent;
                    break;
                case MenuState.Edit:
                case MenuState.EraseSegment:
                case MenuState.MoveCorner:
                    VmsEdit.Finish();
                    MiEdit.Background = Brushes.Transparent;
                    break;
                case MenuState.BackgroundImage:
                    VmsBackgroundImage.Finish();
                    MiBackgroundImage.Background = Brushes.Transparent;
                    break;
                case MenuState.Triangulate:
                    VmsEngine.Finish();
                    MiTriangulate.Background = Brushes.Transparent;
                    break;
                case MenuState.EarCut:
                    VmsEarCut.Finish();
                    MiEarCut.Background = Brushes.Transparent;
                    break;
                case MenuState.Saving:
                    VmsSaver.Finish();
                    MiSave.Background = Brushes.Transparent;
                    break;
            }
        }
    }

    public enum MenuState
    {
        None,
        Edit,
        DrawPolyline,
        EraseSegment,
        BackgroundImage,
        MoveCorner,
        Triangulate,
        EarCut,
        Saving,
        Loading
    }
}

