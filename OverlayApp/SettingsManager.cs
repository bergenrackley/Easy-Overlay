using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OverlayApp
{
    public class BoundingBox
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }

        public BoundingBox()
        {
            StartX = 0;
            StartY = 0;
            EndX = 0;
            EndY = 0;
        }
    }
    public class Settings
    {
        public BoundingBox BoundingBox { get; set; }
        public int MonitorScale { get; set; }
        public int Opacity { get; set; }

        public Settings()
        {
            BoundingBox = new BoundingBox();
            MonitorScale = 100;
            Opacity = 30;
        }
    }

    public class SettingsManager
    {
        private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string directoryPath = Path.Combine(appDataPath, "EasyOverlay");
        private static string settingsPath = Path.Combine(directoryPath, "settings.json");
        public Settings Settings { get; set; }

        public SettingsManager()
        {
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                Settings = JsonSerializer.Deserialize<Settings>(json);
            }
            else
            {
                Settings = new Settings();
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
        }

        public void SaveSettings()
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
            catch
            {

            }
        }
    }
}
