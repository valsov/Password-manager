using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordManager.View
{
    /// <summary>
    /// Interaction logic for DatabaseCreationView.xaml
    /// </summary>
    public partial class DatabaseCreationView : UserControl
    {
        MainWindow window;

        public DatabaseCreationView()
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
            window.WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            GetWindow();
            window.Close();
        }

        private void GetWindow()
        {
            if (window is null) window = Window.GetWindow(this) as MainWindow;
        }
    }
}
