using System;
using System.Collections.Generic;
using System.Text;

namespace Trans.Client.Helper
{
    public interface ITrans
    {
        public IOcror GetOcror();
        public ITranslator GetTranslator();
    }
}
