using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;

namespace WebServerControlPanel.Utils
{
    internal class ScItem
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));//全局通知(给监听此属性的控件)
        }

        private readonly int index;

        private readonly string name;

        private readonly ServiceController scInst;

        public ScItem(string scname, int index)
        {
            this.name = scname;
            this.index = index;
            this.scInst = new ServiceController(scname);
        }

        public int ID
        {
            get => index;
        }

        public string Name
        {
            get => name;
        }

        public string Status
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

        public void Start()
        {
            if (scInst.Status == ServiceControllerStatus.Running)
            {
                return;
            }
            new Thread(() =>
            {
                scInst.Start();
                scInst.WaitForStatus(ServiceControllerStatus.Running);
            }).Start();
        }

        public void Stop()
        {
            if (scInst.Status == ServiceControllerStatus.Stopped)
            {
                return;
            }
            new Thread(() =>
            {
                scInst.Stop();
                scInst.WaitForStatus(ServiceControllerStatus.Stopped);
            }).Start();
        }

    }
}
