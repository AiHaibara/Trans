using System.Collections.Generic;


namespace Trans.Client.Data
{
    public class DemoDataModel
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public string Remark { get; set; }

        public DemoType Type { get; set; }

        public string ImgPath { get; set; }

        public string FromFull { get; set; }
        public string ToFull { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public List<DemoDataModel> DataList { get; set; }
    }
}