using HandyControl.Controls;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Trans.Client.Tools.Helper;

namespace Trans.Client.Windows
{
    public sealed class Sprite
    {
        private static void MoveBottomRightEdgeOfWindowToMousePosition(PopupWindow window)
        {
            var transform = PresentationSource.FromVisual(window).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(new Point(PT.X, PT.Y));
            window.Left = mouse.X-20;
            window.Top = mouse.Y-20;
        }
        public static PopupWindow Popup { get; set; }
        public static Point PT { get; set; }
        public static void Hide()
        {
            if (Popup != null)
            {
                Popup.Visibility = Visibility.Hidden;
            }
        }

        public static PopupWindow Show(object content, Point pt)
        {
            PT = pt;
            if (Popup != null)
            {
                Popup.Visibility = Visibility.Visible;
                MoveBottomRightEdgeOfWindowToMousePosition(Popup);
                return Popup;
            }
            var window = new PopupWindow
            {
                PopupElement = content as FrameworkElement,
            };
            window.Loaded += (s, e) =>
            {
                if(window.Visibility==Visibility.Visible)
                    MoveBottomRightEdgeOfWindowToMousePosition(window);
            };

            window.ShowActivated = false;
            window.Topmost = true;
            window.Show(content as FrameworkElement, false);
            Popup = window;
            return window;
        }
    }
}
