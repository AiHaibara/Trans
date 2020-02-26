using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using HandyControl.Controls;
using Trans.Client.Windows;
using Trans.Client.Helper;
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
        public RelayCommand GlobalShortcutInfoCmd => new Lazy<RelayCommand>(() =>
          new RelayCommand(() => Growl.Warning("Global Shortcut Warning"))).Value;

        public RelayCommand GlobalShortcutWarningCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(() => Grow())).Value;

        public void Grow()
        {
            //MainWindow.Cursor = Cliper.Instance;
            CropWindow.ShowDialog();
            var src = BaiduHelper.Ocror.generalBasic();
            var dest = BaiduHelper.Translator.generalBasic(src);
            Growl.InfoGlobal(dest);
        }
       
    }
}
