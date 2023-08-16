using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Windows.Media.Animation;

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
        //private Image CurrentImage { get; set; }
        public bool IsLocked { get; set; } = false;

        private DispatcherTimer SlideTimer { get; set; }

        public UserConfig UserConfig { get; set; }
        private bool IsApplyingConfig { get; set; }

        //private readonly DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
        //private readonly DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0.5d, TimeSpan.FromSeconds(1));

        public MainWindow()
        {
            InitializeComponent();
            InitializeApp();

            this.DataContext = this;
        }

        private void InitializeApp()
        {
            UserConfig = UserConfig.Load();
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            IsApplyingConfig = true;

            SlideTimer?.Stop();
            SlideTimer = new DispatcherTimer();
            SlideTimer.Interval = TimeSpan.FromSeconds(UserConfig.SlideIntervalSeconds);
            SlideTimer.Tick += SlideImages;

            main.Width = UserConfig.WidgetWidth;
            main.Height = UserConfig.WidgetHeight;

            Mode = (WidgetMode)UserConfig.ModeCode;

            IsLocked = UserConfig.IsLocked;
            lockOption.Header = IsLocked ? Properties.Resources.unlockLabel : Properties.Resources.lockLabel;

            mainBorder.CornerRadius = new CornerRadius(UserConfig.BorderRadius * 1.2f);
            contentBorder.CornerRadius = new CornerRadius(UserConfig.BorderRadius);

            mainBorder.Padding = new Thickness(UserConfig.BorderWidth);

            switch (UserConfig.ModeCode)
            {
                case (int)WidgetMode.Static:
                    if (string.IsNullOrEmpty(UserConfig.CurrentPhotoFilePath) == false)
                    {
                        contentBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ff000000");
                        CurrentPhotoFilePath = UserConfig.CurrentPhotoFilePath;
                        LoadImage(CurrentPhotoFilePath);
                    }
                    else
                    {
                        contentBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ff000000");
                        appTitleLabel.Visibility = Visibility.Visible;
                        appPromptLabel.Visibility = Visibility.Visible;
                        stagePhoto.Visibility = Visibility.Hidden;
                        stagePhoto.Source = null;
                    }
                    break;
                case (int)WidgetMode.Slide:
                    if (string.IsNullOrEmpty(UserConfig.CurrentPhotoFolderPath) == false)
                    {
                        contentBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ff000000");
                        CurrentPhotosFolderPath = UserConfig.CurrentPhotoFolderPath;
                        FolderImagePaths = GetImageFilePathsFromFolder(CurrentPhotosFolderPath);

                        SlideImages();
                        SlideTimer.Start();
                    }
                    else
                    {
                        appTitleLabel.Visibility = Visibility.Visible;
                        appPromptLabel.Visibility = Visibility.Visible;
                        stagePhoto.Visibility = Visibility.Hidden;
                        stagePhoto.Source = null;
                    }
                    break;
            }

            if (string.IsNullOrEmpty(UserConfig.CurrentPhotoFilePath) && string.IsNullOrEmpty(UserConfig.CurrentPhotoFolderPath))
            {
                contentBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffc825");
            }

            IsApplyingConfig = false;
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsApplyingConfig)
            {
                return;
            }

            var height = main.Height;
            var width = main.Width;
            UserConfig.WidgetHeight = (int)height;
            UserConfig.WidgetWidth = (int)width;
            UserConfig.Save();
        }

        private void SwitchLock(object sender, RoutedEventArgs e)
        {
            IsLocked = !IsLocked;

            main.ResizeMode = IsLocked ? ResizeMode.NoResize : ResizeMode.CanResize;
            lockOption.Header = IsLocked ? Properties.Resources.unlockLabel : Properties.Resources.lockLabel;
            UserConfig.IsLocked = IsLocked;
            UserConfig.Save();
        }

        public void OpenPhotoFileDialog(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Filter = "Image Files|*.jpg;*.png;*.jpeg;*.jfif;*.webp;*.bmp";
                
                var result = dialog.ShowDialog();
                
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Mode = WidgetMode.Static;
                    SlideTimer.Stop();
                    stagePhoto.Opacity = 1;
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
                    stagePhoto.Source = null;
                    SlideImages(sender, e);
                    //LoadImage(FolderImagePaths[0]);

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
                if (Regex.IsMatch(file.Name, @"\.jpg$|\.png$|\.jpeg$|\.jfif$|\.webp$|\.bmp$"))
                {
                    imageFilePaths.Add(file.FullName);
                    //Console.WriteLine(file.FullName);
                }
            }

            return imageFilePaths;
        }

        private void LoadImage(string filePath)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.Absolute);
            bi.EndInit();

            appTitleLabel.Visibility = Visibility.Hidden;
            appPromptLabel.Visibility = Visibility.Hidden;
            stagePhoto.Visibility = Visibility.Visible;

            //if (Mode == WidgetMode.Static)
            //{
            //    stagePhoto.Source = bi;
            //    return;
            //}

            stagePhoto.Source = bi;

            //if (stagePhoto.Source != null)
            //{
            //    stagePhoto.BeginAnimation(OpacityProperty, fadeOutAnimation);
            //    fadeOutAnimation.Completed += (s, e) =>
            //    {
            //        if (Mode == WidgetMode.Static)
            //        {
            //            stagePhoto.Opacity = 1;
            //            return;
            //        }
            //        stagePhoto.Source = bi;
            //        stagePhoto.BeginAnimation(OpacityProperty, fadeInAnimation);
            //    };
            //}
            //else
            //{
            //    if (Mode == WidgetMode.Static)
            //    {
            //        stagePhoto.Opacity = 1;
            //        return;
            //    }
            //    stagePhoto.Opacity = 0;
            //    stagePhoto.Source = bi;
            //    stagePhoto.BeginAnimation(OpacityProperty, fadeInAnimation);
            //    fadeInAnimation.Completed += (s, e) =>
            //    {
            //        stagePhoto.Opacity = 1;
            //    };
            //}



        }

        private void SlideImages(object sender = null, EventArgs e = null)
        {
            if (FolderImagePaths.Count == 0)
            {
                SlideTimer.Stop();
                return;
            }

            var imageToLoad = FolderImagePaths[0];
            LoadImage(imageToLoad);
            FolderImagePaths.RemoveAt(0);
            FolderImagePaths.Add(imageToLoad);
        }

        public void Reset(object sender, RoutedEventArgs e)
        {
            UserConfig = new UserConfig();
            UserConfig.Save();
            
            ApplyConfig();
        }

        public void OpenSettings(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings(UserConfig);
            settingsWindow.Owner = this;
            settingsWindow.Closed += (s, args) =>
            {
                UserConfig.Save();
                ApplyConfig();
            };

            settingsWindow.ShowDialog();
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

    }

    public enum WidgetMode
    {
        Static = 0,
        Slide = 1
    }
}
