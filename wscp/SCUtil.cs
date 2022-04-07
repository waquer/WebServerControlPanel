using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace wscp
{
    internal class SCUtil
    {
        private string scname;

        private readonly ServiceController scInst;

        private readonly Label lblName;
        private readonly Label lblStatus;
        private readonly Button btnAction;

        private readonly TextBox txtNotify;

        private readonly Brush RedBrush = new SolidColorBrush(Colors.Red);
        private readonly Brush GreenBrush = new SolidColorBrush(Colors.Green);
        private readonly Brush GrayBrush = new SolidColorBrush(Colors.Gray);


        public SCUtil(string scname, int index, Grid mainGrid, TextBox txtNotify)
        {
            int top = 20 + 40 * index;

            this.scname = scname;

            this.txtNotify = txtNotify;

            this.scInst = new ServiceController(scname);

            this.lblName = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Content = scname,
                Height = 30,
                Margin = new Thickness() { Left = 30, Top = top, Right = 0, Bottom = 0 }
            };
            mainGrid.Children.Add(this.lblName);

            this.lblStatus = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Content = "Checking",
                Height = 30,
                Margin = new Thickness() { Left = 140, Top = top, Right = 0, Bottom = 0 }
            };
            mainGrid.Children.Add(this.lblStatus);

            this.btnAction = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Content = "",
                Height = 30,
                Width = 75,
                Margin = new Thickness() { Left = 0, Top = top, Right = 20, Bottom = 0 },
            };
            this.btnAction.Click += this.BtnClick;
            mainGrid.Children.Add(this.btnAction);

            this.SetStatus(this.CheckRunning());

        }

        private void SetStatus(int status)
        {
            if (status < 0)
            {
                lblStatus.Foreground = GrayBrush;
                lblStatus.Content = "Disabled";
                btnAction.Content = "Disabled";
                btnAction.Opacity = 0.5;
                btnAction.IsEnabled = false;
            }
            else
            {
                if (status > 0)
                {
                    lblStatus.Foreground = GreenBrush;
                    lblStatus.Content = "Running";
                    btnAction.Content = "Stop";
                }
                else
                {
                    lblStatus.Foreground = RedBrush;
                    lblStatus.Content = "Stopped";
                    btnAction.Content = "Start";
                }
                btnAction.Opacity = 1;
                btnAction.IsEnabled = true;
            }
        }

        private int CheckRunning()
        {
            try
            {
                return this.scInst.Status != ServiceControllerStatus.Stopped ? 1 : 0;
            }
            catch (Exception e)
            {
                this.AddLog(e.Message);
                return -1;
            }
        }

        private void StartService()
        {
            try
            {
                if (this.scInst.Status == ServiceControllerStatus.Running)
                {
                    return;
                }
                this.AddLog("Starting " + this.scname + "...");
                new Thread(() =>
                {
                    this.scInst.Start();
                    this.scInst.WaitForStatus(ServiceControllerStatus.Running);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.SetStatus(1);
                        this.AddLog(this.scname + " is Running");
                    }));
                }).Start();
            }
            catch (Exception e)
            {
                this.AddLog("ERROR：" + e.Message);
            }
        }

        private void StopService()
        {
            try
            {
                if (this.scInst.Status == ServiceControllerStatus.Stopped)
                {
                    return;
                }

                this.AddLog("Stopping " + this.scname + "...");
                new Thread(() =>
                {
                    this.scInst.Stop();
                    this.scInst.WaitForStatus(ServiceControllerStatus.Stopped);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.SetStatus(0);
                        this.AddLog(this.scname + " is Stopped");
                    }));
                }).Start();
            }
            catch (Exception e)
            {
                this.AddLog("ERROR：" + e.Message);
            }
        }
        private void BtnClick(object sender, EventArgs e)
        {
            btnAction.IsEnabled = false;
            btnAction.Opacity = 0.5;
            int status = this.CheckRunning();
            if (status > 0)
            {
                this.StopService();
            }
            else if (status == 0)
            {
                this.StartService();
            }
        }

        private void AddLog(string log)
        {
            txtNotify.AppendText(log + Environment.NewLine);
        }

    }
}
