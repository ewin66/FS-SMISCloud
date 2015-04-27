using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using log4net;

namespace DataCenter.View
{
    using IWshRuntimeLibrary;

    class HelperTools
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HelperTools));
        //设置开机自启
        public static void OpenComputerRun(bool yesorno)
        {
            if (yesorno)
            {
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce");
                rk2.SetValue("ToServerByWord", path);
                rk2.Close();
                rk.Close();
            }
            else
            {
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce");
                rk2.DeleteValue("ToServerByWord", false);
                rk2.Close();
                rk.Close();

            }
        }

        //读取节点
        public static string GetConfigString(string key)
        {
            try
            {
                var str = Process.GetCurrentProcess().ProcessName;
                ConfigurationManager.RefreshSection("system.serviceModel");
                var configValue = ConfigurationManager.AppSettings[key];
                return configValue ?? null;
            }
            catch
            {
                return null;
            }
        }

        //刷新节点
        public static void UpdateAppConfig(string newKey, string newValue)
        {
            var isModified = false;
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == newKey)
                {
                    isModified = true;
                }
            }
            // Open App.Config of executable  
            var config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // You need to remove the old settings object before you can replace it  
            if (isModified)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            // Add an Application Setting.  
            config.AppSettings.Settings.Add(newKey, newValue);
            // Save the changes in App.config file.  
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.  
            ConfigurationManager.RefreshSection("appSettings");
        }


        public static void StartupAutoRun(bool trueorfalse)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Startup);//CommonStartup
            var dir = new DirectoryInfo(path);
            var name = ConfigurationManager.AppSettings["FormTitle"] +".lnk";
            switch (trueorfalse)
            {
                case true:
                    {
                        try
                        {
                            var files = dir.GetFiles(name);
                            if (files.Length > 0) return;
                            var shell = new WshShell();
                            var shortcut =
                                (IWshShortcut)
                                shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) +//CommonStartup
                                                     "\\" + name);
                            shortcut.TargetPath = Application.ExecutablePath;
                            shortcut.WorkingDirectory = Environment.CurrentDirectory;
                            shortcut.WindowStyle = 1;
                            shortcut.Description = name;
                            shortcut.Save();
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                            MessageBox.Show("设置失败！" + ex.Message);
                        }
                        return;
                    }
                case false:
                    {
                        var files = dir.GetFiles(name);
                        if (files.Length <= 0) return;
                        foreach (var fn in files)
                        {
                            fn.Delete();
                        }
                        return;
                    }
            }
        }


    }
}
