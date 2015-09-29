using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Entities.Repository;
using GitList.Core.Interfaces;
using GitList.Core.Entities.DataContext;
using GitList.Core.Entities.Git.Console;


namespace GitList.Core.Entities.Controller
{
    public class ControllerInitialiser
    {
        private GitListDataContext GitListDataContext;
        public RepositoryController RepositoryController;
        public ConsoleController ConsoleController;

        public ControllerInitialiser(GitListDataContext gitListDataContext)
        {
            GitListDataContext = gitListDataContext;
        }

        public List<IControllers> InitialiseControllers()
        {
            RepositoryController = new RepositoryController(GitListDataContext);
            ConsoleController = new ConsoleController(GitListDataContext);

            return new List<IControllers>()
            {
                RepositoryController,
                ConsoleController
            };
        }
    }
}
