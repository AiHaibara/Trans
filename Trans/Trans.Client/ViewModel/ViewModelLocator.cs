using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Trans.Client.Data;

namespace Trans.Client.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //SimpleIoc.Default.Register<DataService>();
            //var dataService = ServiceLocator.Current.GetInstance<DataService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register(() => new GrowlDemoViewModel(), "GrowlDemo");
            SimpleIoc.Default.Register(() => new GrowlDemoViewModel(MessageToken.GrowlDemoPanel), "GrowlDemoWithToken");
        }
        public static ViewModelLocator Instance => new Lazy<ViewModelLocator>(() =>
           Application.Current.TryFindResource("Locator") as ViewModelLocator).Value;
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

    }
}
