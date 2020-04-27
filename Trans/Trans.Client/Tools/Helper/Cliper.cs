using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Trans.Client.Tools
{
    public class Cliper
    {
        public static Cursor Instance{get;set;}

        static Cliper()
        {
            //if (isShow)
            {
                if (Instance == null)
                {
                    var info = Application.GetResourceStream(new Uri("pack://application:,,,/Trans.Client;Component/Resources/cursor1.cur"));
                    if (info != null)
                    {
                        Instance = new Cursor(info.Stream);
                    }
                }

                if (Instance == null) return;
            }
            //    MouseHook.Start();
            //    MouseHook.StatusChanged += MouseHook_StatusChanged;
            //}
            //else
            //{
            //    Mouse.OverrideCursor = Cursors.Arrow;
            //    MouseHook.Stop();
            //    MouseHook.StatusChanged -= MouseHook_StatusChanged;
            //}
        }
    }
}
