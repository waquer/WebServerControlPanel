using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace wscp {

    public partial class MainWindow : Window {

        private readonly System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

        private readonly Brush RedBrush = new SolidColorBrush(Colors.Red);
        private readonly Brush GreenBrush = new SolidColorBrush(Colors.Green);
        private readonly Brush GrayBrush = new SolidColorBrush(Colors.Gray);

        private string[] SCNameList = { "Apache", "MySQL", "Redis", "Nginx" };
        private Dictionary<String, ServiceController> SCDict = new Dictionary<String, ServiceController>();

        public MainWindow() {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.MouseClick += NotifyIcon_Click;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            foreach (string scname in SCNameList) {
                ServiceController sc = new ServiceController(scname);
                this.SetStatus(scname, this.CheckRunning(sc));
                this.SCDict.Add(scname, sc);
            }
        }

        private void SetStatus(string scname, int status) {
            scname = scname.ToLower();
            Button btn = MainForm.FindName("btn_" + scname) as Button;
            Label sta = MainForm.FindName("sta_" + scname) as Label;
            if (status < 0) {
                sta.Foreground = GrayBrush;
                sta.Content = "Disabled";
                btn.Content = "Disabled";
                btn.Opacity = 0.5;
                btn.IsEnabled = false;
            } else {
                if (status > 0) {
                    sta.Foreground = GreenBrush;
                    sta.Content = "Running";
                    btn.Content = "Stop";
                } else {
                    sta.Foreground = RedBrush;
                    sta.Content = "Stopped";
                    btn.Content = "Start";
                }
                btn.Opacity = 1;
                btn.IsEnabled = true;
            }
        }

        private int CheckRunning(ServiceController sc) {
            try {
                return sc.Status != ServiceControllerStatus.Stopped ? 1 : 0;
            } catch (Exception e) {
                this.AddLog(e.Message);
                return -1;
            }
        }

        private void StartService(ServiceController sc) {
            try {
                if (sc.Status == ServiceControllerStatus.Running) {
                    return;
                }
                String scname = sc.ServiceName;
                this.AddLog("Starting " + scname + "...");
                new Thread(() => {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        this.SetStatus(scname, 1);
                        this.AddLog(scname + " is Running");
                    }));
                }).Start();
            } catch (Exception e) {
                this.AddLog("ERROR：" + e.Message);
            }
        }

        private void StopService(ServiceController sc) {
            try {
                if (sc.Status == ServiceControllerStatus.Stopped) {
                    return;
                }
                String scname = sc.ServiceName;
                this.AddLog("Stopping " + scname + "...");
                new Thread(() => {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        this.SetStatus(scname, 0);
                        this.AddLog(scname + " is Stopped");
                    }));
                }).Start();
            } catch (Exception e) {
                this.AddLog("ERROR：" + e.Message);
            }
        }

        private void AddLog(string log) {
            NotifyText.AppendText(log + System.Environment.NewLine);
        }

        private void BtnClick(String scname) {
            ServiceController sc = this.SCDict[scname];
            Button btn = MainForm.FindName("btn_" + scname.ToLower()) as Button;
            btn.IsEnabled = false;
            btn.Opacity = 0.5;
            int status = this.CheckRunning(sc);
            if (status > 0) {
                this.StopService(sc);
            } else if (status == 0) {
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

        private void Btn_nginx_Click(object sender, RoutedEventArgs e) {
            this.BtnClick("Nginx");
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
