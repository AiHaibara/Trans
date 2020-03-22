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
#if netle40
using GalaSoft.MvvmLight.Command;
#else
using GalaSoft.MvvmLight.CommandWpf;
#endif
using Trans.Client.Data;

namespace Trans.Client.ViewModel
{
    public class DemoViewModelBase<T> : ViewModelBase
    {
        /// <summary>
        ///     数据列表
        /// </summary>
        private IList<T> _dataList;

        /// <summary>
        ///     数据列表
        /// </summary>
        public IList<T> DataList
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

        public string _to;
        public string To
        {
            get => _to;
            set { 
                Set(ref _to, value);
                TransContext.GetTrans().GetTranslator().setTo(To);
                GlobalData.Config.TransConfig.To = To;
                GlobalData.Save();
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
                TransContext.GetTrans().GetTranslator().setTo(To);
                GlobalData.Save();
            }
        }
        public ITransContext TransContext { get; set; }
        public MainViewModel(ITransContext transContext)
        {
            TransContext = transContext;
            DataList = new List<DemoDataModel>()
            {
                new DemoDataModel()
                {
                    From="en",
                    To="zh",
                    FromFull="English",
                    ToFull="简体中文"
                },
                new DemoDataModel()
                {
                    From="zh",
                    To="en",
                    FromFull="简体中文",
                    ToFull="English"
                }
            };
            Use = GlobalData.Config.TransConfig.Use;
            To = GlobalData.Config.TransConfig.To;
        }
        public RelayCommand GlobalShortcutInfoCmd => new Lazy<RelayCommand>(() =>
          new RelayCommand(() => Environment.Exit(0))).Value;

        public RelayCommand GlobalShortcutWarningCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => Grow())).Value;

        public void Grow()
        {
            //MainWindow.Cursor = Cliper.Instance;
            CropWindow.ShowDialog();
            //var timer = new System.Diagnostics.Stopwatch();

            //timer.Start();
            try
            {
                var src = TransContext.GetTrans().GetOcror().CropImage();
                //timer.Stop();
                //var ret = timer.ElapsedMilliseconds;
                var dest = TransContext.GetTrans().GetTranslator().Translate(src);
                Growl.InfoGlobal(dest);
            }
            catch { }
            finally
            {
            }
        }
    }
}
