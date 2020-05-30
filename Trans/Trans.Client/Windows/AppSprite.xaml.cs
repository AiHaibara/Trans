using System.Windows;
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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            e.Handled = true;
            
            Window.GetWindow(this).Close();

        }
    }
}
