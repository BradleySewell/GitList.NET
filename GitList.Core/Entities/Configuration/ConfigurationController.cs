using GitList.Core.Abstracts;
using GitList.Core.Constants;
using GitList.Core.Entities.DataContext;
using GitList.Core.Entities.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitList.Core.Entities.Configuration
{
    public class ConfigurationController : AControllers
    {
        public ConfigurationController(GitListDataContext gitListDataContext)
            : base(gitListDataContext)
        {
        }

        public override void Initialise()
        {
            GitListDataContext.Configuration = new ConfigurationItem();
        }

        public override void Start()
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var deSerializedConfiguration = BinarySerializer.LoadFile(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, Constants.FileNameConstants.SAVED_CONFIGURATION_FILENAME));
            if (deSerializedConfiguration == null) return;

            var configuration = (ConfigurationItem)deSerializedConfiguration;

            GitListDataContext.Configuration = configuration;
        }

        public void SavConfiguration()
        {
            BinarySerializer.SaveFile(GitListDataContext.Configuration, string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, FileNameConstants.SAVED_CONFIGURATION_FILENAME));
        }

    }
}
