using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trans.Client.ViewModel
{
    public class AppSpriteViewModel:ViewModelBase
    {
        private string _dest = "None";
        public string Dest
        {
            get => _dest;
#if netle40
            set => Set(nameof(Dest), ref _dest, value);
#else 
            set
            {
                Set(ref _dest, value);
            }
#endif
        }

        public AppSpriteViewModel()
        {

        }
    }
}
