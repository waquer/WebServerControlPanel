using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;

namespace WebServerControlPanel.Utils
{
    internal class ScItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly string name;

        private readonly ServiceController scInst;

        public ScItem(string scname)
        {
            this.name = scname;
            this.scInst = new ServiceController(scname);
        }

        public string ServiceName
        {
            get
            {
                try
                {
                    return scInst.ServiceName;
                }
                catch (Exception)
                {
                    return name;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                try
                {
                    return scInst.DisplayName;
                }
                catch (Exception)
                {
                    return name;
                }
            }
        }

        public string StatusName
        {
            get
            {
                try
                {
                    return scInst.Status.ToString();
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
                    if (scInst.Status == ServiceControllerStatus.Running)
                    {
                        return "Stop";
                    }
                    else if (scInst.Status == ServiceControllerStatus.Stopped)
                    {
                        return "Start";
                    }
                    else
                    {
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
                if (scInst.Status == ServiceControllerStatus.Running)
                {
                    return;
                }
                AddLog("Starting " + DisplayName + " ...");
                new Thread(() =>
                {
                    scInst.Start();
                    scInst.WaitForStatus(ServiceControllerStatus.Running);
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
                if (scInst.Status == ServiceControllerStatus.Stopped)
                {
                    return;
                }
                AddLog("Stopping " + DisplayName + " ...");
                new Thread(() =>
                {
                    scInst.Stop();
                    scInst.WaitForStatus(ServiceControllerStatus.Stopped);
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
            //
        }
    }
}
