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
    public class RefreshRepositoryService : ABackgroundService
    {
        public RefreshRepositoryService(GitListDataContext gitListDataContext)
            : base(gitListDataContext)
        {
            GitListDataContext.RootDirectoryItems = gitListDataContext.RootDirectoryItems ?? new ObservableCollection<RootDirectoryItem>();
        }

        public override void Start()
        {
            var refreshRepositoryBackgroundService = new Thread(() =>
            {
                Thread.Sleep(60000);

                while (true && GitListDataContext.Configuration.RefreshPeriodicallyEnabled)
                {
                    if (RefreshNotInProgress())
                    {
                        var nextItemToRefresh = NextRootDirectoryToRefresh();
                        if (nextItemToRefresh != null)
                        {
                            GitListDataContext.Controllers.RepositoryController.RefreshRepositories(
                                nextItemToRefresh, false);
                        }
                    }
                    Thread.Sleep(15000);
                }
            });

            refreshRepositoryBackgroundService.IsBackground = true;
            refreshRepositoryBackgroundService.Start();
        }

        public override void Stop()
        {
        }


        private bool RefreshNotInProgress()
        {
            return GitListDataContext.RootDirectoryItems.ToList().Where(rootItem =>
                rootItem != null &&
                rootItem.Refreshing && 
                rootItem.RepositoryItems.ToList().Any(repo => repo.InProgress))
                .ToList()
                .Count == 0;
        }


        private RootDirectoryItem NextRootDirectoryToRefresh()
        {
            return GitListDataContext.RootDirectoryItems.ToList().Where(rootItem =>
                rootItem != null &&
                rootItem.Refreshing == false &&
                rootItem.RepositoryItems.ToList().Any(repo => repo.InProgress) == false &&
                rootItem.LastRefresh.AddMinutes(ConfigurationLoader.refreshInterval) <= DateTime.Now).DefaultIfEmpty(null).FirstOrDefault();
        }
    }
}