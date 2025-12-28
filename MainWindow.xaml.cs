using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WebServerControlPanel.Utils;

namespace WebServerControlPanel
{
    public partial class MainWindow
    {
        private readonly ObservableCollection<ScItem> _scItemList = new ObservableCollection<ScItem>();

        private readonly System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow() {
            InitializeComponent();
            InitTrayAndPosition();
            InitServiceNameList();
            LoadAllService();
        }

        private void InitTrayAndPosition() {
            Left = SystemParameters.WorkArea.Width - Width;
            Top = SystemParameters.WorkArea.Height - Height;
            _notifyIcon.Icon =
                System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            _notifyIcon.MouseClick += NotifyIcon_Click;
        }

        private void InitServiceNameList() {
            var scnames = RegUtil.GetScNameList();
            if (scnames == null || scnames.Length < 1) {
                scnames = new[] { "Redis" };
                RegUtil.SaveScNameList(scnames);
            }

            for (int i = 0, len = scnames.Length; i < len; i++) {
                _scItemList.Add(new ScItem(scnames[i], AddLog));
            }

            ServicesDataGrid.ItemsSource = _scItemList;
        }

        private void LoadAllService() {
            try {
                ServiceSelectBox.ItemsSource = ServiceController.GetServices().ToList();
            } catch (Exception ex) {
                AddLog($"加载服务列表失败: {ex.Message}");
            }
        }

        private void MainForm_StateChanged(object sender, EventArgs e) {
            var iconShow = WindowState == WindowState.Minimized;
            _notifyIcon.Visible = iconShow;
            ShowInTaskbar = !iconShow;
        }

        private void NotifyIcon_Click(object sender, EventArgs e) {
            WindowState = WindowState.Normal;
        }

        private void UpdateScNameList() {
            var names = _scItemList.Select(item => item.ServiceName).ToList();
            RegUtil.SaveScNameList(names);
        }

        private void ServiceAdd_Click(object sender, RoutedEventArgs e) {
            var item = (ServiceController)ServiceSelectBox.SelectedItem;

            var name = item.ServiceName;
            if (name.Length <= 0) return;
            var selectedIndex = ServicesDataGrid.SelectedIndex;
            var scitem = new ScItem(name, AddLog);
            if (selectedIndex >= 0) {
                _scItemList.Insert(selectedIndex, scitem);
            } else {
                _scItemList.Add(scitem);
            }

            ServiceSelectBox.Text = null;
            UpdateScNameList();
        }

        private void ServiceDel_Click(object sender, RoutedEventArgs e) {
            var selectedIndex = ServicesDataGrid.SelectedIndex;
            if (selectedIndex < 0) return;

            var item = (ScItem)ServicesDataGrid.SelectedItem;

            if (MessageBox.Show($"确定要删除{item.ServiceName}？", "删除项目",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK) return;
            _scItemList.RemoveAt(selectedIndex);
            UpdateScNameList();
        }

        private void ServiceUp_Click(object sender, RoutedEventArgs e) {
            var selectedIndex = ServicesDataGrid.SelectedIndex;
            if (selectedIndex <= 0) return;
            _scItemList.Move(selectedIndex, selectedIndex - 1);
            UpdateScNameList();
        }

        private void ServiceDown_Click(object sender, RoutedEventArgs e) {
            var selectedIndex = ServicesDataGrid.SelectedIndex;
            if (selectedIndex >= ServicesDataGrid.Items.Count - 1) return;
            _scItemList.Move(selectedIndex, selectedIndex + 1);
            UpdateScNameList();
        }

        private void ServiceAction_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            var service = (ScItem)button?.Tag;
            if (service == null) {
                AddLog("服务参数错误");
            } else if (service.IsRunning) {
                service.Stop();
            } else if (service.IsStopped) {
                service.Start();
            } else {
                AddLog($"服务状态异常: {service.StatusName}");
            }
        }

        private void ServicesDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            var dep = (DependencyObject)e.OriginalSource;

            // 查找DataGridRow
            while ((dep != null) && !(dep is DataGridRow)) {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridRow row) {
                // 设置当前行为选中行
                ServicesDataGrid.SelectedItem = row.DataContext;
                var selectedIdx = ServicesDataGrid.SelectedIndex;
                var scItemsCount = ServicesDataGrid.Items.Count;

                // 更新菜单项状态
                ServiceUpMenu.IsEnabled = selectedIdx > 0;
                ServiceDownMenu.IsEnabled = selectedIdx < scItemsCount - 1;
            } else {
                ServiceUpMenu.IsEnabled = false;
                ServiceDownMenu.IsEnabled = false;
            }

            e.Handled = true;
        }

        private void AddLog(string log) {
            NotifyText.AppendText(log + Environment.NewLine);
        }
    }
}