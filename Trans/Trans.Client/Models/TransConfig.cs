using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trans.Client.Helper;

namespace Trans.Client.Models
{
    public class TransConfig
    {
        //public string From { get; set; } = "en";
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LangType To { get; set; } = LangType.en;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransStrategy Use { get; set; } = TransStrategy.Baidu;
        public string HotKey { get; set; }
        public string BaiduOCRID { get; set; }
        public string BaiduOCRSECRET { get; set; }
        public string BaiduTranslateID { get; set; }
        public string BaiduTranslateKEY { get; set; }
        public string TencentTranslateID { get; set; }
        public string TencentTranslateKEY { get; set; }
        public string GoogleTranslateKEY { get; set; }
        public string MicrosoftTranslateKEY { get; set; }
    }
}
