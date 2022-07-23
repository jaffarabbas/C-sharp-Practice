namespace GitCommiter
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbCommitDetails = new System.Windows.Forms.ListBox();
            this.gvDataView = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.fileCountForCommit = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.repoPathLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.headingLabel = new System.Windows.Forms.Label();
            this.btnCommit = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvDataView)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel7
            // 
            this.panel7.AutoSize = true;
            this.panel7.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(0, 450);
            this.panel7.TabIndex = 10;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbCommitDetails);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 89);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(10);
            this.panel3.Size = new System.Drawing.Size(485, 281);
            this.panel3.TabIndex = 9;
            // 
            // lbCommitDetails
            // 
            this.lbCommitDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCommitDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCommitDetails.FormattingEnabled = true;
            this.lbCommitDetails.HorizontalScrollbar = true;
            this.lbCommitDetails.ItemHeight = 15;
            this.lbCommitDetails.Location = new System.Drawing.Point(10, 10);
            this.lbCommitDetails.Margin = new System.Windows.Forms.Padding(10);
            this.lbCommitDetails.Name = "lbCommitDetails";
            this.lbCommitDetails.Size = new System.Drawing.Size(465, 261);
            this.lbCommitDetails.TabIndex = 9;
            // 
            // gvDataView
            // 
            this.gvDataView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvDataView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gvDataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvDataView.Dock = System.Windows.Forms.DockStyle.Right;
            this.gvDataView.Location = new System.Drawing.Point(485, 0);
            this.gvDataView.Name = "gvDataView";
            this.gvDataView.ReadOnly = true;
            this.gvDataView.RowHeadersVisible = false;
            this.gvDataView.Size = new System.Drawing.Size(315, 450);
            this.gvDataView.TabIndex = 0;
            this.gvDataView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(485, 89);
            this.panel1.TabIndex = 7;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label3);
            this.panel6.Controls.Add(this.fileCountForCommit);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(351, 44);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(134, 45);
            this.panel6.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Commit Count: ";
            // 
            // fileCountForCommit
            // 
            this.fileCountForCommit.AutoSize = true;
            this.fileCountForCommit.Location = new System.Drawing.Point(101, 18);
            this.fileCountForCommit.Name = "fileCountForCommit";
            this.fileCountForCommit.Size = new System.Drawing.Size(0, 13);
            this.fileCountForCommit.TabIndex = 14;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.repoPathLabel);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 44);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(485, 45);
            this.panel5.TabIndex = 15;
            // 
            // repoPathLabel
            // 
            this.repoPathLabel.AutoSize = true;
            this.repoPathLabel.Location = new System.Drawing.Point(96, 18);
            this.repoPathLabel.Name = "repoPathLabel";
            this.repoPathLabel.Size = new System.Drawing.Size(0, 13);
            this.repoPathLabel.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Repository : ";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.headingLabel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(485, 44);
            this.panel4.TabIndex = 14;
            // 
            // headingLabel
            // 
            this.headingLabel.AutoSize = true;
            this.headingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headingLabel.Location = new System.Drawing.Point(12, 9);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Size = new System.Drawing.Size(166, 29);
            this.headingLabel.TabIndex = 10;
            this.headingLabel.Text = "Git Commiter";
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(18, 21);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(101, 39);
            this.btnCommit.TabIndex = 6;
            this.btnCommit.Text = "Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCommit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 370);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(485, 80);
            this.panel2.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gvDataView);
            this.Controls.Add(this.panel7);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Git Commiter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvDataView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView gvDataView;
        private System.Windows.Forms.ListBox lbCommitDetails;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label fileCountForCommit;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label repoPathLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label headingLabel;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.Panel panel2;
    }
}

