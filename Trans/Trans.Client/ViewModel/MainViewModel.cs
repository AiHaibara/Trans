using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using HandyControl.Controls;
using Trans.Client.Windows;
using Trans.Client.Helper;
using HandyControl.Tools;
using HandyControl.Data;
using System.Windows.Input;
using Trans.Client.Models;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Trans.Client.Data;
using Microsoft.Win32;
using Trans.Client.Tools.Helper;
using System.Windows.Interop;

namespace Trans.Client.ViewModel
{
    public class DemoViewModelBase<T> : ViewModelBase
    {
        /// <summary>
        ///     数据列表
        /// </summary>
        private IEnumerable<LangType> _dataList = Enum.GetValues(typeof(LangType)).Cast<LangType>();

        /// <summary>
        ///     数据列表
        /// </summary>
        public IEnumerable<LangType> DataList
        {
            get => _dataList;
#if netle40
            set => Set(nameof(DataList), ref _dataList, value);
#else
            set => Set(ref _dataList, value);
#endif       
        }

        private IEnumerable<TransStrategy> _useList = Enum.GetValues(typeof(TransStrategy)).Cast<TransStrategy>();

        public IEnumerable<TransStrategy> UseList
        {
            get => _useList;
#if netle40
            set => Set(nameof(UseList), ref _useList, value);
#else   
            set => Set(ref _useList, value);
#endif       
        }
    }
    public class MainViewModel: DemoViewModelBase<DemoDataModel>
    {
        private string _keyText = "Control + E";
        public string KeyText
        {
            get
            {
                return _keyText;
            }
            set {
                _keyText=ModifierKeys != ModifierKeys.None ? $"{ModifierKeys.ToString()} + {Key.ToString()}" : Key.ToString();
                Set(ref _keyText, value); 
            }
        }

        private Key _key = Key.E;
        public Key Key
        {
            get => _key;
#if netle40
            set => Set(nameof(Key), ref _key, value);
#else 
            set => Set(ref _key, value);
#endif
        }

        private ModifierKeys _modifierKeys = ModifierKeys.Control;
        public ModifierKeys ModifierKeys
        {
            get => _modifierKeys;
#if netle40
            set => Set(nameof(ModifierKeys), ref _modifierKeys, value);
#else 
            set => Set(ref _modifierKeys, value);
#endif
        }

        public LangType _to;
        public LangType To
        {
            get => _to;
            set { 
                Set(ref _to, value);
                var strategyTo = GlobalData.Config.Langs?.FirstOrDefault(p => p.name == TransContext.Strategy.ToString() && p.type == To)?.trans;
                if (string.IsNullOrWhiteSpace(strategyTo))
                    strategyTo = LangType.en.ToString();
                TransContext.GetTrans().GetTranslator().setTo(strategyTo);
                GlobalData.Config.TransConfig.To = To;
                GlobalData.Save();
                if (!InitSetting)
                {
                    Growl.SuccessGlobal($"Target Lang is {To}");
                }
            }
        }

        public TransStrategy _use;
        public TransStrategy Use
        {
            get => _use;
            set
            {
                Set(ref _use, value);
                //Trans.GetTranslator().setTo(To);
                TransContext.Strategy = Use;
                GlobalData.Config.TransConfig.Use = Use;
                var strategyTo=GlobalData.Config.Langs?.FirstOrDefault(p => p.name == TransContext.Strategy.ToString() && p.type == To)?.trans;
                if (string.IsNullOrWhiteSpace(strategyTo))
                    strategyTo = LangType.en.ToString();
                TransContext.GetTrans().GetTranslator().setTo(strategyTo);
                GlobalData.Save();
                if (!InitSetting)
                {
                    Growl.SuccessGlobal($"Target Use is {Use}");
                }
            }
        }

        private bool _isAutoStartup = false;
        public bool IsAutoStartup
        {
            get => _isAutoStartup;
#if netle40
            set => Set(nameof(IsAutoStartup), ref _isAutoStartup, value);
#else 
            set
            {
                // The path to the key where Windows looks for startup applications
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value)
                {
                    // Add the value in the registry so that the application runs at startup
                    rkApp.SetValue("Trans.Client", System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName+" --autorun");
                }
                else
                {
                    rkApp.DeleteValue("Trans.Client", false);
                }
                GlobalData.Config.IsAutoStartUp = value;
                GlobalData.Save();
                Set(ref _isAutoStartup, value);
                if (!InitSetting)
                {
                    if (value)
                        Growl.SuccessGlobal("Auto Startup is On");
                    else
                        Growl.SuccessGlobal("Auto Startup is Off");
                }
            }
#endif
        }

        private bool _isNearMouse = false;
        public bool IsNearMouse
        {
            get => _isNearMouse;
#if netle40
            set => Set(nameof(IsNearMouse), ref _isNearMouse, value);
#else 
            set
            {
                GlobalData.Config.IsNearMouse = value;
                GlobalData.Save();
                Set(ref _isNearMouse, value);
                if (!InitSetting)
                {
                    if (value)
                        Growl.SuccessGlobal("Near Mouse is On");
                    else
                        Growl.SuccessGlobal("Near Mouse is Off");
                }
            }
#endif
        }

