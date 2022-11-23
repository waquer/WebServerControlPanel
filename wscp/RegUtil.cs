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

        private static readonly string RegistryKeyPath = @"SOFTWARE\MyLittleTools\WebServerControlPanel";

        private static readonly string ServiceListKey = "ServiceList";

        private static RegistryKey GetKey()
        {
            RegistryView useRegistryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, useRegistryView);
            return hklm.CreateSubKey(RegistryKeyPath, true);
        }

        // 读取注册表
        public static string GetValue(string name)
        {
            RegistryKey RegKey = GetKey();
            string value = RegKey.GetValue(name)?.ToString();
            RegKey.Close();
            return value;
        }

        // 写入注册表
        public static void SaveValue(string name, string value, RegistryValueKind kind)
        {
            RegistryKey RegKey = GetKey();
            RegKey.SetValue(name, value, kind);
            RegKey.Close();
        }

        public static void SaveValue(string name, string value)
        {
            RegistryKey RegKey = GetKey();
            RegKey.SetValue(name, value, RegistryValueKind.String);
            RegKey.Close();
        }

        // 删除注册表
        public static void DelValue(string name)
        {
            RegistryKey RegKey = GetKey();
            RegKey.DeleteValue(name);
            RegKey.Close();
        }

        public static string[] GetSCNameList()
        {
            String list = GetValue(ServiceListKey);
            return list?.Split(',');
        }

        public static void SaveSCNameList(string[] list)
        {
            if (list == null)
            {
                DelValue(ServiceListKey);
            }
            else
            {
                SaveValue(ServiceListKey, String.Join(",", list));
            }
        }

    }
}
