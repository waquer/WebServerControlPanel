using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace wscp {

    public partial class MainWindow : Window {

        System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow() {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.MouseClick += NotifyIcon_Click;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            string[] wss = { "Apache", "Mysql", "Redis" };
            foreach (string ws in wss) {
                ServiceController sc = new ServiceController(ws);
                this.SetStatus(ws, this.CheckRunning(sc));
            }
        }

        private Brush RedBrush = new SolidColorBrush(Colors.Red);
        private Brush GreenBrush = new SolidColorBrush(Colors.Green);

        private void SetStatus(string scname, bool running) {
            scname = scname.ToLower();
            Button btn = MainForm.FindName("btn_" + scname) as Button;
            Label sta = MainForm.FindName("sta_" + scname) as Label;
            if (running) {
                btn.Content = "Stop";
                sta.Content = "Running";
                sta.Foreground = GreenBrush;
            } else {
                btn.Content = "Start";
                sta.Content = "Stopped";
                sta.Foreground = RedBrush;
            }
            btn.IsEnabled = true;
            btn.Opacity = 1;
        }

        public void AddLog(string log) {
            NotifyText.AppendText(log + System.Environment.NewLine);
        }

        private void StartService(ServiceController sc) {
            if (sc == null || sc.Status == ServiceControllerStatus.Running) {
                return;
            }
            try {
                String scname = sc.ServiceName;
                this.AddLog("Starting " + scname + "...");
                new Thread(() => {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        this.SetStatus(scname, true);
                        this.AddLog(scname + " is Running");
                    }));
                }).Start();
            } catch (Exception e) {
                this.AddLog("ERROR：" + e.Message);
            }
        }

        private void StopService(ServiceController sc) {
            if (sc == null || sc.Status == ServiceControllerStatus.Stopped) {
                return;
            }
            try {
                String scname = sc.ServiceName;
                this.AddLog("Stopping " + scname + "...");
                new Thread(() => {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        this.SetStatus(scname, false);
                        this.AddLog(scname + " is Stopped");
                    }));
                }).Start();
            } catch (Exception e) {
                this.AddLog("ERROR：" + e.Message);
            }
        }

        private bool CheckRunning(ServiceController sc) {
            if (sc != null) {
                return sc.Status != ServiceControllerStatus.Stopped;
            }
            return false;
        }

        private void BtnClick(String scname) {
            ServiceController sc = new ServiceController(scname);
            Button btn = MainForm.FindName("btn_" + scname.ToLower()) as Button;
            btn.IsEnabled = false;
            btn.Opacity = 0.5;
            if (this.CheckRunning(sc)) {
                this.StopService(sc);
            } else {
                this.StartService(sc);
            }
        }

        private void Btn_apache_Click(object sender, RoutedEventArgs e) {
            this.BtnClick("Apache");
        }

        private void Btn_mysql_Click(object sender, RoutedEventArgs e) {
            this.BtnClick("Mysql");
        }

        private void Btn_redis_Click(object sender, RoutedEventArgs e) {
            this.BtnClick("Redis");
        }

        private void MainForm_StateChanged(object sender, EventArgs e) {
            if (WindowState == WindowState.Minimized) {
                ShowInTaskbar = false;
                notifyIcon.Visible = true;
            } else {
                ShowInTaskbar = true;
                notifyIcon.Visible = false;
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e) {
            WindowState = WindowState.Normal;
        }

    }

}
