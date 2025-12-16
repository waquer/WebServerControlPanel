using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebServerControlPanel.Utils
{
    internal class ScItem
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));//全局通知(给监听此属性的控件)
        }

        private int index;

        private string name;

        private int status;

        public int ID
        {
            get => index;
            set
            {
                index = value;
                NotifyChanged();
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyChanged();
            }
        }

        public int Status
        {
            get => status;
            set
            {
                status = value;
                NotifyChanged();
            }
        }
        public ScItem(string scname, int index)
        {
            this.name = scname;
            this.index = index;
            this.status = 0;
        }

    }
}
