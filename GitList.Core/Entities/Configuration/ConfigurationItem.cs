using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitList.Core.Entities.Configuration
{
    [Serializable]
    public class ConfigurationItem : INotifyPropertyChanged
    {
        private bool autoStartEnabled;
        public bool AutoStartEnabled
        {
            get
            {
                return this.autoStartEnabled;
            }
            set
            {
                this.autoStartEnabled = value;
                OnPropertyChanged("AutoStartEnabled");
            }
        }

        private bool minimizeOnClose;
        public bool MinimizeOnClose
        {
            get
            {
                return this.minimizeOnClose;
            }
            set
            {
                this.minimizeOnClose = value;
                OnPropertyChanged("MinimizeOnClose");
            }
        }

        private bool refreshPeriodicallyEnabled;
        public bool RefreshPeriodicallyEnabled
        {
            get
            {
                return this.refreshPeriodicallyEnabled;
            }
            set
            {
                this.refreshPeriodicallyEnabled = value;
                OnPropertyChanged("RefreshPeriodicallyEnabled");
            }
        }

        private bool refreshDetectionEnabled;
        public bool RefreshDetectionEnabled
        {
            get
            {
                return this.refreshDetectionEnabled;
            }
            set
            {
                this.refreshDetectionEnabled = value;
                OnPropertyChanged("RefreshDetectionEnabled");
            }
        }




        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
