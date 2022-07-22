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

        public static void CommitData(string path,string item)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            string gitCommand = "cd " + path + " & git add " + item + "& git commit -m ¨" + CommitMessage(item) + "¨ ";
            cmd.Start();
            cmd.StandardInput.WriteLine(gitCommand);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            commitResult.Add(cmd.StandardOutput.ReadToEnd());
        }

        public static List<string> StageChanges(string path)
        {
            try
            {
                repo = new Repository(path);
                RepositoryStatus status = repo.RetrieveStatus();
                List<string> filePaths = status.Untracked.Select(mods => mods.FilePath).ToList();
                filePaths.AddRange(status.Modified.Select(mods => mods.FilePath).ToList());
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
            foreach (var item in Commiter.StageChanges(@"J:\Github\C-sharp-Practice"))
            {
                Commiter.CommitData(@"J:\Github\C-sharp-Practice",item);
                foreach (var data in Commiter.commitResult)
                {
                    Console.WriteLine(data); 
                }
            }
        }
    }
}
