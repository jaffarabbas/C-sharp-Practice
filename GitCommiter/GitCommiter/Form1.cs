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
            CommitAndPushData();
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
            fileCountForCommit.Text = Commiter.StageChanges(repoPathLabel.Text).Count.ToString();
        }

        private void PopulateListBoxWithStageChanges()
        {
            lbCommitDetails.Items.Clear();
            List<string> dataList = Commiter.StageChanges(repoPathLabel.Text);
            BindingSource bindingSource = new BindingSource();
            int count = 0;
            if (dataList.Count != 0)
            {
                foreach (var item in dataList)
                {
                    count++;
                    lbCommitDetails.Items.Add(count + " : " +item.ToString());
                }
            }
            else
            {
                dataList.Add("No Changes In Repository");
                lbCommitDetails.Items.Add(dataList.FirstOrDefault().ToString());
            }
            bindingSource.DataSource = dataList;
        }


        private void CommitAndPushData()
        {
            lbCommitDetails.Items.Clear();
            BindingSource bindingSource = new BindingSource();
            foreach (var item in Commiter.StageChanges(repoPathLabel.Text))
            {
                Commiter.CommitData(repoPathLabel.Text, item);
                foreach (var data in Commiter.commitResult)
                {
                    lbCommitDetails.Items.Add(data);
                }
                bindingSource.DataSource = Commiter.commitResult;
            }
            lbCommitDetails.Items.Clear();
            lbCommitDetails.Refresh();
            Commiter.PushCommitData(repoPathLabel.Text);

            foreach (var data in Commiter.commitResult)
            {
                lbCommitDetails.Items.Add(data);
                bindingSource.DataSource = Commiter.commitResult;
            }
        }
        #endregion

    }
}
