using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Trans.Client.Helper.CustomTrans;

namespace Trans.Client.Helper
{
    public interface ITranslator
    {
        public Task<string> Translate();
        public Task<string> Translate(MyResult src);
        public void setTo(string to);
    }
}
