using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitCommiterMethodTester
{
    public class Commiter
    {
        private static string path = @"J:\Github";
        private static Repository repo;
        private static Process cmd;
        private static int countCommitedFiles = 0;
        public static List<string> commitResult = new List<string>();

        //get list of directory path
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

        public static void CommitData(string path,string item)
        {
            countCommitedFiles++;
            string gitCommitCommand = "cd " + path + " & git add " + item + $"& git commit -m \"{CommitMessage(item)}\" ";
            string message = $"{countCommitedFiles} : {CommitMessage(item)}";
            ShellRunner(gitCommitCommand, message);
        }

        public static void PushCommitData(string path)
        {
            string gitPushCommand = "cd " + path + " & git push -u origin";
            string message = $"{countCommitedFiles} Files are pushed";
            ShellRunner(gitPushCommand,message);
        }

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
                return filePaths;
            }
            catch (Exception ex)
            {
                return new List<string> { "Exception:RepoActions:StageChanges " + ex.Message };
            }
        }

        private static string CommitMessage(string message)
        {
            return "Commited File : " + (string)message.Split('/').Last() + " at " + DateTime.Now;
        }


        static void Main(string[] args)
        {
            //Commiter.CommitData(@"J:\Github\C-sharp-Practice");
            foreach (var item in StageChanges(@"J:\Github\C-sharp-Practice"))
            {
                CommitData(@"J:\Github\C-sharp-Practice",item);
                foreach (var data in commitResult)
                {
                   MessageBox.Show(data); 
                }
            }
        }
    }
}
