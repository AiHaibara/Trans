using System.IO;
using System.Text.Json;
using System.Windows;

namespace Trans.Client.Data
{
    internal class GlobalData
    {
        //static GlobalData()
        //{
        //    Init();
        //}
        public static void Init()
        {
            if (File.Exists(AppConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(AppConfig.SavePath);
                    Config = (string.IsNullOrEmpty(json) ? new AppConfig() : JsonSerializer.Deserialize<AppConfig>(json)) ?? new AppConfig();
                }
                catch
                {
                    Config = new AppConfig();
                }
            }
            else
            {
                Config = new AppConfig();
            }
        }

        public static void Save()
        {
            var json = JsonSerializer.Serialize(Config);
            File.WriteAllText(AppConfig.SavePath, json);
        }

        public static AppConfig Config { get; set; }

        public static DpiScale DpiScale { get; set; }

        public static double ScreenHeight { get; set; }
        public static double ScreenWidth { get; set; }

        public static bool IsAutoRun { get; set; }
    }
}