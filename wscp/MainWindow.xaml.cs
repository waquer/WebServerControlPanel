using System;
using System.Windows;
using System.Windows.Forms;

namespace wscp
{

    public partial class MainWindow : Window
    {

        private readonly string[] SCNameList = { "Apache", "MySQL", "Redis", "nacos" };

        private readonly NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.MouseClick += NotifyIcon_Click;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void MainForm_StateChanged(object sender, EventArgs e)
        {
            bool iconShow = WindowState == WindowState.Minimized;
            this.notifyIcon.Visible = iconShow;
            this.ShowInTaskbar = !iconShow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (string scname in SCNameList)
            {
                new SCUtil(scname, i++, MainGrid, NotifyText);
            }
        }

    }

}
