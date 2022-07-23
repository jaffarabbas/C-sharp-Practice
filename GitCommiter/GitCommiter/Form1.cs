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
using GitCommiterMethods;

namespace GitCommiter
{
    public partial class Form1 : Form
    {
        #region Form Methods

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize Form method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            FillList();
            WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Grid View Item Selecter Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectRepo(e);
            PopulateListBoxWithStageChanges();
        }

        /// <summary>
        /// Commit button Click Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnCommit_Click(object sender, EventArgs e)
        {
            CommitAndPushData();
        }


        #endregion

        #region Methods

        /// <summary>
        /// Fill DataGridView with Repository Path List using Commiter path List Methods
        /// </summary>
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

        /// <summary>
        /// Select Repository Local Path for Commit and Push
        /// </summary>
        /// <param name="e"></param>
        private void SelectRepo(DataGridViewCellEventArgs e)
        {
            repoPathLabel.Text = gvDataView.Rows[e.RowIndex].Cells[1].Value.ToString();
            fileCountForCommit.Text = Commiter.StageChanges(repoPathLabel.Text).Count.ToString();
        }

        /// <summary>
        /// Populate List box with Staged File Information
        /// </summary>
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

        /// <summary>
        /// Commit Staged data and Push to using Commiter Functions
        /// </summary>
        private void CommitAndPushData()
        {
            lbCommitDetails.Items.Clear();
            BindingSource bindingSource = new BindingSource();
            foreach (var item in Commiter.StageChanges(repoPathLabel.Text))
            {
                lbCommitDetails.Items.Clear();
                Commiter.CommitData(repoPathLabel.Text, item);
                foreach (var data in Commiter.commitResult)
                {
                    lbCommitDetails.Refresh();
                    lbCommitDetails.Items.Add(data);
                }
            }
            bindingSource.DataSource = Commiter.commitResult;
            lbCommitDetails.Items.Clear();
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
