using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

namespace WebServerControlPanel.Utils
{
    internal class ScItem
    {

        private readonly int index;

        private readonly string name;

        private readonly ServiceController scInst;

        public ICommand ActionCommand { get; set; }

        public ScItem(string scname, int index)
        {
            this.index = index;
            this.name = scname;
            this.scInst = new ServiceController(scname);
            ActionCommand = new ScCommand(DoAction);
        }

        private void DoAction(Object obj)
        {
            Debug.WriteLine("dsfdsf");
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

        public string Action
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
                AddLog("Starting " + Name + "...");
                new Thread(() =>
                {
                    scInst.Start();
                    scInst.WaitForStatus(ServiceControllerStatus.Running);
                    AddLog(Name + " is Running");
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
                AddLog("Stopping " + Name + "...");
                new Thread(() =>
                {
                    scInst.Stop();
                    scInst.WaitForStatus(ServiceControllerStatus.Stopped);
                    AddLog(Name + " is Stopped");
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
