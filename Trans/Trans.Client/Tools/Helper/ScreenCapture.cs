using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Trans.Client.Data;
using Trans.Client.Tools.Helper;

namespace Trans.Client.Tools
{
    public class ScreenCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        public static Image CaptureDesktop()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);
            }

            return result;
        }

        private static Bitmap GetScreen()
        {
            return new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

        public static BitmapSource CopyScreen()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = 96 / graphics.DpiX;
                float dpiY = 96 / graphics.DpiY;
                using (var screenBmp = new Bitmap((int)graphics.VisibleClipBounds.Width, (int)graphics.VisibleClipBounds.Height, System.Drawing.Imaging.PixelFormat.Format64bppArgb))
                {
                    IntPtr hbitmap = IntPtr.Zero;
                    try
                    {
                        using (var bmpGraphics = Graphics.FromImage(screenBmp))
                        {
                            bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                            hbitmap = screenBmp.GetHbitmap();
                            var bitmap = Imaging.CreateBitmapSourceFromHBitmap(
                                hbitmap,
                                IntPtr.Zero,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                            bitmap?.Freeze();
                            return bitmap;
                        }
                    }
                    finally
                    {
                        if (hbitmap != IntPtr.Zero)
                        {
                            DeleteObject(hbitmap);
                        }
                    }
                }
            }
        }

        public static void SaveImage(BitmapSource img, Int32Rect rect)
        {
            if (img == null)
                throw new ArgumentNullException("img");
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = 96 / graphics.DpiX;
                float dpiY = 96 / graphics.DpiY;
                var crop = new CroppedBitmap(img, new Int32Rect((int)(rect.X/dpiX), (int)(rect.Y/dpiY), (int)(rect.Width/dpiX), (int)(rect.Height/dpiY)));
                //var crop = new CroppedBitmap(img, new Int32Rect((int)(rect.X * Data.GlobalData.DpiScale.DpiScaleX), (int)(rect.Y * Data.GlobalData.DpiScale.DpiScaleY), Math.Max(1, (int)(rect.Width * Data.GlobalData.DpiScale.DpiScaleX)), Math.Max(1, (int)(rect.Height * Data.GlobalData.DpiScale.DpiScaleY))));
                using (var fileStream = new FileStream(PathHelper.FullPath(GlobalData.SourcePath), FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(crop));
                    encoder.Save(fileStream);
                }
            }
        }
    }
}
