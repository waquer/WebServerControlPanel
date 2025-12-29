using System;
using System.Linq;
using System.ServiceProcess;

namespace WebServerControlPanel
{
    public partial class ServiceWindow
    {
        public ServiceWindow()
        {
            InitializeComponent();
            try
            {
                AllSerivceDataGrid.ItemsSource = ServiceController.GetServices().ToList();
            }
            catch (Exception)
            {
                // 忽略异常
            }
        }
    }
}