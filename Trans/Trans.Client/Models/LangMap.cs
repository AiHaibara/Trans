using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trans.Client.Helper;

namespace Trans.Client.Models
{
    //public class LangData
    //{
    //    public string From { get; set; }
    //    public string To { get; set; }
    //    public string Name { get; set; }
    //}
    public class LangData
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LangType type { get; set; }
        public string ocr { get; set; }
        public string trans { get; set; }
        public string name { get; set; }
        //public List<LangData> Langs { get; set; }
    }
}
