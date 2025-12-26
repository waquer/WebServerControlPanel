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

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
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
                    IsEnabled = false;
                    _addLog("Starting " + DisplayName + " ...");
                    _scInst.Start();
                    _scInst.WaitForStatus(ServiceControllerStatus.Running);
                    _addLog(DisplayName + " is Running");
                    OnPropertyChanged(nameof(StatusName));
                    OnPropertyChanged(nameof(ActionName));
                    IsEnabled = true;
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
                    IsEnabled = false;
                    _addLog("Stopping " + DisplayName + " ...");
                    _scInst.Stop();
                    _scInst.WaitForStatus(ServiceControllerStatus.Stopped);
                    _addLog(DisplayName + " is Stopped");
                    OnPropertyChanged(nameof(StatusName));
                    OnPropertyChanged(nameof(ActionName));
                    IsEnabled = true;
                });
            }
            catch (Exception e)
            {
                _addLog("ERROR：" + e.Message);
            }
        }
    }
}