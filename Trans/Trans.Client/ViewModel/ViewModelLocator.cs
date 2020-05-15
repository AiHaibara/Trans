using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Trans.Client.Data;
using Trans.Client.Helper;

namespace Trans.Client.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //SimpleIoc.Default.Register<DataService>();
            //var dataService = ServiceLocator.Current.GetInstance<DataService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AppSpriteViewModel>();
            //SimpleIoc.Default.Register(() => new GrowlDemoViewModel(), "GrowlDemo");
            //SimpleIoc.Default.Register(() => new GrowlDemoViewModel(MessageToken.GrowlDemoPanel), "GrowlDemoWithToken");
            //SimpleIoc.Default.Register<IOcror, CustomTrans.Ocror>();
            //SimpleIoc.Default.Register<ITranslator, CustomTrans.Translator>();
            SimpleIoc.Default.Register<ITrans, BaiduTrans>();
            SimpleIoc.Default.Register<ITransContext,TransContext>();
        }
        public static ViewModelLocator Instance => new Lazy<ViewModelLocator>(() =>
           Application.Current.TryFindResource("Locator") as ViewModelLocator).Value;
        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public MainViewModel Box => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}
