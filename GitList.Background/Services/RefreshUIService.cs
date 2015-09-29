using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using GitList.Core.Abstracts;
using GitList.Core.Entities.Configuration;
using GitList.Core.Entities.DataContext;
using GitList.Core.Entities.Repository;


namespace GitList.Background.Services
{
    public class RefreshUIService : ABackgroundService
    {
        public RefreshUIService(GitListDataContext GitListDataContext)
            : base(GitListDataContext)
        {
            GitListDataContext.RootDirectoryItems = GitListDataContext.RootDirectoryItems ?? new ObservableCollection<RootDirectoryItem>();
        }

        public override void Start()
        {
            var refreshUIBackgroundService = new Thread(() =>
            {
                Thread.Sleep(2000);

                while (true)
                {
                    UpdateLastRefreshTime();
                    UpdateInProgressPercentage();
                    UpdateNotifyIcon();
                    Thread.Sleep(1000);
                }
            });

            refreshUIBackgroundService.IsBackground = true;
            refreshUIBackgroundService.Start();
        }

        public override void Stop()
        {
        }


        private void UpdateLastRefreshTime()
        {
            GitListDataContext.RootDirectoryItems.ToList().ForEach(x => x.LastRefresh = x.LastRefresh);
        }

        private void UpdateInProgressPercentage()
        {
            GitListDataContext.RootDirectoryItems.ToList().ForEach(root =>
            {
                var repoItems = root.RepositoryItems.ToList();
                if (repoItems.Count != 0)
                {
                    var notInProgressCount = repoItems.Count(repo => !repo.InProgress);
                    var totalCount = repoItems.Count();

                    //in progress percentage = (inprogress repos * 100 / total repos)
                    root.InProgressPercentage = notInProgressCount*100/totalCount;
                }
            });
        }


        private DateTime _lastNotifyIconUpdate;
        private bool _alertShown;
        private void UpdateNotifyIcon()
        {
            if (_lastNotifyIconUpdate.AddSeconds(30) <= DateTime.Now)
            {
                bool UpdateInProgress = GitListDataContext.RootDirectoryItems.ToList().Where(rootItem =>
                    rootItem != null &&
                    rootItem.RepositoryItems.ToList().Any(repo => repo.InProgress))
                    .ToList()
                    .Count > 0;

                if (UpdateInProgress && !_alertShown)
                {
                    GitListDataContext.NotifyIconHandler.SetWorkingIcon();
                    GitListDataContext.NotifyIconHandler.ShowAlert("GitList update in progress", TimeSpan.FromSeconds(10));
                    _alertShown = true;
                }
                else if (!UpdateInProgress && _alertShown)
                {
                    GitListDataContext.NotifyIconHandler.SetStandardIcon();
                    GitListDataContext.NotifyIconHandler.ShowAlert("GitList update finished.", TimeSpan.FromSeconds(10));
					_alertShown = false;
                }

                _lastNotifyIconUpdate = DateTime.Now;
            }
        }


    }
}