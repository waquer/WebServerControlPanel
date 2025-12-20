using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WebServerControlPanel.Utils;

namespace WebServerControlPanel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));//全局通知(给监听此属性的控件)
        }

        public ICommand ActionCommand { get; set; }

        public MainViewModel()
        {
            ActionCommand = new ScCommand(DoAction);
        }

        private void DoAction(Object obj)
        {
            Debug.WriteLine("dsfdsf");
        }

    }
}
