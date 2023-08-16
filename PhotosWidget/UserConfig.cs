using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

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
        public int BorderRadious { get; set; } = 8;
        public int BorderWidth { get; set; } = 2;

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
            result += $"BorderRadious = {BorderRadious}\n";
            result += $"BorderWidth = {BorderWidth}\n";

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
                    case "BorderRadious":
                        config.BorderRadious = int.Parse(keyValue[1].Trim() ?? "0");
                        break;
                    case "BorderWidth":
                        config.BorderWidth = int.Parse(keyValue[1].Trim() ?? "0");
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
                return UserConfig.LoadFromConfigText(configString);
            }
        }
    }
}
