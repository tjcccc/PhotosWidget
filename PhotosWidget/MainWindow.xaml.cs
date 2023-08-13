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

        public UserConfig UserConfig { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            SlideTimer = new DispatcherTimer();
            SlideTimer.Interval = TimeSpan.FromSeconds(5);
            SlideTimer.Tick += SlideImages;

            UserConfig = UserConfig.Load();

            switch (UserConfig.ModeCode)
            {
                case (int)WidgetMode.Static:
                    if (string.IsNullOrEmpty(UserConfig.CurrentPhotoFilePath) == false)
                    {
                        CurrentPhotoFilePath = UserConfig.CurrentPhotoFilePath;
                        LoadImage(CurrentPhotoFilePath);
                    }
                    break;
                case (int)WidgetMode.Slide:
                    if (string.IsNullOrEmpty(UserConfig.CurrentPhotoFolderPath) == false)
                    {
                        CurrentPhotosFolderPath = UserConfig.CurrentPhotoFolderPath;
                        FolderImagePaths = GetImageFilePathsFromFolder(CurrentPhotosFolderPath);
                        
                        SlideImages();

                        SlideTimer.Start();
                    }
                    break;
            }

            
            IsLocked = UserConfig.IsLocked;
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
            UserConfig.IsLocked = IsLocked;
            UserConfig.Save();
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

                    UserConfig.ModeCode = (int)WidgetMode.Static;
                    UserConfig.CurrentPhotoFilePath = dialog.FileName;
                    UserConfig.Save();

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

                    UserConfig.ModeCode = (int)WidgetMode.Slide;
                    UserConfig.CurrentPhotoFolderPath = dialog.SelectedPath;
                    UserConfig.Save();

                    FolderImagePaths = GetImageFilePathsFromFolder(dialog.SelectedPath);

                    // Load the first image immidiately.
                    SlideImages(sender, e);

                    SlideTimer.Start();
                }
            }
        }

        private List<string> GetImageFilePathsFromFolder(string folderPath)
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            var files = directoryInfo.GetFiles("*.*");

            var imageFilePaths = new List<string>();

            foreach (var file in files)
            {
                //Console.WriteLine(file.Name);
                if (Regex.IsMatch(file.Name, @"\.jpg$|\.png$|\.jpeg$|\.jfif$|\.webp$|\.bmp$|\.gif$"))
                {
                    imageFilePaths.Add(file.FullName);
                    //Console.WriteLine(file.FullName);
                }
            }

            return imageFilePaths;
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

        private void SlideImages(object sender = null, EventArgs e = null)
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
