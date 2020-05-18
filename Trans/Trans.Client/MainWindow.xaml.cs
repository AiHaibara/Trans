using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trans.Client.Data;
using Trans.Client.Helper;
using Trans.Client.ViewModel;

namespace Trans.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public MainWindow()
        {
            GlobalData.Init();
            Instance = this;
            WindowState = WindowState.Normal;
            InitializeComponent();
            ContentRendered += MainWindow_ContentRendered;
            if (GlobalData.Config.TransConfig.HotKey != null)
            {
                var text = GlobalData.Config.TransConfig.HotKey;
                if (text.Split('+').Length <= 1)
                {
                    Hot.Key = (Key)Key.Parse(typeof(Key), text);
                    var keyStr = Hot.Modifiers != ModifierKeys.None ? $"{Hot.Modifiers.ToString()} + {Hot.Key.ToString()}" : Hot.Key.ToString();
                    (DataContext as MainViewModel).KeyText = keyStr;                       
                    GlobalShortcut.Init(this);
                }
                else
                {
                    Hot.Modifiers = (ModifierKeys)ModifierKeys.Parse(typeof(ModifierKeys), text.Split('+')[0].Trim());
                    Hot.Key = (Key)Key.Parse(typeof(Key), text.Split('+')[1].Trim());
                    var keyStr = Hot.Modifiers != ModifierKeys.None ? $"{Hot.Modifiers.ToString()} + {Hot.Key.ToString()}" : Hot.Key.ToString();
                    (DataContext as MainViewModel).KeyText = keyStr;
                    GlobalShortcut.Init(this);
                }
            }

        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            if (GlobalData.IsAutoRun)
            {
                Hide();
            }
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            GlobalData.DpiScale = VisualTreeHelper.GetDpi(image);
            GlobalData.ScreenWidth = SystemParameters.PrimaryScreenWidth * GlobalData.DpiScale.DpiScaleX;
            GlobalData.ScreenHeight = SystemParameters.PrimaryScreenHeight* GlobalData.DpiScale.DpiScaleY;
            image.DpiChanged += MainWindow_DpiChanged;
        }

        private void MainWindow_DpiChanged(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            GlobalData.DpiScale = VisualTreeHelper.GetDpi(image);
        }

        public static MainWindow Instance { get; set; }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!GlobalData.Config.NotifyIconNotified)
            {
                GlobalData.Config.NotifyIconNotified = true;
                GlobalData.Save();
                HandyControl.Controls.MessageBox.Info(Properties.Langs.Lang.AppClosingTip, Properties.Langs.Lang.Tip);
                Hide();
                e.Cancel = true;
            }
            else
            {
                Hide();
                e.Cancel = true;
                //base.OnClosing(e);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            if (e.Key == Key.System)
                key = e.SystemKey;
            HitTest(key);
            if (IsModifierKey(key)) return;
            (sender as UIElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
        private static bool IsModifierKey(Key key)
        {
            return key == Key.LeftCtrl || key == Key.LeftAlt || key == Key.LeftShift || key == Key.LWin ||
                   key == Key.RightCtrl || key == Key.RightAlt || key == Key.RightShift || key == Key.RWin || key == Key.System;
        }
        private void HitTest(Key key)
        {
            if (IsModifierKey(key)) return;

            var modifierKeys = Keyboard.Modifiers;
            var keyStr = modifierKeys != ModifierKeys.None ? $"{modifierKeys.ToString()} + {key.ToString()}" : key.ToString();
            Hot.Modifiers = Keyboard.Modifiers;
            Hot.Key = key;
            (DataContext as MainViewModel).KeyText = keyStr;
            //(DataContext as MainViewModel).ModifierKeys = modifierKeys;
            //(DataContext as MainViewModel).Key = key;
            GlobalShortcut.Init(this);
            GlobalData.Config.TransConfig.HotKey = (DataContext as MainViewModel).KeyText;
            GlobalData.Save();
            Growl.SuccessGlobal($"HotKey is {keyStr}");
        }
        //Mouse.OverrideCursor = DisplayArea.Cursor;
    }
}
