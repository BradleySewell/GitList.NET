using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitList.Core.Constants;
using GitList.Core.Entities.Configuration;
using GitList.Core.Entities.DataContext;
using GitList.Core.Entities.Dispatcher;
using GitList.Core.Entities.Git;
using GitList.Core.Entities.Serialization;
using GitList.Core.Interfaces;
using GitList.Core.Abstracts;

namespace GitList.Core.Entities.Repository
{
    public class RepositoryController : AControllers
    {
        public RepositoryController(GitListDataContext gitListDataContext) : base(gitListDataContext)
        {
        }

        public override void Initialise()
        {
            GitListDataContext.RootDirectoryItems = new ObservableCollection<RootDirectoryItem>();
        }

        public override void Start()
        {
            LoadSavedRootDirectoryItems();
        }

        private void LoadSavedRootDirectoryItems()
        {
            var deSerializedRootDirectoryItems = BinarySerializer.LoadFile(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, Constants.FileNameConstants.SAVED_ROOT_DIRECTORIES_FILENAME));
            if (deSerializedRootDirectoryItems == null) return;
            
            var rootDirectoryItems = (ObservableCollection<RootDirectoryItem>)deSerializedRootDirectoryItems;
            foreach (var rootDirectoryItem in rootDirectoryItems)
            {
                DiscoverRepositoriesAndAddToList(rootDirectoryItem.Path, rootDirectoryItem.Collapsed);
            }
            RefreshAll();
        }


        public void AddRootDirectoryPath()
        {
            if (!Directory.Exists(GitListDataContext.RootDirectoryPath))
            {
                SetRootDirectoryPathError("Directory is not valid or does not exist!");
                return;
            }
            if (GitListDataContext.RootDirectoryItems.Any(x => x.Path == GitListDataContext.RootDirectoryPath))
            {
                SetRootDirectoryPathError("Directory has already been added!");
                return;
            }


            SetRootDirectoryPathError(null);

            var rootItem = DiscoverRepositoriesAndAddToList(GitListDataContext.RootDirectoryPath);

            ClearRootDirectoryPath();

            if (rootItem != null)
            {
                RefreshRepositories(rootItem, false);
            }
        }

        private RootDirectoryItem DiscoverRepositoriesAndAddToList(string path, bool collapsed = false)
        {
            if (!Directory.Exists(path) || GitListDataContext.RootDirectoryItems.Any(x => x.Path == path))
            {
                return null;
            }


            var repositories = new GitManager().FindRepositories(path);
            var newRootItem = new RootDirectoryItem()
            {
                Path = path,
                RepositoryItems = repositories,
                LastRefresh = DateTime.Now,
                RepositoriesDetected = repositories.Count > 0,
                Collapsed = collapsed,
                RepositoryBranches = new ObservableCollection<string>()
            };

            var watcher = new FileSystemWatcher(newRootItem.Path);
            watcher.Changed += (o, args) => OnChanged(o, args, newRootItem);
            watcher.Created += (o, args) => OnChanged(o, args, newRootItem);
            watcher.Deleted += (o, args) => OnChanged(o, args, newRootItem);
            watcher.Renamed += (o, args) => OnChanged(o, args, newRootItem);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            AddRootDirectoryItem(newRootItem);

            //if (gitListDataContext.RootDirectoryItems.Count > 0 && gitListDataContext.RootDirectoryItems[0].RepositoryItems.Count > 0 && gitListDataContext.RootDirectoryItems[0].Collapsed == false)
            //{
            //    gitListDataContext.SelectedRepositoryItem = gitListDataContext.RootDirectoryItems[0].RepositoryItems[0];
            //    //gitListDataContext.Controllers.ConsoleController.ChangeRepository(gitListDataContext.RootDirectoryItems[0].RepositoryItems[0]);
            //}

            return newRootItem;
        }

