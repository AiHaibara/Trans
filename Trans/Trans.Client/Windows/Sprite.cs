using HandyControl.Controls;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Trans.Client.Tools.Helper;

namespace Trans.Client.Windows
{
    public sealed class Sprite
    {
        private static void MoveBottomRightEdgeOfWindowToMousePosition(PopupWindow window, Point pt)
        {
            var transform = PresentationSource.FromVisual(window).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(new Point(pt.X, pt.Y));
            window.Left = mouse.X;
            window.Top = mouse.Y;
        }
        public static PopupWindow Show(object content, Point pt)
        {
            var window = new PopupWindow
            {
                PopupElement = content as FrameworkElement,
            };
            window.Loaded += (s, e) =>
            {
                MoveBottomRightEdgeOfWindowToMousePosition(window, pt);
            };
            window.Show(content as FrameworkElement, false);

            return window;
        }
    }
}
