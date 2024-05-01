using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;
using System.Windows;

namespace PhotosWidget
{
    public class UserConfig
    {
        public int ModeCode { get; set; } = 0;
        public string CurrentPhotoFilePath { get; set; } = "";
        public string CurrentPhotoFolderPath { get; set; } = "";
        public int SlideIntervalSeconds { get; set; } = 42;
        public bool IsLocked { get; set; } = false;
        public int WidgetWidth { get; set; } = 417;
        public int WidgetHeight { get; set; } = 579;
        public int BorderRadius { get; set; } = 8;
        public int BorderWidth { get; set; } = 2;
        public double LocationX { get; set; } = -1;
        public double LocationY { get; set; } = -1;

        public string ToConfigText()
        {
            var result = "";

            result += $"ModeCode = {ModeCode}\n";
            result += $"CurrentPhotoFilePath = {CurrentPhotoFilePath}\n";
            result += $"CurrentPhotoFolderPath = {CurrentPhotoFolderPath}\n";
            result += $"SlideIntervalSeconds = {SlideIntervalSeconds}\n";
            result += $"IsLocked = {IsLocked}\n";
            result += $"WidgetWidth = {WidgetWidth}\n";
            result += $"WidgetHeight = {WidgetHeight}\n";
            result += $"BorderRadius = {BorderRadius}\n";
            result += $"BorderWidth = {BorderWidth}\n";
            result += $"LocationX = {LocationX}\n";
            result += $"LocationY = {LocationY}\n";

            return result;
        }

        public static UserConfig LoadFromConfigText(string configString)
        {
            var config = new UserConfig();
            
            var lines = configString.Split('\n');
            foreach (var line in lines)
            {
                var keyValue = line.Split('=');
                switch (keyValue[0].Trim())
                {
                    case "ModeCode":
                        config.ModeCode = int.Parse(keyValue[1].Trim() ?? "0");
                        break;
                    case "CurrentPhotoFilePath":
                        config.CurrentPhotoFilePath = keyValue[1].Trim() ?? "";
                        break;
                    case "CurrentPhotoFolderPath":
                        config.CurrentPhotoFolderPath = keyValue[1].Trim() ?? "";
                        break;
                    case "SlideIntervalSeconds":
                        config.SlideIntervalSeconds = int.Parse(keyValue[1].Trim() ?? "0");
                        break;
                    case "IsLocked":
                        config.IsLocked = (keyValue[1].Trim() ?? "False") == "True";
                        break;
                    case "WidgetWidth":
                        config.WidgetWidth = int.Parse(keyValue[1].Trim() ?? "0");
                        break;
                    case "WidgetHeight":
                        config.WidgetHeight = int.Parse(keyValue[1].Trim() ?? "0");
                        break;
                    case "BorderRadius":
                        config.BorderRadius = int.Parse(keyValue[1].Trim() ?? "0");
                        break;
                    case "BorderWidth":
                        config.BorderWidth = int.Parse(keyValue[1].Trim() ?? "0");
                        break;
                    case "LocationX":
                        config.LocationX = double.Parse(keyValue[1].Trim() ?? "-1");
                        break;
                    case "LocationY":
                        config.LocationY = double.Parse(keyValue[1].Trim() ?? "-1");
                        break;
                    default:
                        break;
                }
            }

            return config;
        }

        public void Save()
        {
            string configFilename = ".config";

            var currentAppPath = Directory.GetCurrentDirectory();
            var configFilePath = Path.Combine(currentAppPath, configFilename);

            File.WriteAllText(configFilePath, ToConfigText());
        }

        public static UserConfig Load()
        {
            string configFilename = ".config";

            var currentAppPath = Directory.GetCurrentDirectory();
            var configFilePath = Path.Combine(currentAppPath, configFilename);

            if (File.Exists(configFilePath) == false)
            {
                var userConfig = new UserConfig();
                File.WriteAllText(configFilePath, userConfig.ToConfigText());
                return userConfig;
            }
            else
            {
                var configString = File.ReadAllText(configFilePath);
                return LoadFromConfigText(configString);
            }
        }
    }
}
