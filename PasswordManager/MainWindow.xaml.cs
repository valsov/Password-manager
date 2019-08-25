using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using PasswordManager.Extensions;
using System.Windows.Shell;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Forms.NotifyIcon trayIcon;

        public MainWindow()
        {
            InitializeComponent();

            trayIcon = new Forms.NotifyIcon()
            {
                Visible = true,
                Text = "Password manager",
                ContextMenu = new Forms.ContextMenu(new Forms.MenuItem[]
                {
                    new Forms.MenuItem("Show", (x,y) => this.Show()),
                    new Forms.MenuItem("Exit", (x,y) => this.Close())
                })
            };
            trayIcon.Click += delegate
            {
                if (!this.IsVisible)
                {
                    this.Show();
                }
            };

            using (var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream)
            {
                trayIcon.Icon = new Icon(iconStream);
            }
            TaskbarItemInfo = new TaskbarItemInfo()
            {
                ProgressState = TaskbarItemProgressState.Normal
            };

            PasswordEntryModelExtension.CopyDataStart += CopyDataStartEventHandler;
            PasswordEntryModelExtension.CopyDataEnd += CopyDataEndEventHandler;
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

        private void CopyDataStartEventHandler(object sender, EventArgs e)
        {
            DataCopyDecayProgressBar.Value = 100;
            DataCopyDecayProgressBar.Visibility = Visibility.Visible;

            // Stop eventual previous animation
            DataCopyDecayProgressBar.BeginAnimation(ProgressBar.ValueProperty, null);

            // Start new animation
            var doubleAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(7)));
            DataCopyDecayProgressBar.BeginAnimation(ProgressBar.ValueProperty, doubleAnimation);
        }

        private void CopyDataEndEventHandler(object sender, EventArgs e)
        {
            DataCopyDecayProgressBar.Visibility = Visibility.Collapsed;
            DataCopyDecayProgressBar.Value = 100;
        }

        private void DataCopyDecayProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TaskbarItemInfo.ProgressValue = e.NewValue / 100;
        }
    }
}
