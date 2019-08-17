using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordManager.View
{
    /// <summary>
    /// Interaction logic for DatabaseSelectionView.xaml
    /// </summary>
    public partial class DatabaseSelectionView : UserControl
    {
        MainWindow window;

        public DatabaseSelectionView()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetWindow();
            window.Border_MouseDown(sender, e);
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            GetWindow();
            window.MinimizeWindow(sender, e);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            GetWindow();
            window.CloseWindow(sender, e);
        }

        private void GetWindow()
        {
            if (window is null) window = Window.GetWindow(this) as MainWindow;
        }
    }
}
