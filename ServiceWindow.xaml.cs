using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WebServerControlPanel
{
    public partial class ServiceWindow
    {
        private readonly Action<string> _addService;

        private readonly ICollectionView _serviceList;

        public ServiceWindow(Action<string> addService)
        {
            _addService = addService;
            InitializeComponent();
            var services = GetServices();
            _serviceList = CollectionViewSource.GetDefaultView(services);
            AllSerivceDataGrid.ItemsSource = _serviceList;
        }

        private List<ServiceController> GetServices()
        {
            return ServiceController.GetServices().ToList();
        }

        private void ListFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_serviceList == null) return;
            _serviceList.Filter = FilterServices;
            _serviceList.Refresh();
        }

        private bool FilterServices(object item)
        {
            if (!(item is ServiceController service)) return false;
            var filterText = ListFilter.Text.ToLower();
            return string.IsNullOrEmpty(filterText) ||
                   service.ServiceName.ToLower().Contains(filterText) ||
                   service.DisplayName.ToLower().Contains(filterText);
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