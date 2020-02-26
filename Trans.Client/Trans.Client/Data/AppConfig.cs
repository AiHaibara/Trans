using System;
using HandyControl.Data;
using Trans.Client.Models;

namespace Trans.Client.Data
{
    internal class AppConfig
    {
        public static readonly string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}config.json";

        public string Lang { get; set; } = "zh-cn";

        public SkinType Skin { get; set; }

        public TransConfig TransConfig { get; set; }
        public bool NotifyIconNotified { get; set; }
    }
}