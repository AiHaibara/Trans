using Trans.Client.ViewModel;

namespace Trans.Client.Windows
{
    public partial class AppSprite
    {
        public AppSprite(string dest)
        {
            InitializeComponent();
            (DataContext as AppSpriteViewModel).Dest = dest;
        }
    }
}
