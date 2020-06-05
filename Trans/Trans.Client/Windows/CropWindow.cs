using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
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
using Trans.Client.Tools;
using Trans.Client.Tools.Helper;

namespace Trans.Client.Windows
{
    public class CropWindow
    {
        public static Action Do { get; set; }
        public static void Show()
        {

            flag = true;
            //MainWindow.Instance.Deactivated -= Instance_StateChanged;
            //MainWindow.Instance.Deactivated += Instance_StateChanged;
            MainWindow.Instance.WindowState = WindowState.Minimized;
            Show(null, null);
        }
        public static bool flag = false;
        private static void Show(object sender, EventArgs e)
        {
            Thread.Sleep(200);
            //if (MainWindow.Instance.WindowState != WindowState.Minimized)
            //    return;
            if (flag == false)
                return;
            flag = false;
            Window window = new Window();
            window.Owner = null;
            window.Cursor = Cliper.Instance;
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowStyle = WindowStyle.None;
            //window.WindowState = WindowState.Maximized;
            var canvas = new Canvas();
            canvas.Background = new SolidColorBrush(Colors.Transparent);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            var rectangle = new RectangleGeometry();
            path.Data = rectangle;
            path.Stroke = new SolidColorBrush() { Color = Colors.Red, Opacity = 1f };
            window.Content = canvas;
            //var scale=getScalingFactor();
            BitmapSource source = ScreenCapture.CopyScreen();
            System.Windows.Controls.Image image = null;
            window.Width = SystemParameters.PrimaryScreenWidth;
            window.Height = SystemParameters.PrimaryScreenHeight;
            window.Left = 0;
            window.Top = 0;
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = 96/graphics.DpiX;
                float dpiY = 96/graphics.DpiY;
                Data.GlobalData.DpiScale = new DpiScale(Data.GlobalData.ScreenHeight/(graphics.VisibleClipBounds.Height * dpiY), Data.GlobalData.ScreenWidth/(graphics.VisibleClipBounds.Width*dpiX));
                //image = new System.Windows.Controls.Image() { Source = source, SnapsToDevicePixels = true, Height =graphics.VisibleClipBounds.Height*dpiY, Width=graphics.VisibleClipBounds.Width*dpiX };
                image = new System.Windows.Controls.Image() { Source = source };
                image.Width = SystemParameters.PrimaryScreenWidth;
                image.Height = SystemParameters.PrimaryScreenHeight;
                //RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
                canvas.Children.Add(image);
            }
            canvas.Children.Add(path);
            //System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
            System.Windows.Point? start = null;
            System.Windows.Point? end = null;
            window.MouseDown += (s, e) =>
            {
                e.Handled = true;
                window.CaptureMouse();
                start = e.GetPosition(window);
                //start = new System.Windows.Point(Math.Max(0d, start.Value.X),Math.Max(0d, start.Value.Y));
                Canvas.SetLeft(path, start.Value.X);
                Canvas.SetTop(path, start.Value.Y);
            };
            window.MouseMove += (ss, ee) =>
            {
                ee.Handled = true;
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
                eee.Handled = true;
                window.ReleaseMouseCapture();
                if (start.HasValue && end.HasValue)
                {
                    ScreenCapture.SaveImage(source, new Int32Rect(start.Value.X < end.Value.X ? (int)start.Value.X : (int)end.Value.X,
                        start.Value.Y < end.Value.Y ? (int)start.Value.Y:(int)end.Value.Y,
                        (int)rectangle.Rect.Width, (int)rectangle.Rect.Height));
                    window.Cursor = Cursors.Arrow;
                }
                
                //InteropMethods.SetForegroundWindow(current);

                source = null;
                image.Source = null;
                window.Close();
                Do();
                //MainWindow.Instance.WindowState = WindowState.Normal;
            };
            //window.Deactivated += (aa, bb) =>
            //{
            //    Window window = (Window)aa;
            //    window.Topmost = true;
            //};
            //window.ShowActivated = false;
            //var current = InteropMethods.GetForegroundWindow();
            window.ShowActivated = false;
            window.Topmost = true;
            window.Show();
            var handle = new WindowInteropHelper(window).Handle;
            int exstyle = (int)InteropMethods.GetWindowLong(handle, InteropMethods.GWL_EXSTYLE);
            InteropMethods.SetWindowLong(handle, InteropMethods.GWL_EXSTYLE, (IntPtr)(exstyle | ((int)InteropMethods.WS_EX_NOACTIVATE | ((int)InteropMethods.WS_EX_TOOLWINDOW))));
            //InteropMethods.SetForegroundWindow(current);
            //InteropMethods.ShowWindow(current, 9);
            //InteropMethods.SetWindowPos(current, 0, 0, 0, 0, 0, InteropMethods.SWP_NOZORDER | InteropMethods.SWP_NOSIZE | InteropMethods.SWP_SHOWWINDOW);
        }
        public enum ImageFormats
        {
            PNG,
            BMP,
            JPG
        }
    }
}
