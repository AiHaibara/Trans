using System;
using System.Collections.Generic;
using System.Text;

namespace Trans.Client.Helper
{
    public interface ITranslator
    {
        public string Translate(string src);
        public void setTo(string to);
    }
}