        private void OnChanged(object sender, FileSystemEventArgs e, RootDirectoryItem item)
        {
            if (GitListDataContext.Configuration.RefreshDetectionEnabled)
            {
                TimeSpan lastRefresh = (DateTime.Now - item.LastRefresh);

                var anyInProgress = item.RepositoryItems.ToList().Any(x => x.InProgress);

                if (lastRefresh.TotalMinutes >= ConfigurationLoader.refreshDetectionInterval && !item.Refreshing && !anyInProgress)
                {
                    RefreshRepositories(item, true);
                }
                else
                {
                }
            }
        }




        private void SetRootDirectoryPathError(string errorMessage)
        {
            GitListDataContext.RootDirectoryPathError = errorMessage;
        }

        private void ClearRootDirectoryPath()
        {
            GitListDataContext.RootDirectoryPath = null;
        }



        public void RemoveRootDirectoryItem(RootDirectoryItem item)
        {
            if (GitListDataContext.RootDirectoryItems.Contains(item))
            {
                GitListDataContext.RootDirectoryItems.Remove(item);
                if (GitListDataContext.SelectedRepositoryItem != null && GitListDataContext.SelectedRepositoryItem.Parent == item)
                {
                    GitListDataContext.SelectedRepositoryItem = null;
                }

                SaveRootDirectoryitems();
            }
        }

        public void AddRootDirectoryItem(RootDirectoryItem item)
        {
            if (!GitListDataContext.RootDirectoryItems.Contains(item))
            {
                GitListDataContext.RootDirectoryItems.Add(item);
                SaveRootDirectoryitems();
            }
        }

