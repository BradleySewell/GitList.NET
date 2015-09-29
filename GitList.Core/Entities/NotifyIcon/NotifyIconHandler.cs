using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitList.Core.Entities.DataContext;
using Application = System.Windows.Application;

namespace GitList.Core.Entities.NotifyIcon
{

    public class NotifyIconHandler
    {
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Drawing.Icon StandardIcon;
        private System.Drawing.Icon WorkingIcon;

        public delegate void OnClick();
        public event OnClick OnClickEvent;

        public NotifyIconHandler(System.Drawing.Icon standardIcon, System.Drawing.Icon workingIcon)
        {
            StandardIcon = standardIcon;
            WorkingIcon = workingIcon;

            NotifyIcon = new System.Windows.Forms.NotifyIcon { BalloonTipText = "GitList", Icon = standardIcon, Visible = true};
            
            var menu = new System.Windows.Forms.ContextMenu();
            menu.MenuItems.Add("Show", delegate(object sender, EventArgs e)
            {
                NotifyDoubleClick(sender, null);
            });
            menu.MenuItems.Add("Exit", delegate(object sender, EventArgs e)
            {
                Application.Current.Shutdown();
            });

            NotifyIcon.ContextMenu = menu;
            
            NotifyIcon.MouseDoubleClick += (sender, e) => NotifyDoubleClick(sender, e);
            NotifyIcon.BalloonTipClicked += (sender, e) => BalloonTipClick(sender, e);

            ShowAlert("GitList is running...", TimeSpan.FromSeconds(30));

            AutoShowInSystemTray();
        }

        private void NotifyDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnClickEvent();
        }

        private void BalloonTipClick(object sender, EventArgs e)
        {
            OnClickEvent();
        }

        private void AutoShowInSystemTray()
        {
            var registryKey = Registry.CurrentUser.CreateSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\TrayNotify");
            registryKey.SetValue("EnableAutoTray", 0);
        }


        public void ShowAlert(string message, TimeSpan timeSpan)
        {
            NotifyIcon.ShowBalloonTip(Convert.ToInt32(timeSpan.TotalMilliseconds), "", message, System.Windows.Forms.ToolTipIcon.None);
        }

        public void SetStandardIcon()
        {
            NotifyIcon.Icon = StandardIcon;
        }

        public void SetWorkingIcon()
        {
            NotifyIcon.Icon = WorkingIcon;
        }
    }
}
