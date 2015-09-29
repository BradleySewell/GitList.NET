using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using GitList.Core.Abstracts;
using GitList.Core.Entities.Configuration;
using GitList.Core.Entities.DataContext;
using GitList.Core.Entities.Repository;


namespace GitList.Background.Services
{
    public class NotificationAlertService : ABackgroundService
    {
        public NotificationAlertService(GitListDataContext GitListDataContext)
            : base(GitListDataContext)
        {
            GitListDataContext.RootDirectoryItems = GitListDataContext.RootDirectoryItems ?? new ObservableCollection<RootDirectoryItem>();
        }

        public override void Start()
        {
            var notificationAlertBackgroundService = new Thread(() =>
            {
                Thread.Sleep(60000);

                while (true)
                {
                    if (RefreshNotInProgress())
                    {
                        var repoBehindCount = NumberOfRepositoriesBehindOnCommits();
                        if (repoBehindCount > 0)
                        {
                            GitListDataContext.NotifyIconHandler.ShowAlert(repoBehindCount == 1
                                ? "You have 1 repository out of date"
                                : string.Format("You have {0} repositories out of date", repoBehindCount),
                                TimeSpan.FromSeconds(30));
                        }
                    }
                    Thread.Sleep(TimeSpan.FromMinutes(ConfigurationLoader.notificationAlertInterval));
                }
            });

            notificationAlertBackgroundService.IsBackground = true;
            notificationAlertBackgroundService.Start();
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


        private int NumberOfRepositoriesBehindOnCommits()
        {
            int repoCount = 0;

            var rootDirectories = GitListDataContext.RootDirectoryItems.ToList().Where(rootItem =>
                rootItem != null &&
                rootItem.Refreshing == false).ToList();

            rootDirectories.ForEach(rootItem =>
            {
                repoCount += rootItem.RepositoryItems.Count(repo => repo.CommitsBehindBy > 0);
            });

            return repoCount;
        }
    }
}