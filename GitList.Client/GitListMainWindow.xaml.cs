using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitList.Client.Startup;
using GitList.Background.Services;
using GitList.Core.Entities.Configuration;
using GitList.Core.Entities.Controller;
using GitList.Core.Entities.DataContext;
using GitList.Core.Entities.Repository;
using GitList.Core.Extensions;
using MahApps.Metro.Controls;
using System.Threading;
using GitList.Core.Entities.NotifyIcon;

namespace GitList.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GitListMainWindow : MetroWindow
    {
        private GitListDataContext gitListDataContext;
        public ControllerInitialiser Controllers;
        public BackgroundServices Services;

        public GitListMainWindow()
        {
            ThreadPool.SetMinThreads(100, 100);
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Startup();
        }

        private void Startup()
        {
            using (new Splash(this))
            {
                new ProcessHandler().Check();
                //new AutoStart().Register();

                gitListDataContext = new GitListDataContext();

                gitListDataContext.NotifyIconHandler = new NotifyIconHandler(Properties.Resources.Icon,
                    Properties.Resources.Icon_Working);
                gitListDataContext.NotifyIconHandler.OnClickEvent += () =>
                {
                    WindowState = System.Windows.WindowState.Maximized;
                    Topmost = true;
                    Show();
                    Topmost = false;
                };

                (gitListDataContext.Controllers = (Controllers = new ControllerInitialiser(gitListDataContext)))
                    .InitialiseControllers();
                (Services = new BackgroundServices(gitListDataContext)).Start();

                this.DataContext = gitListDataContext;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            gitListDataContext.NotifyIconHandler.ShowAlert("GitList is minimized...", TimeSpan.FromSeconds(30));
            gitListDataContext.Controllers.RepositoryController.SaveRootDirectoryitems();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    break;
                case WindowState.Minimized:
                    gitListDataContext.NotifyIconHandler.ShowAlert("GitList is minimized...", TimeSpan.FromSeconds(30));
                    break;
                case WindowState.Normal:

                    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                gitListDataContext.Controllers.RepositoryController.RefreshAll();
            }
        }

        private void tbRootDirectoryPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                gitListDataContext.Controllers.RepositoryController.AddRootDirectoryPath();
        }

        private void btnAddRootDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.RepositoryController.AddRootDirectoryPath();
        }

        private void btnRemoveRootDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            var selection = (Button) sender;
            var selectionDetails = (RootDirectoryItem) selection.DataContext;

            gitListDataContext.Controllers.RepositoryController.RemoveRootDirectoryItem(selectionDetails);
        }

        private void btnRefreshRootDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            var selection = (Button) sender;
            var selectionDetails = (RootDirectoryItem) selection.DataContext;

            gitListDataContext.Controllers.RepositoryController.RefreshRepositories(selectionDetails, false);
        }

        private void btnSelectRootDirectoryFolder_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.SelectedPath))
            {
                gitListDataContext.RootDirectoryPath = dialog.SelectedPath;
            }
        }


        private void gbRepositoryItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var selection = (GroupBox) sender;
            var selectionDetails = (RepositoryItem) selection.DataContext;
            gitListDataContext.SelectedRepositoryItem = selectionDetails;

            gitListDataContext.Controllers.ConsoleController.ChangeRepository(selectionDetails);
        }


        private void btnChangeBranch_OnClick(object sender, RoutedEventArgs e)
        {
            var selection = (Button) sender;
            var selectionDetails = (RepositoryItem) selection.DataContext;

            gitListDataContext.Controllers.RepositoryController.ChangeRepositoryBranch(selectionDetails);
        }


        private void MenuItem_Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.RepositoryController.RefreshAll();
        }

        private void tbRootDirectoryPath_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var selection = (TextBlock) sender;
            var selectionDetails = (RootDirectoryItem) selection.DataContext;
            selectionDetails.Path.OpenPathInExplorer();
        }

        private void gbRepositoryItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 1 && e.LeftButton == MouseButtonState.Pressed)
            {
                var selection = (GroupBox) sender;
                var selectionDetails = (RepositoryItem) selection.DataContext;
                selectionDetails.Path.OpenPathInExplorer();
            }
        }

        private void tbRepositoryFilePath_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var selection = (TextBlock) sender;
            var selectionDetails = (RepositoryItem) selection.DataContext;
            selectionDetails.Path.OpenPathInExplorer();
        }

        private void btnRootDirectoryVisibility_OnClick(object sender, RoutedEventArgs e)
        {
            var selection = (Button) sender;
            var selectionDetails = (RootDirectoryItem) selection.DataContext;

            gitListDataContext.Controllers.RepositoryController.CollapseRootDirectory(selectionDetails);
        }

        private void btnChangeRootBranch_OnClick(object sender, RoutedEventArgs e)
        {
            var selection = (Button) sender;
            var selectionDetails = (RootDirectoryItem) selection.DataContext;

            gitListDataContext.Controllers.RepositoryController.ChangeRootBranch(selectionDetails);
        }

        private void btnFetchAndResetRootBranch_OnClick(object sender, RoutedEventArgs e)
        {
            var selection = (Button) sender;
            var selectionDetails = (RootDirectoryItem) selection.DataContext;

            gitListDataContext.Controllers.RepositoryController.FetchAndResetRootBranch(selectionDetails);
        }

        private void btnCloseSelectedRepositoryItem_OnClick(object sender, RoutedEventArgs e)
        {
            gitListDataContext.SelectedRepositoryItem = null;
        }





        private void btnSubmitCommand_OnClick(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.ConsoleController.RunConsoleCommand();
        }

        private void tbConsoleCommand_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                gitListDataContext.Controllers.ConsoleController.RunConsoleCommand();
            }
        }


        private void btnClearConsole_Click(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.ConsoleController.Clear();
        }

        private void btnPopOutConsole_Click(object sender, RoutedEventArgs e)
        {
            var selection = (Button)sender;
            var selectionDetails = (RepositoryItem)selection.DataContext;
            gitListDataContext.Controllers.ConsoleController.Popout(selectionDetails);
        }



        private void miAutoStart_Click(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.ConfigurationController.SavConfiguration();
        }

        private void miMinimizeOnClose_Click(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.ConfigurationController.SavConfiguration();
        }

        private void miRefreshPeriodically_Click(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.ConfigurationController.SavConfiguration();
        }

        private void miRefreshDetection_Click(object sender, RoutedEventArgs e)
        {
            gitListDataContext.Controllers.ConfigurationController.SavConfiguration();
        }



    }

}
