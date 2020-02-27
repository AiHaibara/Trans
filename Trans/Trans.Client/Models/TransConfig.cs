using System;
using System.Collections.Generic;
using System.Text;

namespace Trans.Client.Models
{
    public class TransConfig
    {
        //public string From { get; set; } = "en";
        public string To { get; set; } = "zh";
        public string HotKey { get; set; }
        public string BaiduOCRID { get; set; }
        public string BaiduOCRSECRET { get; set; }
        public string BaiduTranslateID { get; set; }
        public string BaiduTranslateKEY { get; set; }
    }
}
