using System.Windows;
using Trans.Client.ViewModel;

namespace Trans.Client.Windows
{
    public partial class AppSprite
    {
        public AppSprite(string dest,double width)
        {
            InitializeComponent();
            (DataContext as AppSpriteViewModel).Dest = dest;
            Width = width+10;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            e.Handled = true;
            Window.GetWindow(this).Visibility = Visibility.Hidden;
            //Window.GetWindow(this).Close();

        }
    }
}
