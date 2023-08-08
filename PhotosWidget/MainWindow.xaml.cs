using System;
using System.IO;
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
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PhotosWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WidgetMode Mode { get; set; } = WidgetMode.Static;
        private string CurrentPhotoFilePath { get; set; }
        private string CurrentPhotosFolderPath { get; set; }
        private List<string> FolderImagePaths { get; set; } = new List<string>();
        private Image CurrentImage { get; set; }
        public bool IsLocked { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EnableDrag(object sender, MouseButtonEventArgs e)
        {
            if (IsLocked)
            {
                return;
            }

            var move = sender as Window;
            if (move != null)
            {
                Window win = Window.GetWindow(move);
                win.DragMove();
            }
        }

        private void SwitchLock(object sender, RoutedEventArgs e)
        {
            IsLocked = !IsLocked;
        }

        public void OpenPhotoFileDialog(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Filter = "Image Files|*.jpg;*.png;*.jpeg;*.jfif;*.webp;*.bmp;*.gif";
                
                var result = dialog.ShowDialog();
                
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Mode = WidgetMode.Static;
                    CurrentPhotoFilePath = dialog.FileName;

                    LoadImage(dialog.FileName);
                }
            }
        }

        public void OpenPhotoFolderDialog(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Mode = WidgetMode.Slider;
                    CurrentPhotosFolderPath = dialog.SelectedPath;
                    Console.WriteLine(CurrentPhotosFolderPath);

                    var directoryInfo = new DirectoryInfo(dialog.SelectedPath);
                    var files = directoryInfo.GetFiles("*.*");
                    foreach (var file in files)
                    {
                        //Console.WriteLine(file.Name);
                        if (Regex.IsMatch(file.Name, @"\.jpg$|\.png$|\.jpeg$|\.jfif$|\.webp$|\.bmp$|\.gif$"))
                        {
                            FolderImagePaths.Add(file.FullName);
                            //Console.WriteLine(file.FullName);
                        }
                    }
                    //Console.WriteLine(FolderImagePaths);
                    SlideImages(FolderImagePaths);
                }
            }
        }

        public void OpenSettings(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void LoadImage(string filePath)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.Absolute);
            bi.EndInit();

            StagePhoto.Source = bi;
        }

        private async void SlideImages(List<string> filePaths)
        {
            if (filePaths.Count == 0)
            {
                return;
            }

            for (var i = 0; i < filePaths.Count; i += 1)
            {
                if (Mode == WidgetMode.Static)
                {
                    break;
                }

                if (i + 1 == filePaths.Count)
                {
                    i = 0;
                }

                LoadImage(filePaths[i]);

                await Task.Delay(3000);
            }
        }

    }

    public enum WidgetMode
    {
        Static,
        Slider
    }
}
