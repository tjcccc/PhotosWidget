using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotosWidget
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public UserConfig UserConfig { get; set; }

        private int[] defaultLandscapeSize = new int[] { 579, 417 };
        private int[] defaultPortraitSize = new int[] { 417, 579 };

        public Settings(UserConfig userConfig)
        {
            InitializeComponent();

            UserConfig = userConfig;
            widthTextBox.Text = userConfig.WidgetWidth.ToString();
            heightTextBox.Text = userConfig.WidgetHeight.ToString();
            borderRadiousTextBox.Text = userConfig.BorderRadious.ToString();
            borderWidthTextBox.Text = userConfig.BorderWidth.ToString();
            slideIntervalTextbox.Text = userConfig.SlideIntervalSeconds.ToString();
        }

        private void PreviewTextboxInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void defaultLandscapeButton_Click(object sender, RoutedEventArgs e)
        {
            widthTextBox.Text = defaultLandscapeSize[0].ToString();
            heightTextBox.Text = defaultLandscapeSize[1].ToString();
        }

        private void defaultPortraitButton_Click(object sender, RoutedEventArgs e)
        {
            widthTextBox.Text = defaultPortraitSize[0].ToString();
            heightTextBox.Text = defaultPortraitSize[1].ToString();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfig.WidgetWidth = int.Parse(widthTextBox.Text);
            UserConfig.WidgetHeight = int.Parse(heightTextBox.Text);
            UserConfig.BorderRadious = int.Parse(borderRadiousTextBox.Text);
            UserConfig.BorderWidth = int.Parse(borderWidthTextBox.Text);
            UserConfig.SlideIntervalSeconds = int.Parse(slideIntervalTextbox.Text);
            //UserConfig.Save();
            
            Close();
        }
    }
}
