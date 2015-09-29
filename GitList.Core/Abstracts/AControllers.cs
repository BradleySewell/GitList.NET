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
        public GitListDataContext gitListDataContext;

        public AControllers(GitListDataContext GitListDataContext)
        {
            gitListDataContext = GitListDataContext;
            Initialise();
            Start();
        }

        public abstract void Initialise();

        public abstract void Start();
    }
}
