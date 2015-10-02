using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using GitList.Core.Constants;
using GitList.Core.Entities.Repository;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitList.Core.Entities.Git
{
    
    public class GitManager
    {
        public ObservableCollection<RepositoryItem> FindRepositories(string rootDirectoryPath)
        {
            var repositories = new ObservableCollection<RepositoryItem>();

            var rootDir = new DirectoryInfo(rootDirectoryPath);
            if (rootDir.Exists)
            {
                foreach (var directory in rootDir.GetDirectories())
                {
                    //if (directory.Exists && directory.GetDirectories(".git").Any())
                    if (directory.Exists && LibGit2Sharp.Repository.IsValid(directory.FullName))
                    {
                        repositories.Add(new RepositoryItem()
                        {
                            Name = directory.Name, 
                            Path = directory.FullName, 
                            Exists = true,
                            Modified = null,
                            Message =  null,
                            CurrentBranch = null,
                            InProgress = true,
                            Branches = new ObservableCollection<string>(),
                            Changes = new ObservableCollection<string>(),
                            CommandHistory = new ObservableCollection<string>(),

                        });
                    }
                }
            }

            return repositories;
        }

        public bool IsValidRepository(RepositoryItem repository)
        {
            return LibGit2Sharp.Repository.IsValid(repository.Path);
        }

        public List<string> Branches(RepositoryItem repository)
        {
            var repo = new LibGit2Sharp.Repository(repository.Path);
            return repo.Branches.Select(x => x.Name).Distinct().ToList();
        }

        private LibGit2Sharp.Repository CurrentBranch(RepositoryItem repository)
        {
            return new LibGit2Sharp.Repository(repository.Path);
        }

        public string CurrentBranchName(RepositoryItem repository)
        {
            return CurrentBranch(repository).Head.Name;
        }



        public void ChangeBranch(RepositoryItem repository, string BranchName)
        {
            var repo = new LibGit2Sharp.Repository(repository.Path);
            
            var branches = repo.Branches.Where(b => b.Name == BranchName).ToList();
            if (branches.Any())
            {
                var localBranch = branches.FirstOrDefault(x => !x.IsRemote);
                if (localBranch != null)
                {
                    repo.Checkout(localBranch, new CheckoutOptions(), new Signature(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now));
                }
                else
                {
                    throw new Exception("You must checkout manually the first time");
                    
                    //COMMENTED THIS OUT AS IT CREATES DUPLICATE REMOTE BRANCHES, For instance R15_Release will duplicate as origin/R15_Release.
                    //Current work around is to get them to check out the branch manually the first time around.
                    
                    //var remoteBranch = branches.FirstOrDefault(x => x.IsRemote);
                    //if (remoteBranch != null)
                    //{
                    //    var newLocalBranch = repo.CreateBranch(BranchName, remoteBranch.Tip);
                    //    newLocalBranch = repo.Branches.Update(newLocalBranch,
                    //        b =>
                    //        {
                    //            b.TrackedBranch = remoteBranch.CanonicalName;
                    //        });
                    //    repo.Checkout(newLocalBranch, new CheckoutOptions(), new Signature(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now));
                    //}
                }
            }
        }

        public List<string> Changes(RepositoryItem repository)
        {
            var repo = new LibGit2Sharp.Repository(repository.Path);

            return repo.RetrieveStatus().Where(x => x.State != FileStatus.Ignored).Select(x => x.FilePath).ToList();
        }

        public int Aheadby(RepositoryItem repository)
        {
            var repo = new LibGit2Sharp.Repository(repository.Path);
            var aheadBy = repo.Head.TrackingDetails.AheadBy ?? 0;
            return aheadBy;
        }

        public int BehindBy(RepositoryItem repository)
        {
            var repo = new LibGit2Sharp.Repository(repository.Path);
            var behindBy = repo.Head.TrackingDetails.BehindBy ?? 0;
            return behindBy;
        }

        //public void Reset(RepositoryItem repository)
        //{
        //    try
        //    {
        //        var repo = new LibGit2Sharp.Repository(repository.Path);
        //        repo.RemoveUntrackedFiles();
        //        repo.Reset(ResetMode.Hard);
        //    }
        //    catch(Exception e)
        //    {
        //    }
        //}



        public void FetchAndReset(RepositoryItem repository)
        {
            var repo = new LibGit2Sharp.Repository(repository.Path);
            var currentBranch = CurrentBranch(repository);

            //foreach (Remote remote in repo.Network.Remotes)
            //{
            //    //FetchOptions options = new FetchOptions();
            //    //options.CredentialsProvider = new CredentialsHandler(
            //    //    (url, usernameFromUrl, types) =>
            //    //        new UsernamePasswordCredentials()
            //    //        {
            //    //            Username = "user",
            //    //            Password = "password"
            //    //        });
            //    //repo.Network.Pull(new Signature(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now), new PullOptions());
            //}


            List<string> Commands = new List<string>()
            {
                "git fetch --all --prune",
                //below line works ok if you are on master, not if you are on a different branch.
                //string.Format("git reset --hard {0}", string.Format("{0}/{1}",currentBranch.Head.Remote.Name, currentBranch.Head.Name)),
                //"git reset --hard",
                //string.Format("git reset --hard {0}", currentBranch.Head.TrackedBranch.UpstreamBranchCanonicalName),
                "git reset --hard HEAD^",
                "git clean -d -f -f",
                "git pull"
            };

            RunGitCommands(repository.Path, Commands);
        }


        private void RunGitCommands(string RepositoryPath, List<string> Commands)
        {
            new GitShellHidden(RepositoryPath).ExecuteCommands(Commands);
        }

        public List<string> CommitLog(RepositoryItem repository)
        {
            var repo = new LibGit2Sharp.Repository(repository.Path);

            var commitLog = new List<string>();
            
            repo.Commits.ToList().ForEach(commit =>
            {
                commitLog.Add(string.Format("{0} - {1} : {2}", commit.Author.When.LocalDateTime, commit.Author.Name, commit.Message));
            });
        
            return commitLog;
        }
    }
}
