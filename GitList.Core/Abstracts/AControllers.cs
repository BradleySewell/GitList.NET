using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Entities.DataContext;
using GitList.Core.Interfaces;

namespace GitList.Core.Abstracts
{
    public abstract class AControllers : IControllers
    {
        public GitListDataContext GitListDataContext;

        public AControllers(GitListDataContext gitListDataContext)
        {
            GitListDataContext = gitListDataContext;
            Initialise();
            Start();
        }

        public abstract void Initialise();

        public abstract void Start();
    }
}
