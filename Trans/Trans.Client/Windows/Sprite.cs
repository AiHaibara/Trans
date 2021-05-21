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
            window.Left = mouse.X;
            window.Top = mouse.Y-20;
        }
        private static void MoveBottomEdgeOfWindowToMousePosition(PopupWindow window,double width)
        {
            var transform = PresentationSource.FromVisual(window).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(new Point(PT.X, PT.Y));
            window.Left = mouse.X - width - 15;
            window.Top = mouse.Y - 20;
            window.InvalidateVisual();
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

        public static PopupWindow Show(object content, Point pt, double width)
        {
            PT = pt;
            if (Popup != null)
            {
                Popup.Width = width;
                Popup.PopupElement.Width = width;
                Popup.Visibility = Visibility.Visible;
                (Popup.PopupElement as AppSprite).CloseBtn.Visibility = Visibility.Visible;
                MoveBottomEdgeOfWindowToMousePosition(Popup,width);
                return Popup;
            }
            var window = new PopupWindow
            {
                PopupElement = content as FrameworkElement,
                ResizeMode = ResizeMode.CanResize,
                HorizontalAlignment= HorizontalAlignment.Stretch,
            };
            window.Loaded += (s, e) =>
            {
                window.Width = width;
                window.PopupElement.Width = width;
                window.Visibility = Visibility.Visible;
                (window.PopupElement as AppSprite).CloseBtn.Visibility = Visibility.Visible;
                if (window.Visibility==Visibility.Visible)
                    MoveBottomEdgeOfWindowToMousePosition(window, width);
            };
            window.ShowActivated = false;
            window.Topmost = true;
            window.Show(content as FrameworkElement, false);
            Popup = window;
            return window;
        }
    }
}
