using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VectraMesh.Studio
{
    public class VmsBackgroundImage
    {
        private static bool isDragAndDropEnabled = false;
        public static void Start()
        {
            MainWindow.Instance.BackgroundImagePanel.Visibility = Visibility.Visible;
            EnableDragAndDrop();
        }
        public static void Finish()
        {
            DisableDragAndDrop();
            MainWindow.Instance.BackgroundImagePanel.Visibility = Visibility.Hidden;
        }
        public static void EnableDragAndDrop()
        {
            if (isDragAndDropEnabled) return;
            isDragAndDropEnabled = true;
            var canvas = MainWindow.Instance.DrawCanvas;
            canvas.AllowDrop = true;
            canvas.PreviewDrop += Canvas_PreviewDrop;
            canvas.PreviewDragOver += Canvas_PreviewDragOver;
        }

        public static void DisableDragAndDrop()
        {
            isDragAndDropEnabled = false;
            var canvas = MainWindow.Instance.DrawCanvas;
            canvas.AllowDrop = false;
            canvas.PreviewDrop -= Canvas_PreviewDrop;
            canvas.PreviewDragOver -= Canvas_PreviewDragOver;
        }
        private static void Canvas_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsImageFile(files[0]))
                {
                    MainWindow.Instance.BgImageGrid.Background = LoadImageBrush(files[0]);
                }
            }
        }
        private static void Canvas_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && IsImageFile(files[0]))
                    e.Effects = DragDropEffects.Copy;
                else
                    e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }
        private static bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif";
        }
        public static void EraseBackgroundImage()
        {
            MainWindow.Instance.BgImageGrid.Background = null;
        }
        public static void OpenBackgroundImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Title = "Open Background Image"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true && IsImageFile(openFileDialog.FileName))
            {
                MainWindow.Instance.BgImageGrid.Background = LoadImageBrush(openFileDialog.FileName);
            }
        }
        private static ImageBrush LoadImageBrush(string filePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(filePath);
            bitmap.EndInit();
            bitmap.Freeze(); // Gibt das Bild für die UI frei und spart Speicher-Overhead

            return new ImageBrush
            {
                ImageSource = bitmap
            };
        }
    }
}
