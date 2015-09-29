using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GitList.Core.Entities.Dispatcher;
using GitList.Core.Entities.Git;

namespace GitList.Core.Entities.Repository
{
    [Serializable]
    public class RepositoryItem : INotifyPropertyChanged
    {
        private RootDirectoryItem parent;
        public RootDirectoryItem Parent
        {
            get { return this.parent; }
            set
            {
                this.parent = value;
                OnPropertyChanged("Parent");
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        private string path;
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
                OnPropertyChanged("Path");
            }
        }

        private bool exists;
        public bool Exists
        {
            get
            {
                return this.exists;
            }
            set
            {
                this.exists = value;
                OnPropertyChanged("Exists");
            }
        }

        private Nullable<bool> modified;
        public Nullable<bool> Modified
        {
            get
            {
                return this.modified;
            }
            set
            {
                this.modified = value;
                OnPropertyChanged("Modified");
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private string currentBranch;
        public string CurrentBranch
        {
            get { return currentBranch; }
            set
            {
                currentBranch = value;
                OnPropertyChanged("CurrentBranch");
            }
        }

        private int commitsAheadBy;
        public int CommitsAheadBy
        {
            get { return commitsAheadBy; }
            set
            {
                commitsAheadBy = value;
                OnPropertyChanged("CommitsAheadBy");
            }
        }

        private int commitsBehindBy;
        public int CommitsBehindBy
        {
            get { return commitsBehindBy; }
            set
            {
                commitsBehindBy = value;
                OnPropertyChanged("CommitsBehindBy");
            }
        }

        

        private ObservableCollection<string> branches;
        public ObservableCollection<string> Branches
        {
            get { return branches; }
            set
            {
                branches = value;
                OnPropertyChanged("Branches");
            }
        }

        private ObservableCollection<string> changes;
        public ObservableCollection<string> Changes
        {
            get
            {
                return this.changes;
            }
            set
            {
                this.changes = value;
                OnPropertyChanged("Changes");
            }
        }


        private ObservableCollection<string> commitLog;
        public ObservableCollection<string> CommitLog
        {
            get
            {
                return this.commitLog;
            }
            set
            {
                this.commitLog = value;
                OnPropertyChanged("CommitLog");
            }
        }

        
        public ObservableCollection<string> CommandHistory { get; set; }


        private bool inProgress;
        public bool InProgress
        {
            get
            {
                return this.inProgress;
            }
            set
            {
                this.inProgress = value;
                OnPropertyChanged("InProgress");
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
