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
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Threading;

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

        private DispatcherTimer SlideTimer { get; set; }

        //public Config UserConfig { get; set; }

        private bool LoopingTheFolder { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();

            SlideTimer = new DispatcherTimer();
            SlideTimer.Interval = TimeSpan.FromSeconds(5);
            SlideTimer.Tick += SlideImages;
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
                    SlideTimer.Stop();
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
                    Mode = WidgetMode.Slide;
                    CurrentPhotosFolderPath = dialog.SelectedPath;
                    //Console.WriteLine(CurrentPhotosFolderPath);

                    var directoryInfo = new DirectoryInfo(dialog.SelectedPath);
                    var files = directoryInfo.GetFiles("*.*");
                    FolderImagePaths = new List<string>();
                    //SlideThread.Abort();

                    foreach (var file in files)
                    {
                        //Console.WriteLine(file.Name);
                        if (Regex.IsMatch(file.Name, @"\.jpg$|\.png$|\.jpeg$|\.jfif$|\.webp$|\.bmp$|\.gif$"))
                        {
                            FolderImagePaths.Add(file.FullName);
                            //Console.WriteLine(file.FullName);
                        }
                    }

                    SlideImages(sender, e);

                    SlideTimer.Start();
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

            appTitleLabel.Visibility = Visibility.Hidden;
            appPromptLabel.Visibility = Visibility.Hidden;
            StagePhoto.Visibility = Visibility.Visible;
            StagePhoto.Source = bi;
        }

        private void SlideImages(object sender, EventArgs e)
        {
            if (FolderImagePaths.Count == 0)
            {
                SlideTimer.Stop();
                return;
            }

            var imageToLoad = FolderImagePaths[0];
            LoadImage(FolderImagePaths[0]);
            FolderImagePaths.RemoveAt(0);
            FolderImagePaths.Add(imageToLoad);
        }

    }

    public enum WidgetMode
    {
        Static = 0,
        Slide = 1
    }
}
