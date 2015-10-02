using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Entities.Configuration;

namespace GitList.Core.Constants
{
    public static class FileNameConstants
    {
        public const string SAVED_ROOT_DIRECTORIES_FILENAME = "RootDirectories.bin";
        public const string SAVED_CONFIGURATION_FILENAME = "Configuration.bin";


        public static string GIT_EXE_PATH
        {
            get
            {
                var configGitExePath = ConfigurationLoader.gitExePath;
                if (string.IsNullOrEmpty(configGitExePath))
                {
                    return string.Format(@"{0}\AppData\Local\Programs\Git\bin\sh.exe",
                        Environment.GetEnvironmentVariable("USERPROFILE"));
                }

                return configGitExePath;
            }
        }
    }
}
