using System;
using System.Collections.Generic;
using System.Text;

namespace Trans.Client.Tools.Helper
{
    public class PathHelper
    {
        public static string FullPath(string path)
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}{path}";
        }
    }
}
