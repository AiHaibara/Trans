using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            GlobalData.Init();
            var token =BaiduHelper.AccessToken.getAccessToken();
        }

        public static MainWindow Instance { get; set; }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!GlobalData.Config.NotifyIconNotified)
            {
                GlobalData.Config.NotifyIconNotified = true;
                GlobalData.Save();
                MessageBox.Info(Properties.Langs.Lang.AppClosingTip, Properties.Langs.Lang.Tip);
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
        //Mouse.OverrideCursor = DisplayArea.Cursor;
    }
}
