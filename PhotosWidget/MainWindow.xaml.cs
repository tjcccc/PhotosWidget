using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotosWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WidgetMode Mode { get; set; } = WidgetMode.Static;
        private string CurrentImageFilePath { get; set; }
        private List<string> FolderImagePaths { get; set; }
        public Image CurrentImage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EnableDrag(object sender, MouseButtonEventArgs e)
        {
            var move = sender as Window;
            if (move != null)
            {
                Window win = Window.GetWindow(move);
                win.DragMove();
            }
        }

        public void OpenPhotoFileDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            dialog.Multiselect = false;

            bool? result = dialog.ShowDialog();
            
            if (result == true)
            {
                //Console.WriteLine(dialog.FileName);
                Mode = WidgetMode.Static;
                CurrentImageFilePath = dialog.FileName;

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
                bi.EndInit();

                StagePhoto.Source = bi;
            }
        }

        public void OpenPhotoFolderDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            dialog.Multiselect = false;

            bool? result = dialog.ShowDialog();

        }

        public void OpenSettings(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

    }

    public enum WidgetMode
    {
        Static,
        Slider
    }
}
