using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitList.Core.Entities.Configuration
{
    public static class ConfigurationLoader
    {
        private static string LoadSettingString(string SettingName)
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                return ConfigurationManager.AppSettings[SettingName];
            }
            catch
            {
                return "";
            }
        }

        private static int LoadSettingInt(string SettingName)
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                return Convert.ToInt32(ConfigurationManager.AppSettings[SettingName]);
            }
            catch
            {
                return 0;
            }
        }

        private static bool LoadSettingBool(string SettingName)
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                return Convert.ToBoolean(ConfigurationManager.AppSettings[SettingName]);
            }
            catch
            {
                return false;
            }
        }


        private static void UpdateSetting(string SettingName, string NewValue)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[SettingName].Value = NewValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }



        private static string _gitExePath;
        public static string gitExePath
        {
            get
            {
                _gitExePath = LoadSettingString("gitExePath");
                return _gitExePath;
            }
            set
            {
                UpdateSetting("gitExePath", value);
                _gitExePath = LoadSettingString("gitExePath");
            }
        }


        private static int _refreshInterval;
        public static int refreshInterval
        {
            get
            {
                _refreshInterval = LoadSettingInt("refreshInterval");
                return _refreshInterval;
            }
            set
            {
                UpdateSetting("refreshInterval", value.ToString());
                _refreshInterval = LoadSettingInt("refreshInterval");
            }
        }

        private static int _notificationAlertInterval;
        public static int notificationAlertInterval
        {
            get
            {
                _notificationAlertInterval = LoadSettingInt("notificationAlertInterval");
                return _notificationAlertInterval;
            }
            set
            {
                UpdateSetting("notificationAlertInterval", value.ToString());
                _notificationAlertInterval = LoadSettingInt("notificationAlertInterval");
            }
        }



    }
}
