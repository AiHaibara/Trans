using System;
using System.Collections.Generic;
using System.Text;
using static Trans.Client.Helper.CustomTrans;

namespace Trans.Client.Helper
{
    public interface ITranslator
    {
        public string Translate(MyResult src);
        public void setTo(string to);
    }
}
