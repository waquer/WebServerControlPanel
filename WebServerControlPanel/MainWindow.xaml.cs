using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebServerControlPanel.Utils;

namespace WebServerControlPanel
{
    public partial class MainWindow
    {
        private readonly ObservableCollection<string> _scNameList = new ObservableCollection<string>();

        private readonly ObservableCollection<ScItem> _scItemList = new ObservableCollection<ScItem>();

        private readonly System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            Left = SystemParameters.WorkArea.Width - Width;
            Top = SystemParameters.WorkArea.Height - Height;
            _notifyIcon.Icon =
                System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            _notifyIcon.MouseClick += NotifyIcon_Click;
            InitServiceNameList();
            ServiceNameList.ItemsSource = _scNameList;
            ServiceNameGrid.ItemsSource = _scItemList;
        }

        private void InitServiceNameList()
        {
            var scnames = RegUtil.GetScNameList();
            if (scnames == null || scnames.Length < 1)
            {
                scnames = new[] { "Apache", "MySQL", "Redis" };
                RegUtil.SaveScNameList(scnames);
            }

            for (int i = 0, len = scnames.Length; i < len; i++)
            {
                var util = new ScUtil(scnames[i], i, MainGrid, NotifyText);
                _scNameList.Add(util.ScName);
                _scItemList.Add(new ScItem(scnames[i], i));
            }
        }

        private void MainForm_StateChanged(object sender, EventArgs e)
        {
            var iconShow = WindowState == WindowState.Minimized;
            _notifyIcon.Visible = iconShow;
            ShowInTaskbar = !iconShow;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void ServiceAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = ServiceNameText.Text;
            if (name.Length <= 0) return;
            var selectedIndex = ServiceNameList.SelectedIndex;
            if (selectedIndex >= 0)
            {
                _scNameList.Insert(selectedIndex, name);
            }
            else
            {
                _scNameList.Add(name);
            }

            ServiceNameText.Clear();
            RegUtil.SaveScNameList(_scNameList);
        }

        private void ServiceDel_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = ServiceNameList.SelectedIndex;
            if (selectedIndex < 0) return;
            if (MessageBox.Show("确定要删除 " + ServiceNameList.SelectedItem + " ？", "删除项目", MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) != MessageBoxResult.OK) return;
            _scNameList.RemoveAt(selectedIndex);
            RegUtil.SaveScNameList(_scNameList);
        }

        private void ServiceUp_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = ServiceNameList.SelectedIndex;
            if (selectedIndex <= 0) return;
            _scNameList.Move(selectedIndex, selectedIndex - 1);
            RegUtil.SaveScNameList(_scNameList);
        }

        private void ServiceDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = ServiceNameList.SelectedIndex;
            if (selectedIndex >= ServiceNameList.Items.Count - 1) return;
            _scNameList.Move(selectedIndex, selectedIndex + 1);
            RegUtil.SaveScNameList(_scNameList);
        }
    }
}