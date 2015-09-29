using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GitList.Core.Entities.Repository
{
    [Serializable]
    public class RootDirectoryItem : INotifyPropertyChanged
    {
        public string Path { get; set; }

        public ObservableCollection<RepositoryItem> RepositoryItems { get; set; }

        [field: NonSerialized]
        private DateTime lastRefresh;
        [field: NonSerialized]
        public DateTime LastRefresh
        {
            get { return lastRefresh; }
            set
            {
                lastRefresh = value;
                OnPropertyChanged("LastRefresh");
            }
        }

        [field: NonSerialized]
        public bool Refreshing { get; set; }

        [field: NonSerialized]
        private int inProgressPercentage;
        [field: NonSerialized]
        public int InProgressPercentage
        {
            get { return inProgressPercentage; }
            set
            {
                inProgressPercentage = value;
                OnPropertyChanged("InProgressPercentage");
            }
        }

        [field: NonSerialized]
        public bool RepositoriesDetected { get; set; }

        private bool collapsed;
        public bool Collapsed
        {
            get { return collapsed; }
            set
            {
                collapsed = value;
                OnPropertyChanged("Collapsed");
            }
        }


        [field: NonSerialized]
        private string rootBranch;
        public string RootBranch
        {
            get { return rootBranch; }
            set
            {
                rootBranch = value;
                OnPropertyChanged("RootBranch");
            }
        }


        [field: NonSerialized]
        private ObservableCollection<string> repositoryBranches;
        public ObservableCollection<string> RepositoryBranches
        {
            get { return repositoryBranches; }
            set
            {
                repositoryBranches = value;
                OnPropertyChanged("RepositoryBranches");
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
