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
                Trans.GetTranslator().setTo(To);
                GlobalData.Config.TransConfig.To = To;
                GlobalData.Save();
            }
        }
        public ITrans Trans { get; set; }
        public MainViewModel(ITrans trans)
        {
            Trans = trans;
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
            To = GlobalData.Config.TransConfig.To;
        }
        public RelayCommand GlobalShortcutInfoCmd => new Lazy<RelayCommand>(() =>
          new RelayCommand(() => Growl.Warning("Global Shortcut Warning"))).Value;

        public RelayCommand GlobalShortcutWarningCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => Grow())).Value;

        public void Grow()
        {
            //MainWindow.Cursor = Cliper.Instance;
            CropWindow.ShowDialog();
            var src = Trans.GetOcror().CropImage();
            var dest = Trans.GetTranslator().Translate(src);
            Growl.InfoGlobal(dest);
        }
    }
}
