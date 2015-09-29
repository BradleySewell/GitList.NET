using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Entities.DataContext;
using GitList.Core.Interfaces;

namespace GitList.Background.Services
{
    public class BackgroundServices : IBackgroundService
    {
        private List<IBackgroundService> backgroundServices;

        public BackgroundServices(GitListDataContext gitListDataContext)
        {
            backgroundServices = new List<IBackgroundService>() 
            {
                new RefreshRepositoryService(gitListDataContext),
                new RefreshUIService(gitListDataContext),
                new NotificationAlertService(gitListDataContext)
            };
        }

        public void Start()
        {
            backgroundServices.ForEach(service =>
            {
                service.Start();
            });
        }

        public void Stop()
        {
            backgroundServices.ForEach(service =>
            {
                service.Stop();
            });
        }
    }
}