        public void SaveRootDirectoryitems()
        {
            BinarySerializer.SaveFile(GitListDataContext.RootDirectoryItems, string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, FileNameConstants.SAVED_ROOT_DIRECTORIES_FILENAME));
        }



        public void RefreshAll()
        {
            GitListDataContext.RootDirectoryItems.ToList().ForEach(x => RefreshRepositories(x, false));
        }

        public void RefreshRepositories(RootDirectoryItem rootItem, bool eventTriggered)
        {
            if (rootItem == null)
            {
                return;
            }

            rootItem.LastRefresh = DateTime.Now;
            rootItem.Refreshing = true;

            SetRootRefreshInProgress(rootItem);

            if (!Directory.Exists(rootItem.Path))
            {
                RemoveRootDirectoryItem(rootItem);
                return;
            }

            new Thread(() =>
            {
                if (eventTriggered)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }

                DispatchInvoker.InvokeBackground(() =>
                {
                    rootItem.RepositoryBranches.Clear();
                });

                rootItem.RepositoryItems.ToList().ForEach(repo =>
                {
                    if (!Directory.Exists(repo.Path))
                    {
                        DispatchInvoker.InvokeBackground(() =>
                        {
                            rootItem.RepositoryItems.Remove(repo);
                            //item.RepositoryItems = new GitManager().FindRepositories(item.Path);
                        });
                        return;
                    }

                    var changesList = new ObservableCollection<string>();
                    new GitManager().Changes(repo).ForEach(changesList.Add);
                    var branchList = GetBranchList(repo);
                    var currentBranch = GetCurrentBranchName(repo);
                    var aheadCount = new GitManager().Aheadby(repo);
                    var behindCount = new GitManager().BehindBy(repo);

                    var commitLog = new ObservableCollection<string>();
                    new GitManager().CommitLog(repo).ForEach(commitLog.Add);

                    DispatchInvoker.InvokeBackground(() =>
                    {
                        branchList.ToList().ForEach(branch =>
                        {
                            if (!rootItem.RepositoryBranches.Contains(branch) && branch != "DETACHED HEAD")
                            {
                                rootItem.RepositoryBranches.Add(branch);
                            }
                        });

                        repo.Parent = rootItem;
                        repo.Modified = changesList.Count > 0;
                        repo.Changes = changesList;
                        repo.Branches = branchList;
                        repo.CurrentBranch = currentBranch;
                        repo.CommitsAheadBy = aheadCount;
                        repo.CommitsBehindBy = behindCount;
                        repo.CommitLog = commitLog;
                        repo.InProgress = false;
                        rootItem.Refreshing = false;

                    });
                });


                DispatchInvoker.InvokeBackground(() =>
                {
                    if (rootItem.RepositoryItems != null)
                    {
                        var sortedList = rootItem.RepositoryItems.OrderBy(x => x.CurrentBranch).ThenBy(x => x.Name).ToList();
                        rootItem.RepositoryItems.Clear();
                        sortedList.ForEach(x =>
                        {
                            rootItem.RepositoryItems.Add(x);
                        });
                    }
                });

            }).Start();

        }











        public string GetCurrentBranchName(RepositoryItem item)
        {
            var branchName = new GitManager().CurrentBranchName(item);

            if (branchName == "(no branch)")
            {
                branchName = "DETACHED HEAD";
            }

            return branchName;
        }

        private ObservableCollection<string> GetBranchList(RepositoryItem item)
        {
            var list = new ObservableCollection<string>();
            new GitManager().Branches(item).ForEach(list.Add);

            if (list.Count == 0)
            {
                list.Add("master");
            }
            list.Add("DETACHED HEAD");


            return list;
        }


        //public void Reset(RepositoryItem item)
        //{
        //    new GitManager().Reset(item);
        //}

        public void Fetch(RepositoryItem item)
        {
            new GitManager().FetchAndReset(item);
            DispatchInvoker.InvokeBackground(() => { item.InProgress = false; });
        }


        public void CollapseRootDirectory(RootDirectoryItem item)
        {
            item.Collapsed = !item.Collapsed;
            SaveRootDirectoryitems();
        }


        public void ChangeRootBranch(RootDirectoryItem item)
        {
            if (item != null && item.RepositoryItems != null)
            {
                SetRootRefreshInProgress(item);

                new Thread(() =>
                {
                    item.RepositoryItems.ToList().ForEach(repo =>
                    {
                        if (repo.Branches.Contains(item.RootBranch) && repo.CurrentBranch != item.RootBranch)
                        {
                            repo.CurrentBranch = item.RootBranch;
                            ChangeBranch(repo);
                        }
                    });

                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    DispatchInvoker.InvokeBackground(() =>
                    {
                        item.RootBranch = null;
                        RefreshRepositories(item, false);
                    });
                }).Start();
            }
        }

        public void ChangeRepositoryBranch(RepositoryItem item)
        {
            if (item != null && !string.IsNullOrEmpty(item.CurrentBranch))
            {
                SetRootRefreshInProgress(item.Parent);

                new Thread(() =>
                {
                    ChangeBranch(item);

                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    DispatchInvoker.InvokeBackground(() =>
                    {
                        RefreshRepositories(item.Parent, false);
                    });
                }).Start();
            }
        }


        private void ChangeBranch(RepositoryItem item)
        {
            item.Message = null;

            new Thread(() =>
            {
                try
                {
                    if (item.CurrentBranch == "DETACHED HEAD")
                    {
                        item.Message = "You cannot checkout branch 'DETACHED HEAD'";
                    }
                    else
                    {
                        new GitManager().ChangeBranch(item, item.CurrentBranch);
                    }
                }
                catch (Exception e)
                {
                    DispatchInvoker.InvokeBackground(() =>
                    {
                        item.Message = e.Message;
                        item.InProgress = false;
                    });
                }

                DispatchInvoker.InvokeBackground(() =>
                {
                    item.CurrentBranch = GetCurrentBranchName(item);
                    item.InProgress = false;
                });

            }).Start();
        }

        public void FetchAndResetRootBranch(RootDirectoryItem item)
        {
            if (item != null && item.RepositoryItems != null)
            {
                SetRootRefreshInProgress(item);

                new Thread(() =>
                {
                    item.RepositoryItems.ToList().ForEach(Fetch);

                    DispatchInvoker.InvokeBackground(() => { RefreshRepositories(item, false); });
                }).Start();
            }
        }

        public void SetRootRefreshInProgress(RootDirectoryItem item)
        {
            item.RepositoryItems.ToList().ForEach(x =>
            {
                x.InProgress = true;
                x.Modified = null;
                x.Changes = null;
                x.CommitsAheadBy = 0;
                x.CommitsBehindBy = 0;
                x.CommitLog = null;
                x.Message = null;
            });
        }

    }
}
