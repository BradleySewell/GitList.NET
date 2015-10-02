using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Constants;

namespace GitList.Core.Entities.Git
{
    public class GitShellVisible : IDisposable
    {
        private Process _shellProcess;
        private StreamWriter _inputStreamWriter;
        
        private string _repositoryPath;
        private string _gitExePath;

        public GitShellVisible(string repositoryPath)
        {
            _gitExePath = FileNameConstants.GIT_EXE_PATH;
            _repositoryPath = repositoryPath;
        }

        public void InstantiateVisibleShell()
        {
            if (!File.Exists(_gitExePath))
            {
                throw new Exception(string.Format("Git path does not exist: {0}", _gitExePath));
            }

            if (_shellProcess == null)
            {
                _shellProcess = new Process();
            }

            _shellProcess.StartInfo = new ProcessStartInfo(_gitExePath, "--login -i")
            {
                WorkingDirectory = _repositoryPath,
                WindowStyle = ProcessWindowStyle.Normal,
            };

            _shellProcess.Start();
        }


        public void Dispose()
        {
            _inputStreamWriter.Close();
            _shellProcess.WaitForExit();
            _shellProcess.Close();

            _inputStreamWriter.Dispose();
            _shellProcess.Dispose();
        }
    }
}
