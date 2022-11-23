using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;

namespace wscp
{

    public partial class MainWindow : Window
    {

        private readonly ObservableCollection<string> SCNameList = new ObservableCollection<string>();

        private readonly NotifyIcon notifyIcon = new NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.MouseClick += NotifyIcon_Click;
            this.InitServiceNameList();
            this.ServiceNameList.ItemsSource = SCNameList;
        }

        private void InitServiceNameList()
        {
            string[] scnames = RegUtil.GetSCNameList();
            if (scnames == null || scnames.Length < 1)
            {
                scnames = new string[] { "Apache", "MySQL", "Redis", "nacos" };
                RegUtil.SaveSCNameList(scnames);
            }
            for (int i = 0, len = scnames.Length; i < len; i++)
            {
                new SCUtil(scnames[i], i, MainGrid, NotifyText);
                SCNameList.Add(scnames[i]);
            }
        }

        private void MainForm_StateChanged(object sender, EventArgs e)
        {
            bool iconShow = WindowState == WindowState.Minimized;
            this.notifyIcon.Visible = iconShow;
            this.ShowInTaskbar = !iconShow;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void ServiceAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = ServiceNameText.Text;
            if (name != null && name.Length > 0)
            {
                int SelectedIndex = ServiceNameList.SelectedIndex;
                if (SelectedIndex >= 0)
                {
                    SCNameList.Insert(SelectedIndex, name);
                }
                else
                {
                    SCNameList.Add(name);
                }
                ServiceNameText.Clear();
                RegUtil.SaveSCNameList(SCNameList);
            }
        }

        private void ServiceDel_Click(object sender, RoutedEventArgs e)
        {
            int SelectedIndex = ServiceNameList.SelectedIndex;
            if (SelectedIndex >= 0)
            {
                if (System.Windows.MessageBox.Show("确定要删除 " + ServiceNameList.SelectedItem + " ？", "删除项目", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    SCNameList.RemoveAt(SelectedIndex);
                    RegUtil.SaveSCNameList(SCNameList);
                }
            }
        }

        private void ServiceUp_Click(object sender, RoutedEventArgs e)
        {
            int SelectedIndex = ServiceNameList.SelectedIndex;
            if (SelectedIndex > 0)
            {
                SCNameList.Move(SelectedIndex, SelectedIndex - 1);
                RegUtil.SaveSCNameList(SCNameList);
            }

        }

        private void ServiceDown_Click(object sender, RoutedEventArgs e)
        {
            int SelectedIndex = ServiceNameList.SelectedIndex;
            if (SelectedIndex < ServiceNameList.Items.Count - 1)
            {
                SCNameList.Move(SelectedIndex, SelectedIndex + 1);
                RegUtil.SaveSCNameList(SCNameList);
            }
        }

    }

}
