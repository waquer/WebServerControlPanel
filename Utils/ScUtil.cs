using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WebServerControlPanel.Utils
{
    internal class ScUtil
    {
        private readonly string _scName;

        private readonly ServiceController _scInst;

        private readonly Label _lblStatus;

        private readonly Button _btnAction;

        private readonly TextBox _txtNotify;

        public string ScName => _scName;

        public ScUtil(string scname, int index, Grid mainGrid, TextBox notifyBox)
        {
            var top = 20 + 40 * index;

            _scName = scname;

            _txtNotify = notifyBox;

            _scInst = new ServiceController(scname);

            var lblName = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Content = scname,
                Height = 30,
                Margin = new Thickness() { Left = 30, Top = top, Right = 0, Bottom = 0 }
            };
            mainGrid.Children.Add(lblName);

            _lblStatus = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Content = "Checking",
                Height = 30,
                Margin = new Thickness() { Left = 140, Top = top, Right = 0, Bottom = 0 }
            };
            mainGrid.Children.Add(_lblStatus);

            _btnAction = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Content = "",
                Height = 30,
                Width = 75,
                Margin = new Thickness() { Left = 0, Top = top, Right = 20, Bottom = 0 },
            };
            _btnAction.Click += BtnClick;
            mainGrid.Children.Add(_btnAction);

            SetStatus(CheckRunning());
        }

        private void SetStatus(int status)
        {
            if (status < 0)
            {
                _lblStatus.Foreground = ColorSet.GrayBrush;
                _lblStatus.Content = "Disabled";
                _btnAction.Content = "Disabled";
                _btnAction.Opacity = 0.5;
                _btnAction.IsEnabled = false;
            }
            else
            {
                if (status > 0)
                {
                    _lblStatus.Foreground = ColorSet.GreenBrush;
                    _lblStatus.Content = "Running";
                    _btnAction.Content = "Stop";
                }
                else
                {
                    _lblStatus.Foreground = ColorSet.RedBrush;
                    _lblStatus.Content = "Stopped";
                    _btnAction.Content = "Start";
                }

                _btnAction.Opacity = 1;
                _btnAction.IsEnabled = true;
            }
        }

        private int CheckRunning()
        {
            try
            {
                return _scInst.Status != ServiceControllerStatus.Stopped ? 1 : 0;
            }
            catch (Exception e)
            {
                AddLog(e.Message);
                return -1;
            }
        }

        private void StartService()
        {
            try
            {
                if (_scInst.Status == ServiceControllerStatus.Running)
                {
                    return;
                }

                AddLog("Starting " + _scName + "...");
                new Thread(() =>
                {
                    _scInst.Start();
                    _scInst.WaitForStatus(ServiceControllerStatus.Running);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SetStatus(1);
                        AddLog(_scName + " is Running");
                    }));
                }).Start();
            }
            catch (Exception e)
            {
                AddLog("ERROR：" + e.Message);
            }
        }

        private void StopService()
        {
            try
            {
                if (_scInst.Status == ServiceControllerStatus.Stopped)
                {
                    return;
                }

                AddLog("Stopping " + _scName + "...");
                new Thread(() =>
                {
                    _scInst.Stop();
                    _scInst.WaitForStatus(ServiceControllerStatus.Stopped);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SetStatus(0);
                        AddLog(_scName + " is Stopped");
                    }));
                }).Start();
            }
            catch (Exception e)
            {
                AddLog("ERROR：" + e.Message);
            }
        }

        private void BtnClick(object sender, EventArgs e)
        {
            _btnAction.IsEnabled = false;
            _btnAction.Opacity = 0.5;
            var status = this.CheckRunning();
            if (status > 0)
            {
                StopService();
            }
            else if (status == 0)
            {
                StartService();
            }
        }

        private void AddLog(string log)
        {
            _txtNotify.AppendText(log + Environment.NewLine);
        }
    }
}