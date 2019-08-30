using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordManager.View
{
    /// <summary>
    /// Interaction logic for SyncOpeningView.xaml
    /// </summary>
    public partial class SyncOpeningView : UserControl
    {
        MainWindow window;

        public SyncOpeningView()
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
            window.CloseWindow(sender, e);
        }

        private void GetWindow()
        {
            if (window is null) window = Window.GetWindow(this) as MainWindow;
        }
    }
}
