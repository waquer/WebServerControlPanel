using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Windows;

namespace WebServerControlPanel.Utils
{
    internal class ScItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly string _name;

        private readonly ServiceController _scInst;

        private readonly Action<string> _addLog;

        public ScItem(string scname, Action<string> addLog)
        {
            _name = scname;
            _scInst = new ServiceController(scname);
            _addLog = addLog;
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

        public bool IsRunning
        {
            get
            {
                try
                {
                    return _scInst.Status == ServiceControllerStatus.Running;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsStopped
        {
            get
            {
                try
                {
                    return _scInst.Status == ServiceControllerStatus.Stopped;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                try
                {
                    return _scInst.Status != ServiceControllerStatus.ContinuePending
                           && _scInst.Status != ServiceControllerStatus.PausePending
                           && _scInst.Status != ServiceControllerStatus.StartPending
                           && _scInst.Status != ServiceControllerStatus.StopPending;
                }
                catch (Exception)
                {
                    return true;
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(IsEnabled));
                    _addLog("Starting " + DisplayName + " ...");
                    _scInst.Start();
                    _scInst.WaitForStatus(ServiceControllerStatus.Running);
                    _addLog(DisplayName + " is Running");
                    OnPropertyChanged(nameof(IsEnabled));
                    OnPropertyChanged(nameof(StatusName));
                    OnPropertyChanged(nameof(ActionName));
                });
            }
            catch (Exception e)
            {
                _addLog("ERROR：" + e.Message);
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(IsEnabled));
                    _addLog("Stopping " + DisplayName + " ...");
                    _scInst.Stop();
                    _scInst.WaitForStatus(ServiceControllerStatus.Stopped);
                    _addLog(DisplayName + " is Stopped");
                    OnPropertyChanged(nameof(IsEnabled));
                    OnPropertyChanged(nameof(StatusName));
                    OnPropertyChanged(nameof(ActionName));
                });
            }
            catch (Exception e)
            {
                _addLog("ERROR：" + e.Message);
            }
        }
    }
}