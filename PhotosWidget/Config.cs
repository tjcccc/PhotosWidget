using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosWidget
{
    class Config
    {
        public int ModeCode { get; set; } = 0;
        public string CurrentPhotoFilePath { get; set; } = "";
        public string CurrentPhotoFolderPath { get; set; } = "";
        public int SlideIntervalSeconds { get; set; } = 42;
        public bool IsLocked { get; set; } = false;
        public int WidgetWidth { get; set; } = 417;
        public int WidgetHeight { get; set; } = 579;
        public bool BorderEnabled { get; set; } = false;
        public bool ShadowEnabled { get; set; } = false;
        
    }
}
