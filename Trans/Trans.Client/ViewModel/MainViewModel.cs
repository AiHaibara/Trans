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

        public ITransContext TransContext { get; set; }
        public bool InitSetting { get; set; }
        public MainViewModel(ITransContext transContext)
        {
            TransContext = transContext;
            InitSetting = true;
            To = GlobalData.Config.TransConfig.To;
            Use = GlobalData.Config.TransConfig.Use;
            IsAutoStartup = GlobalData.Config.IsAutoStartUp;
            InitSetting = false;
        }

        public RelayCommand MouseCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => Grow())).Value;

        public RelayCommand GlobalShortcutInfoCmd => new Lazy<RelayCommand>(() =>
          new RelayCommand(() => Environment.Exit(0))).Value;

        public RelayCommand GlobalShortcutWarningCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => Grow())).Value;

        public RelayCommand GlobalShortcutSwitchCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => SwitchToLang())).Value;
        public static bool isActive = false;
        public async Task SwitchToLang()
        {
            var index=Enum.GetValues(typeof(LangType)).Cast<LangType>().ToList().FindIndex(p=>p==To);
            int cnt=Enum.GetValues(typeof(LangType)).Cast<LangType>().ToList().Count();
            index = (index + 1) % cnt;
            To = Enum.GetValues(typeof(LangType)).Cast<LangType>().ToList()[index];
        }

        public async Task Grow()
        {
            if (isActive == true) return;
            isActive = true;
            //GlobalData.DpiScale = System.Windows.Media.VisualTreeHelper.GetDpi(MainWindow.Instance);
            //MainWindow.Cursor = Cliper.Instance;
            CropWindow.ShowDialog();                                                                                         
            //var timer = new System.Diagnostics.Stopwatch();

            //timer.Start();
            try
            {
                var src = TransContext.GetTrans().GetOcror().CropImage();
                //timer.Stop();
                //var ret = timer.ElapsedMilliseconds;
                var dest =await TransContext.GetTrans().GetTranslator().Translate(src);
                Growl.InfoGlobal(dest);
                Clipboard.SetText(dest);
            }
            catch (Exception ex){
                var x = ex;
            }
            finally
            {
                isActive = false;
            }
        }
    }
}
