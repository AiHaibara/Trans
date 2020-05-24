using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Trans.Client.Tools;

namespace Trans.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#pragma warning disable IDE0052
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private static Mutex AppMutex;
#pragma warning restore IDE0052
        protected override void OnStartup(StartupEventArgs e)
        {
            var isRestart = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "--autorun")
                {
                    Data.GlobalData.IsAutoRun = true;
                }
                if (e.Args[i] == "--restart")
                {
                    isRestart = true;
                }
            }

            AppMutex = new Mutex(true, "Trans", out var createdNew);

            if (!isRestart && !createdNew)
            {
                var current = Process.GetCurrentProcess();

                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        Win32Helper.SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
                Shutdown();
            }
            else
            {

                if (!Data.GlobalData.IsAutoRun)
                {
                    var splashScreen = new SplashScreen("Resources/Img/icon.png");
                    splashScreen.Show(true);
                }
                base.OnStartup(e);
                //UpdateRegistry();

                //ShutdownMode = ShutdownMode.OnMainWindowClose;
                //GlobalData.Init();
                //ConfigHelper.Instance.SetLang(GlobalData.Config.Lang);

                //if (GlobalData.Config.Skin != SkinType.Default)
                //{ 
                //    UpdateSkin(GlobalData.Config.Skin);
                //}

                //ConfigHelper.Instance.SetSystemVersionInfo(CommonHelper.GetSystemVersionInfo());

                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)(SslProtocols)0x00000C00;
            }
        }
    }
}
