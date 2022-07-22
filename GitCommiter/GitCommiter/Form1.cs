using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitCommiterMethodTester;

namespace GitCommiter
{
    public partial class Form1 : Form
    {
        public static List<string> commitResult = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillList();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectRepo(e);
            PopulateListBoxWithStageChanges();
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            foreach (var item in Commiter.StageChanges(@"J:\Github\C-sharp-Practice"))
            {
                //MessageBox.Show(item);
                Commiter.CommitData(@"J:\Github\C-sharp-Practice", item);
                foreach (var data in commitResult)
                {
                    MessageBox.Show(data);
                }
            }
            //lbCommitDetails.Items.Clear();
            //List<string> dataList = new List<string>();
            //BindingSource bindingSource = new BindingSource();

            //foreach (var item in Commiter.StageChanges(repoPathLabel.Text))
            //{
            //    Commiter.CommitData(repoPathLabel.Text, item);
            //    foreach (var data in Commiter.commitResult)
            //    {
            //        MessageBox.Show(data);
            //        lbCommitDetails.Items.Add(data);
            //        bindingSource.DataSource = Commiter.commitResult;
            //    }
            //}
        }

        #region Methods

        private void FillList()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Repo");
            foreach (var data in Commiter.GitPathList())
            {
                table.Rows.Add(new object[] { data.Key, data.Value });
            }
            gvDataView.DataSource = table;
            gvDataView.Columns[0].Width = 20;
        }

        private void SelectRepo(DataGridViewCellEventArgs e)
        {
            repoPathLabel.Text = gvDataView.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void PopulateListBoxWithStageChanges()
        {
            lbCommitDetails.Items.Clear();
            List<string> dataList = Commiter.StageChanges(repoPathLabel.Text);
            BindingSource bindingSource = new BindingSource();
            if (dataList.Count != 0)
            {
                foreach (var item in dataList)
                {
                    lbCommitDetails.Items.Add(item.ToString());
                }
            }
            else
            {
                dataList.Add("No Changes In Repository");
                lbCommitDetails.Items.Add(dataList.FirstOrDefault().ToString());
            }
            bindingSource.DataSource = dataList;
        }

        private static string CommitMessage(string message)
        {
            return "Commited File : " + (string)message.Split('/').Last() + " at " + DateTime.Now;
        }

        public static void CommitData(string path, string item)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            ///string gitCommand = 
            cmd.Start();
            cmd.StandardInput.WriteLine("cd " + path + " & git add " + item + " & git commit -m ¨" + CommitMessage(item) + "¨ ");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            commitResult.Add(cmd.StandardOutput.ReadToEnd());
        }


        #endregion

    }
}
