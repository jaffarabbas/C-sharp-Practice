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
            this.gvDataView = new System.Windows.Forms.DataGridView();
            this.headingLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.repoPathLabel = new System.Windows.Forms.Label();
            this.btnCommit = new System.Windows.Forms.Button();
            this.lbCommitDetails = new System.Windows.Forms.ListBox();
            this.fileCountForCommit = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gvDataView)).BeginInit();
            this.SuspendLayout();
            // 
            // gvDataView
            // 
            this.gvDataView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvDataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvDataView.Location = new System.Drawing.Point(432, 12);
            this.gvDataView.Name = "gvDataView";
            this.gvDataView.ReadOnly = true;
            this.gvDataView.RowHeadersVisible = false;
            this.gvDataView.Size = new System.Drawing.Size(356, 426);
            this.gvDataView.TabIndex = 0;
            this.gvDataView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // headingLabel
            // 
            this.headingLabel.AutoSize = true;
            this.headingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headingLabel.Location = new System.Drawing.Point(22, 12);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Size = new System.Drawing.Size(166, 29);
            this.headingLabel.TabIndex = 1;
            this.headingLabel.Text = "Git Commiter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Repository : ";
            // 
            // repoPathLabel
            // 
            this.repoPathLabel.AutoSize = true;
            this.repoPathLabel.Location = new System.Drawing.Point(112, 60);
            this.repoPathLabel.Name = "repoPathLabel";
            this.repoPathLabel.Size = new System.Drawing.Size(0, 13);
            this.repoPathLabel.TabIndex = 3;
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(30, 383);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(75, 23);
            this.btnCommit.TabIndex = 5;
            this.btnCommit.Text = "Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // lbCommitDetails
            // 
            this.lbCommitDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCommitDetails.FormattingEnabled = true;
            this.lbCommitDetails.HorizontalScrollbar = true;
            this.lbCommitDetails.ItemHeight = 15;
            this.lbCommitDetails.Location = new System.Drawing.Point(30, 89);
            this.lbCommitDetails.Name = "lbCommitDetails";
            this.lbCommitDetails.Size = new System.Drawing.Size(387, 274);
            this.lbCommitDetails.TabIndex = 6;
            // 
            // fileCountForCommit
            // 
            this.fileCountForCommit.AutoSize = true;
            this.fileCountForCommit.Location = new System.Drawing.Point(408, 60);
            this.fileCountForCommit.Name = "fileCountForCommit";
            this.fileCountForCommit.Size = new System.Drawing.Size(0, 13);
            this.fileCountForCommit.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(310, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Commit Count: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fileCountForCommit);
            this.Controls.Add(this.lbCommitDetails);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.repoPathLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.headingLabel);
            this.Controls.Add(this.gvDataView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvDataView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gvDataView;
        private System.Windows.Forms.Label headingLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label repoPathLabel;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.ListBox lbCommitDetails;
        private System.Windows.Forms.Label fileCountForCommit;
        private System.Windows.Forms.Label label3;
    }
}

