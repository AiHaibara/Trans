﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Trans.Client.Windows
{
    public class CropWindow
    {
        public static void ShowDialog()
        {
            flag = true;
            //MainWindow.Instance.Deactivated -= Instance_StateChanged;
            //MainWindow.Instance.Deactivated += Instance_StateChanged;
            MainWindow.Instance.WindowState = WindowState.Minimized;
            Instance_StateChanged(null, null);
        }
        public static bool flag = false;

        private static void Instance_StateChanged(object sender, EventArgs e)
        {
            Thread.Sleep(200);
            if (MainWindow.Instance.WindowState != WindowState.Minimized)
                return;
            if (flag == false)
                return;
            flag = false;
            Window window = new Window();
            window.Owner = null;
            window.Cursor = Cliper.Instance;
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowStyle = WindowStyle.None;
            window.WindowState = WindowState.Maximized;
            var canvas = new Canvas();
            canvas.Background = new SolidColorBrush(Colors.Transparent);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            var rectangle = new RectangleGeometry();
            path.Data = rectangle;
            path.Stroke = new SolidColorBrush() { Color = Colors.Red, Opacity = 1f };
            window.Content = canvas;
            BitmapSource source = CopyScreen();
            var image = new System.Windows.Controls.Image() { Source = source };
            canvas.Children.Add(image);
            canvas.Children.Add(path);
            //System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            System.Windows.Point? start = null;
            System.Windows.Point? end = null;
            window.MouseDown += (s, e) =>
            {
                window.CaptureMouse();
                start = e.GetPosition(window);
                //start = new System.Windows.Point(Math.Max(0d, start.Value.X),Math.Max(0d, start.Value.Y));
                Canvas.SetLeft(path, start.Value.X);
                Canvas.SetTop(path, start.Value.Y);
            };
            window.MouseMove += (ss, ee) =>
            {
                if (start.HasValue)
                {
                    end = ee.GetPosition(window);
                    //end = new System.Windows.Point(Math.Max(0d, end.Value.X), Math.Max(0d, end.Value.Y));
                    Canvas.SetLeft(path, Math.Min(start.Value.X,end.Value.X));
                    Canvas.SetTop(path, Math.Min(start.Value.Y,end.Value.Y));
                    rectangle.Rect = new Rect(
                        start.Value.X < end.Value.X ? start.Value : end.Value, 
                        start.Value.X < end.Value.X ? end.Value : start.Value);
                    canvas.InvalidateVisual();
                }
                //rectangle.Width = Math.Max(0,eee.GetPosition(window).X - sx);
                //rectangle.Height = Math.Max(0,eee.GetPosition(window).Y - sy);
            };
            window.MouseUp += (sss, eee) =>
            {
                window.ReleaseMouseCapture();
                if (start.HasValue && end.HasValue)
                {
                    SaveImage(source, new Int32Rect(start.Value.X < end.Value.X ? (int)start.Value.X : (int)end.Value.X,
                        start.Value.Y < end.Value.Y ? (int)start.Value.Y:(int)end.Value.Y,
                        (int)rectangle.Rect.Width, (int)rectangle.Rect.Height));
                }
                source = null;
                image.Source = null;
                window.Close();
                MainWindow.Instance.WindowState = WindowState.Normal;
            };
            window.Deactivated += (aa, bb) =>
            {
                Window window = (Window)aa;
                window.Topmost = true;
            };
            window.Activate();
            window.Topmost = true;
            window.ShowDialog();
        }
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);
        public static void SaveImage(BitmapSource img, Int32Rect rect)
        {
            if (img == null)
                throw new ArgumentNullException("img");
            Console.WriteLine(rect);
            var crop = new CroppedBitmap(img, new Int32Rect(rect.X, rect.Y, Math.Max(1,rect.Width), Math.Max(1,rect.Height)));
            using (var fileStream = new FileStream(@"source.jpg", FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(crop));
                encoder.Save(fileStream);
            }
        }
        private static Bitmap GetScreen()
        {
            return new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }
        private static BitmapSource CopyScreen()
        {
            using (var screenBmp = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
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

        public enum ImageFormats
        {
            PNG,
            BMP,
            JPG
        }
    }
}