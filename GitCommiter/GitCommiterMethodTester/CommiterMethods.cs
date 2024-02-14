using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitCommiterMethods
{
    public class Commiter
    {
        #region Attributes 

        private static string path = @"J:\\GitHub";
        private static Repository repo;
        private static Process cmd;
        private static int countCommitedFiles = 0;
        public static List<string> commitResult = new List<string>();

        #endregion

        #region Commiter Methods

        /// <summary>
        /// get list of directory path
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int,string> GitPathList()
        {
            int count = 0;
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] directoryInfos = directory.GetDirectories();
            Dictionary<int,string> data= new Dictionary<int,string>();
            foreach (DirectoryInfo directoryInfo in directoryInfos)
            {
                count++;
                data.Add(count,directoryInfo.FullName);
            }
            return data;
        }

        /// <summary>
        /// returns List of Files for Commits
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> StageChanges(string path)
        {
            try
            {
                repo = new Repository(path);
                RepositoryStatus status = repo.RetrieveStatus();
                //new files
                List<string> filePaths = status.Untracked.Select(mods => mods.FilePath).ToList();
                //updated files
                filePaths.AddRange(status.Modified.Select(mods => mods.FilePath).ToList());
                //deleted files
                filePaths.AddRange(status.Missing.Select(mods => mods.FilePath).ToList());
                //files that not modified
                filePaths.AddRange(status.Unaltered.Select(mods => mods.FilePath).ToList());
                return filePaths;
            }
            catch (Exception ex)
            {
                return new List<string> { "Exception:RepoActions:StageChanges " + ex.Message };
            }
        }

        /// <summary>
        /// execute shell commands and append output to global file list
        /// </summary>
        /// <param name="script"></param>
        /// <param name="message"></param>
        private static void ShellRunner(string script,string message)
        {
            try
            {
                cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.StandardInput.WriteLine(script);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                commitResult.Add(message);
            }
            catch(Exception error)
            {
                commitResult.Add(error.Message);
            }
        }

        /// <summary>
        /// commit files from satged list using Shell Runner
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        public static void CommitData(string path,string item)
        {
            countCommitedFiles++;
            string gitCommitCommand = "cd " + path + " & git add " + item + $"& git commit -m \"{CommitMessage(item)}\" ";
            string message = $"{countCommitedFiles} : {CommitMessage(item)}";
            ShellRunner(gitCommitCommand, message);
        }
      
        /// <summary>
        /// Push commited files using Shell Runner
        /// </summary>
        /// <param name="path"></param>
        public static void PushCommitData(string path)
        {
            string gitPushCommand = "cd " + path + " & git push -u origin main";
            string message = $"{countCommitedFiles} Files are pushed";
            ShellRunner(gitPushCommand,message);
        }

        /// <summary>
        /// Commit message generater
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string CommitMessage(string message)
        {
            return "Commited File : " + (string)message.Split('/').Last() + " at " + DateTime.Now;
        }

        #endregion

    }
}
