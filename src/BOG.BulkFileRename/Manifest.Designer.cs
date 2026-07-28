namespace BOG.BulkFileRename
{
    partial class Manifest
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manifest));
			lblFolder = new Label();
			btnCancel = new Button();
			btnRename = new Button();
			lblInfo = new Label();
			dgvRename = new DataGridView();
			ch1_Select = new DataGridViewCheckBoxColumn();
			ch2_Overwrite = new DataGridViewCheckBoxColumn();
			ch3_Original = new DataGridViewTextBoxColumn();
			ch4_New = new DataGridViewTextBoxColumn();
			ch5_Result = new DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)dgvRename).BeginInit();
			SuspendLayout();
			// 
			// lblFolder
			// 
			lblFolder.AutoSize = true;
			lblFolder.Location = new Point(14, 10);
			lblFolder.Margin = new Padding(4, 0, 4, 0);
			lblFolder.Name = "lblFolder";
			lblFolder.Size = new Size(46, 15);
			lblFolder.TabIndex = 0;
			lblFolder.Text = "{folder}";
			// 
			// btnCancel
			// 
			btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnCancel.Location = new Point(447, 404);
			btnCancel.Margin = new Padding(4, 3, 4, 3);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(88, 27);
			btnCancel.TabIndex = 3;
			btnCancel.Text = "&Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += btnCancel_Click;
			// 
			// btnRename
			// 
			btnRename.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnRename.Location = new Point(552, 396);
			btnRename.Margin = new Padding(4, 3, 4, 3);
			btnRename.Name = "btnRename";
			btnRename.Size = new Size(100, 42);
			btnRename.TabIndex = 4;
			btnRename.Text = "&Rename";
			btnRename.UseVisualStyleBackColor = true;
			btnRename.Click += btnRename_Click;
			// 
			// lblInfo
			// 
			lblInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			lblInfo.AutoSize = true;
			lblInfo.Location = new Point(14, 410);
			lblInfo.Margin = new Padding(4, 0, 4, 0);
			lblInfo.Name = "lblInfo";
			lblInfo.Size = new Size(378, 15);
			lblInfo.TabIndex = 2;
			lblInfo.Text = "Original names will change to the new names with the Rename button";
			// 
			// dgvRename
			// 
			dgvRename.AllowUserToAddRows = false;
			dgvRename.AllowUserToDeleteRows = false;
			dgvRename.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			dgvRename.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			dgvRename.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			dgvRename.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgvRename.Columns.AddRange(new DataGridViewColumn[] { ch1_Select, ch2_Overwrite, ch3_Original, ch4_New, ch5_Result });
			dgvRename.Location = new Point(15, 30);
			dgvRename.Margin = new Padding(4, 3, 4, 3);
			dgvRename.Name = "dgvRename";
			dgvRename.Size = new Size(637, 359);
			dgvRename.TabIndex = 5;
			// 
			// ch1_Select
			// 
			ch1_Select.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
			ch1_Select.HeaderText = "Include?";
			ch1_Select.Name = "ch1_Select";
			ch1_Select.Width = 57;
			// 
			// ch2_Overwrite
			// 
			ch2_Overwrite.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
			ch2_Overwrite.HeaderText = "Overwrite?";
			ch2_Overwrite.Name = "ch2_Overwrite";
			ch2_Overwrite.Width = 69;
			// 
			// ch3_Original
			// 
			ch3_Original.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			ch3_Original.HeaderText = "Original Name";
			ch3_Original.Name = "ch3_Original";
			ch3_Original.Width = 109;
			// 
			// ch4_New
			// 
			ch4_New.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			ch4_New.HeaderText = "New name";
			ch4_New.Name = "ch4_New";
			ch4_New.Width = 89;
			// 
			// ch5_Result
			// 
			ch5_Result.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			ch5_Result.HeaderText = "Result";
			ch5_Result.Name = "ch5_Result";
			ch5_Result.Width = 64;
			// 
			// Manifest
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(666, 444);
			Controls.Add(dgvRename);
			Controls.Add(lblInfo);
			Controls.Add(btnRename);
			Controls.Add(btnCancel);
			Controls.Add(lblFolder);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(4, 3, 4, 3);
			MinimumSize = new Size(673, 477);
			Name = "Manifest";
			Text = "Manifest";
			((System.ComponentModel.ISupportInitialize)dgvRename).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.DataGridView dgvRename;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ch1_Select;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ch2_Overwrite;
        private System.Windows.Forms.DataGridViewTextBoxColumn ch3_Original;
        private System.Windows.Forms.DataGridViewTextBoxColumn ch4_New;
        private System.Windows.Forms.DataGridViewTextBoxColumn ch5_Result;
    }
}