using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Trans.Client.Tools.Helper
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static implicit operator Point(POINT p){
            return new Point(p.X, p.Y);
        }
    }
    public class InteropMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetCursorPos(out POINT pt);
    }
}
