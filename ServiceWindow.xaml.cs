using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;

namespace WebServerControlPanel
{
    public partial class ServiceWindow
    {
        
        private readonly Action<string> _addService;
        
        public ServiceWindow(Action<string> addService)
        {
            _addService = addService;
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
        
        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var service = (ServiceController)button?.Tag;
            var name = service?.ServiceName;
            if (name != null)
            {
                _addService(name);
            }
            Close();
        }
    }
}