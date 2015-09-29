using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Entities.DataContext;
using GitList.Core.Interfaces;

namespace GitList.Core.Abstracts
{
    public abstract class ABackgroundService : IBackgroundService
    {
        public GitListDataContext GitListDataContext;

        public ABackgroundService(GitListDataContext gitListDataContext)
        {
            GitListDataContext = gitListDataContext;
        }

        public abstract void Start();

        public abstract void Stop();
    }
}
