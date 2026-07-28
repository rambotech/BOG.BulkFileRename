namespace BOG.BulkFileRename
{
	partial class AboutBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			splitContainer1 = new SplitContainer();
			linkHomePage = new LinkLabel();
			pictureBox1 = new PictureBox();
			pgInfo = new PropertyGrid();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = DockStyle.Fill;
			splitContainer1.IsSplitterFixed = true;
			splitContainer1.Location = new Point(10, 10);
			splitContainer1.Margin = new Padding(4, 3, 4, 3);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(linkHomePage);
			splitContainer1.Panel1.Controls.Add(pictureBox1);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(pgInfo);
			splitContainer1.Size = new Size(877, 287);
			splitContainer1.SplitterDistance = 203;
			splitContainer1.SplitterWidth = 5;
			splitContainer1.TabIndex = 33;
			// 
			// linkHomePage
			// 
			linkHomePage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
			linkHomePage.AutoSize = true;
			linkHomePage.Location = new Point(19, 240);
			linkHomePage.Margin = new Padding(4, 0, 4, 0);
			linkHomePage.Name = "linkHomePage";
			linkHomePage.Size = new Size(164, 15);
			linkHomePage.TabIndex = 46;
			linkHomePage.TabStop = true;
			linkHomePage.Tag = "http://www.bitsofgenius.com";
			linkHomePage.Text = "http://www.bitsofgenius.com";
			// 
			// pictureBox1
			// 
			pictureBox1.Dock = DockStyle.Fill;
			pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
			pictureBox1.Location = new Point(0, 0);
			pictureBox1.Margin = new Padding(4, 3, 4, 3);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(203, 287);
			pictureBox1.TabIndex = 1;
			pictureBox1.TabStop = false;
			// 
			// pgInfo
			// 
			pgInfo.BackColor = SystemColors.Control;
			pgInfo.Dock = DockStyle.Fill;
			pgInfo.Location = new Point(0, 0);
			pgInfo.Margin = new Padding(4, 3, 4, 3);
			pgInfo.Name = "pgInfo";
			pgInfo.Size = new Size(669, 287);
			pgInfo.TabIndex = 44;
			pgInfo.ToolbarVisible = false;
			// 
			// AboutBox
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(897, 307);
			Controls.Add(splitContainer1);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Icon = (Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(913, 346);
			Name = "AboutBox";
			Padding = new Padding(10);
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "About";
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.LinkLabel linkHomePage;
		private System.Windows.Forms.PropertyGrid pgInfo;
	}
}