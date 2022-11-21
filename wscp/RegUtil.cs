using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wscp
{
    internal class RegUtil
    {

        private readonly string RegistryKeyPath = @"SOFTWARE\MyLittleTools\WebServerControlPanel";

        public RegUtil()
        {
            RegistryKey RegKey = this.GetKey();
            RegKey.Close();
        }

        private RegistryKey GetKey()
        {
            RegistryView useRegistryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, useRegistryView);
            return hklm.CreateSubKey(RegistryKeyPath, true);
        }

        // 读取注册表
        public string GetValue(string name)
        {
            RegistryKey RegKey = this.GetKey();
            string value = RegKey.GetValue(name).ToString();
            RegKey.Close();
            return value;
        }

        // 写入注册表
        public void SaveValue(string name, string value)
        {
            RegistryKey RegKey = this.GetKey();
            RegKey.SetValue(name, value);
            RegKey.Close();
        }

        // 删除注册表
        public void DelValue(string name)
        {
            RegistryKey RegKey = this.GetKey();
            RegKey.DeleteValue(name);
            RegKey.Close();
        }

    }
}
