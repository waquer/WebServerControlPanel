using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace wscp
{

    public partial class MainWindow : Window
    {

        private readonly string[] SCNameList = { "Apache", "MySQL", "Redis", "nacos" };

        private readonly Dictionary<String, SCUtil> SCDict = new Dictionary<String, SCUtil>(StringComparer.OrdinalIgnoreCase);

        private readonly System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.MouseClick += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void MainForm_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
            else
            {
                ShowInTaskbar = true;
                notifyIcon.Visible = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (string scname in SCNameList)
            {
                SCUtil scu = new SCUtil(scname, i++, MainGrid, NotifyText);
                this.SCDict.Add(scname, scu);
            }
        }

        private void AddLog(string log)
        {
            NotifyText.AppendText(log + Environment.NewLine);
        }

    }

}
