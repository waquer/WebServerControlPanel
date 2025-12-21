using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WebServerControlPanel.Utils
{
    internal class ScItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly string _name;

        private readonly ServiceController _scInst;

        private readonly TextBox _notifyText;

        public ScItem(string scname, TextBox notifyText)
        {
            this._name = scname;
            this._scInst = new ServiceController(scname);
            this._notifyText = notifyText;
        }

        public string ServiceName
        {
            get
            {
                try
                {
                    return _scInst.ServiceName;
                }
                catch (Exception)
                {
                    return _name;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                try
                {
                    return _scInst.DisplayName;
                }
                catch (Exception)
                {
                    return _name;
                }
            }
        }

        public string StatusName
        {
            get
            {
                try
                {
                    return _scInst.Status.ToString();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        public string ActionName
        {
            get
            {
                try
                {
                    switch (_scInst.Status)
                    {
                        case ServiceControllerStatus.Running:
                            return "Stop";
                        case ServiceControllerStatus.Stopped:
                            return "Start";
                        default:
                            return "Waiting";
                    }
                }
                catch (Exception)
                {
                    return "Invalid";
                }
            }
        }

        public void Start()
        {
            try
            {
                if (_scInst.Status == ServiceControllerStatus.Running)
                {
                    return;
                }

                AddLog("Starting " + DisplayName + " ...");
                new Thread(() =>
                {
                    _scInst.Start();
                    _scInst.WaitForStatus(ServiceControllerStatus.Running);
                    AddLog(DisplayName + " is Running");
                }).Start();
            }
            catch (Exception e)
            {
                AddLog("ERROR：" + e.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (_scInst.Status == ServiceControllerStatus.Stopped)
                {
                    return;
                }

                AddLog("Stopping " + DisplayName + " ...");
                new Thread(() =>
                {
                    _scInst.Stop();
                    _scInst.WaitForStatus(ServiceControllerStatus.Stopped);
                    AddLog(DisplayName + " is Stopped");
                }).Start();
            }
            catch (Exception e)
            {
                AddLog("ERROR：" + e.Message);
            }
        }

        private void AddLog(string log)
        {
            Dispatcher.CurrentDispatcher.Invoke(delegate { _notifyText.AppendText(log + Environment.NewLine); });
        }
    }
}