using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Abstracts;
using GitList.Core.Constants;
using GitList.Core.Entities.DataContext;
using System.Windows.Media;
using GitList.Core.Entities.Repository;

namespace GitList.Core.Entities.Git.Console
{
    public class ConsoleController : AControllers
    {
        public ConsoleController(GitListDataContext gitListDataContext)
            : base(gitListDataContext)
        {
        }

        public override void Initialise()
        {
            GitListDataContext.ConsoleControl = new ConsoleControl.WPF.ConsoleControl();
        }

        public override void Start()
        {
            GitListDataContext.ConsoleControl.InitializeComponent();
            GitListDataContext.ConsoleControl.StartProcess(FileNameConstants.GIT_EXE_PATH, "--login -i");
            //this.console.StartProcess("cmd.exe", string.Empty);
            GitListDataContext.ConsoleControl.IsInputEnabled = false;
            GitListDataContext.ConsoleControl.ShowDiagnostics = false;
        }

        public void RunConsoleCommand()
        {
            if (string.IsNullOrEmpty(GitListDataContext.ConsoleCommand)) return;

            GitListDataContext.ConsoleControl.WriteInput(GitListDataContext.ConsoleCommand, Color.FromRgb(1, 100, 3), false);
            GitListDataContext.ConsoleCommand = String.Empty;
        }

        public void ChangeRepository(RepositoryItem selectionDetails)
        {
            if (selectionDetails != null)
            {
                GitListDataContext.ConsoleControl.ClearOutput();
                GitListDataContext.ConsoleControl.WriteInput(string.Format("cd \"{0}\"", selectionDetails.Path.ToLower()),
                    Color.FromRgb(1, 100, 3), false);
            }
        }

        public void Clear()
        {
            GitListDataContext.ConsoleControl.ClearOutput();
        }

        public void Popout(RepositoryItem selectionDetails)
        {
            if (selectionDetails != null)
            {
                new GitShellVisible(selectionDetails.Path).InstantiateVisibleShell();
            }
        }
    }
}