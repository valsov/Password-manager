using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon trayIcon;

        public MainWindow()
        {
            InitializeComponent();

            trayIcon = new NotifyIcon()
            {
                Visible = true,
                Text = "Password manager",
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Show", (x,y) => this.Show()),
                    new MenuItem("Exit", (x,y) => this.Close())
                })
            };
            trayIcon.Click += delegate
            {
                if (!this.IsVisible)
                {
                    this.Show();
                }
            };

            using (var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream)
            {
                trayIcon.Icon = new Icon(iconStream);
            }
        }

        public void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void CloseWindow(object sender, RoutedEventArgs e)
        {
            trayIcon.Dispose();
            this.Close();
        }
    }
}
