using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace wscp
{
    internal static class RegUtil
    {
        private const string RegistryKeyPath = @"SOFTWARE\MyLittleTools\WebServerControlPanel";

        private const string ServiceListKey = "ServiceList";

        private static RegistryKey GetKey()
        {
            var useRegistryView =
                Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, useRegistryView);
            return hklm.CreateSubKey(RegistryKeyPath, true);
        }

        // 读取注册表
        private static string GetValue(string name)
        {
            var regKey = GetKey();
            var value = regKey.GetValue(name)?.ToString();
            regKey.Close();
            return value;
        }

        // 写入注册表
        private static void SaveValue(string name, string value, RegistryValueKind kind)
        {
            var regKey = GetKey();
            regKey.SetValue(name, value, kind);
            regKey.Close();
        }

        private static void SaveValue(string name, string value)
        {
            var regKey = GetKey();
            regKey.SetValue(name, value, RegistryValueKind.String);
            regKey.Close();
        }

        // 删除注册表
        private static void DelValue(string name)
        {
            var regKey = GetKey();
            regKey.DeleteValue(name);
            regKey.Close();
        }

        public static string[] GetScNameList()
        {
            var list = GetValue(ServiceListKey);
            return list?.Split(',');
        }

        public static void SaveScNameList(IEnumerable<string> list)
        {
            if (list == null)
            {
                DelValue(ServiceListKey);
            }
            else
            {
                SaveValue(ServiceListKey, string.Join(",", list));
            }
        }
    }
}