        private bool _isOpenSearch = false;
        public bool IsOpenSearch
        {
            get => _isOpenSearch;
#if netle40
            set => Set(nameof(IsOpenSearch), ref _isOpenSearch, value);
#else 
            set
            {
                GlobalData.Config.IsOpenSearch = value;
                GlobalData.Save();
                Set(ref _isOpenSearch, value);
                if (!InitSetting)
                {
                    if (value)
                        Growl.SuccessGlobal("Search is On");
                    else
                        Growl.SuccessGlobal("Search is Off");
                }
            }
#endif
        }

        public ITransContext TransContext { get; set; }
        public bool InitSetting { get; set; }
        public MainViewModel(ITransContext transContext)
        {
            TransContext = transContext;
            InitSetting = true;
            To = GlobalData.Config.TransConfig.To;
            Use = GlobalData.Config.TransConfig.Use;
            IsAutoStartup = GlobalData.Config.IsAutoStartUp;
            IsNearMouse = GlobalData.Config.IsNearMouse;
            IsOpenSearch = GlobalData.Config.IsOpenSearch;
            InitSetting = false;
        }

        public RelayCommand MouseCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(async() => await Trans())).Value;

        public RelayCommand GlobalShortcutInfoCmd => new Lazy<RelayCommand>(() =>
          new RelayCommand(() => Environment.Exit(0))).Value;

        public RelayCommand GlobalShortcutWarningCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(async () => await Trans())).Value;

        public RelayCommand GlobalShortcutSwitchCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(async() =>await SwitchToLang())).Value;

        public RelayCommand NormalWindowMouseCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(async () => await NormalWindow())).Value;

        public RelayCommand GlobalShortcutSearchCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(async () => await SwitchOpenSearch())).Value;

        //public RelayCommand OpenCmd => new Lazy<RelayCommand>(() =>
        //    new RelayCommand(() => Sprite.Show(new AppSprite()))).Value;

        public static bool isActive = false;

        public async Task NormalWindow()
        {
            MainWindow.Instance.WindowState = WindowState.Normal;
            await Task.CompletedTask;
        }

        public async Task SwitchToLang()
        {
            var index=Enum.GetValues(typeof(LangType)).Cast<LangType>().ToList().FindIndex(p=>p==To);
            int cnt=Enum.GetValues(typeof(LangType)).Cast<LangType>().ToList().Count();
            index = (index + 1) % cnt;
            To = Enum.GetValues(typeof(LangType)).Cast<LangType>().ToList()[index];
            await Task.CompletedTask;
        }

        public async Task SwitchOpenSearch()
        {
            IsOpenSearch = !IsOpenSearch;
            await Task.CompletedTask;
        }

        public static PopupWindow Popup { get; set; }
        public async Task Trans()
        {
            if (isActive == true) return;
            isActive = true;
            //GlobalData.DpiScale = System.Windows.Media.VisualTreeHelper.GetDpi(MainWindow.Instance);
            //MainWindow.Cursor = Cliper.Instance;
            CropWindow.Do = async (width) =>
            {
                try
                {
                    var trans=TransContext.GetTrans();
                    string dest = null;
                    if (trans is CustomTrans) {
                        //timer.Stop();
                        //var ret = timer.ElapsedMilliseconds;
                        dest = await trans.GetTranslator().Translate();
                    }
                    else
                    {
                        var src = trans.GetOcror().CropImage();
                        //timer.Stop();
                        //var ret = timer.ElapsedMilliseconds;
                        dest = await trans.GetTranslator().Translate(src);
                    }
                    if (IsOpenSearch)
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                        process.StartInfo.Arguments = "google.com/search?q=" + System.Web.HttpUtility.UrlEncode(dest) + " --new-window";
                        process.Start();
                    }
                    if (IsNearMouse)
                    {
                        POINT pt;
                        InteropMethods.GetCursorPos(out pt);
                        //if (Windows.Sprite.Popup != null)
                        //{
                        //    Windows.Sprite.Popup.Hide();
                        //    //Popup.Close();
                        //}
                        var box = new AppSprite(dest, width);
                        Windows.Sprite.Popup = Windows.Sprite.Show(box, pt, width);

                        var handle = new WindowInteropHelper(Windows.Sprite.Popup).Handle;
                        int exstyle = (int)InteropMethods.GetWindowLong(handle, InteropMethods.GWL_EXSTYLE);
                        InteropMethods.SetWindowLong(handle, InteropMethods.GWL_EXSTYLE, (IntPtr)(exstyle | ((int)InteropMethods.WS_EX_NOACTIVATE | ((int)InteropMethods.WS_EX_TOOLWINDOW))));
                        //InteropMethods.SetForegroundWindow(current);
                        //(box.DataContext as AppSpriteViewModel).Dest = dest;
                    }
                    else
                    {
                        await Grow(dest);
                    }
                    Clipboard.SetText(dest);
                }
                catch (Exception ex)
                {
                    var x = ex;
                }
                finally
                {
                    isActive = false;
                }
                //System.Diagnostics.Process.Start("microsoft-edge:http://www.google.com");
                //System.Diagnostics.Process.Start("chrome", "http://www.google.com");
            };
            CropWindow.Show();
            //var timer = new System.Diagnostics.Stopwatch();
        }

        public async Task Grow(string dest)
        {
            Growl.InfoGlobal(dest);
            await Task.CompletedTask;
        }
    }
}
