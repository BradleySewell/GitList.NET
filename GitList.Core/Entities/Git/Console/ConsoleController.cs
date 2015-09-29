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
            gitListDataContext.ConsoleControl = new ConsoleControl.WPF.ConsoleControl();
        }

        public override void Start()
        {
            gitListDataContext.ConsoleControl.InitializeComponent();
            gitListDataContext.ConsoleControl.StartProcess(FileNameConstants.GIT_EXE_PATH, "--login -i");
            //this.console.StartProcess("cmd.exe", string.Empty);
            gitListDataContext.ConsoleControl.IsInputEnabled = false;
            gitListDataContext.ConsoleControl.ShowDiagnostics = false;
        }

        public void RunConsoleCommand()
        {
            if (string.IsNullOrEmpty(gitListDataContext.ConsoleCommand)) return;

            gitListDataContext.ConsoleControl.WriteInput(gitListDataContext.ConsoleCommand, Color.FromRgb(1, 100, 3), false);
            gitListDataContext.ConsoleCommand = String.Empty;
        }

        public void ChangeRepository(RepositoryItem selectionDetails)
        {
            if (selectionDetails != null)
            {
                gitListDataContext.ConsoleControl.ClearOutput();
                gitListDataContext.ConsoleControl.WriteInput(string.Format("cd \"{0}\"", selectionDetails.Path.ToLower()),
                    Color.FromRgb(1, 100, 3), false);
            }
        }
    }
}