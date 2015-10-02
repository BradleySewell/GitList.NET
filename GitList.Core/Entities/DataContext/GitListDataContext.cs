using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GitList.Core.Entities.Controller;
using GitList.Core.Entities.NotifyIcon;
using GitList.Core.Entities.Repository;
using GitList.Core.Entities.Configuration;

namespace GitList.Core.Entities.DataContext
{
    public class GitListDataContext : INotifyPropertyChanged
    {
        public ControllerInitialiser Controllers;

        public NotifyIconHandler NotifyIconHandler { get; set; }





        private ConfigurationItem configuration;
        public ConfigurationItem Configuration
        {
            get
            {
                return this.configuration;
            }
            set
            {
                this.configuration = value;
                OnPropertyChanged("Configuration");
            }
        }


        private string rootDirectoryPath;
        public string RootDirectoryPath
        {
            get
            {
                return this.rootDirectoryPath;
            }
            set
            {
                this.rootDirectoryPath = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
                OnPropertyChanged("RootDirectoryPath");
            }
        }

        private string rootDirectoryPathError;
        public string RootDirectoryPathError
        {
            get
            {
                return this.rootDirectoryPathError;
            }
            set
            {
                this.rootDirectoryPathError = value;
                OnPropertyChanged("RootDirectoryPathError");
            }
        }

        private ObservableCollection<RootDirectoryItem> rootDirectoryItems;
        public ObservableCollection<RootDirectoryItem> RootDirectoryItems
        {
            get
            {
                return this.rootDirectoryItems;
            }
            set
            {
                this.rootDirectoryItems = value;
                OnPropertyChanged("RootDirectoryItems");
            }
        }

        private RepositoryItem selectedRepositoryItem;
        public RepositoryItem SelectedRepositoryItem
        {
            get
            {
                return this.selectedRepositoryItem;
            }
            set
            {
                this.selectedRepositoryItem = value;
                OnPropertyChanged("SelectedRepositoryItem");
            }
        }


        private ConsoleControl.WPF.ConsoleControl consoleControl;
        public ConsoleControl.WPF.ConsoleControl ConsoleControl
        {
            get
            {
                return this.consoleControl;
            }
            set
            {
                this.consoleControl = value;
                OnPropertyChanged("ConsoleControl");
            }
        }

        private string consoleCommand;
        public string ConsoleCommand
        {
            get
            {
                return this.consoleCommand;
            }
            set
            {
                this.consoleCommand = value;
                OnPropertyChanged("ConsoleCommand");
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
