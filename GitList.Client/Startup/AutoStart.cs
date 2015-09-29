using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitList.Client.Startup
{
    public class AutoStart
    {
        private RegistryKey registryKey;

        public AutoStart()
        {
            registryKey = Registry.CurrentUser.OpenSubKey
                 ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }

        public void Register()
        {
            registryKey.SetValue(System.AppDomain.CurrentDomain.FriendlyName, Application.ExecutablePath);
        }

        public void Unregister()
        {
            registryKey.DeleteValue(System.AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
