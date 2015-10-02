using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Entities.Repository;
using GitList.Core.Interfaces;
using GitList.Core.Entities.DataContext;
using GitList.Core.Entities.Git.Console;
using GitList.Core.Entities.Configuration;


namespace GitList.Core.Entities.Controller
{
    public class ControllerInitialiser
    {
        private GitListDataContext GitListDataContext;
        public ConfigurationController ConfigurationController;
        public RepositoryController RepositoryController;
        public ConsoleController ConsoleController;

        public ControllerInitialiser(GitListDataContext gitListDataContext)
        {
            GitListDataContext = gitListDataContext;
        }

        public List<IControllers> InitialiseControllers()
        {
            ConfigurationController = new ConfigurationController(GitListDataContext);
            RepositoryController = new RepositoryController(GitListDataContext);
            ConsoleController = new ConsoleController(GitListDataContext);

            return new List<IControllers>()
            {
                ConfigurationController,
                RepositoryController,
                ConsoleController
            };
        }
    }
}
