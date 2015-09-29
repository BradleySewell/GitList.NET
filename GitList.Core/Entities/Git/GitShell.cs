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
    public class GitShell : IDisposable
    {
        private Process _shellProcess;
        private StreamWriter _inputStreamWriter;
        
        private string _repositoryPath;
        private string _gitExePath;

        public GitShell(string repositoryPath)
        {
            _gitExePath = FileNameConstants.GIT_EXE_PATH;
            _repositoryPath = repositoryPath;
        }

        private void InstantiateShell()
        {
            if (!File.Exists(_gitExePath))
            {
                throw new Exception(string.Format("Git path does not exist: {0}", _gitExePath));
            }

            if (_shellProcess == null)
            {
                _shellProcess = new Process();
            }

            _shellProcess.StartInfo = new ProcessStartInfo(_gitExePath,"--login -i")
            {
                WorkingDirectory = _repositoryPath,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Normal,
            };

            _shellProcess.Start();

            _shellProcess.OutputDataReceived += _shellProcess_OutputDataReceived;
            _shellProcess.ErrorDataReceived += _shellProcess_ErrorDataReceived;
            _shellProcess.Start();

            _shellProcess.BeginOutputReadLine();
            _shellProcess.BeginErrorReadLine();

        }

        private void _shellProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            
        }

        private void _shellProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {

        }



        public void ExecuteCommands(List<string> Commands)
        {
            InstantiateShell();

            try
            {
                _inputStreamWriter = _shellProcess.StandardInput;
                if (_inputStreamWriter.BaseStream.CanWrite)
                {
                    Commands.ForEach(c =>
                    {
                        _inputStreamWriter.WriteLine(c);
                    });

                    Dispose();
                }
            }
            catch (Exception objException)
            {
                // Log the exception
            }
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
