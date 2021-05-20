using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trans.Client.Tools.Helper
{
    public class PathHelper
    {
        public static string FullPath(string path)
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}{path}";
        }

        public static String GetFileBase64(String fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Open);
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string baser64 = Convert.ToBase64String(arr);
            filestream.Close();
            return baser64;
        }
    }
}
