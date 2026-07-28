using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BOG.SwissArmyKnife;
using BOG.SwissArmyKnife.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BOG.BulkFileRename
{
	public partial class MainForm : Form
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

		private SettingsDictionary AppSettings = new SettingsDictionary();
		private ToolStripMenuItem createWindowsExplorerShellExtensionToolStripMenuItem;
		private ToolStripMenuItem removeWindowsExplorerShellExtensionToolStripMenuItem;
		bool LaunchedWithParameter = false;

		public MainForm(string[] args)
		{
			InitializeComponent();
			AppSettings.ConfigurationFile = Path.Combine(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Bits of Genius\Bulk File Rename"),
				"usersettings.xml");
			if (!Directory.Exists(Path.GetDirectoryName(AppSettings.ConfigurationFile)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(AppSettings.ConfigurationFile));
			}
			if (File.Exists(AppSettings.ConfigurationFile) == false)
			{
				AppSettings.SetSetting("LastFolder", @"C:\");
				AppSettings.SaveSettings();
			}
			AppSettings.LoadSettings();
			this.txtFolder.Text = AppSettings.GetSetting("LastFolder", @"C:\").ToString();
			if (args.Length > 0)
			{
				LaunchedWithParameter = true;
				if (!Directory.Exists(args[0]))
				{
					MessageBox.Show("Folder not found.  Use the select option to set the folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				this.txtFolder.Text = args[0];
			}
			if (Directory.Exists(this.txtFolder.Text))
			{
				LoadFiles();
			}
			Adjust_Controls();
		}

		private void LoadFiles()
		{
			this.lbxFileManifest.Items.Clear();
			SortedList Temp = new SortedList();
			foreach (string f in Directory.GetFiles(this.txtFolder.Text, "*.*", SearchOption.TopDirectoryOnly))
			{
				Temp.Add(Path.GetFileName(f), string.Empty);
			}
			for (int Index = 0; Index < Temp.Count; Index++)
			{
				this.lbxFileManifest.Items.Add(Temp.GetKey(Index));
			}
			this.lbxFileManifest.Visible = true;
		}

		private void UserChangeFolder()
		{
			FolderBrowserDialog f = new FolderBrowserDialog();
			f.SelectedPath = @"C:\";
			if (Directory.Exists(this.txtFolder.Text))
			{
				f.SelectedPath = this.txtFolder.Text;
			}
			f.Description = "Select folder with files to rename";
			if (f.ShowDialog() == DialogResult.OK)
			{
				this.txtFolder.Text = f.SelectedPath;
				LoadFiles();
				if (LaunchedWithParameter == false)
				{
					AppSettings.SetSetting("LastFolder", this.txtFolder.Text);
					AppSettings.SaveSettings();
				}
			}
		}

		private string Build_New_Name(string originalName)
		{
			string Result = originalName;

			if (this.rbReplaceText.Checked || this.rbRemoveText.Checked)
			{
				if (this.txtFind.Text.IndexOf("?") == -1 && this.txtReplace.Text.IndexOf("?") == -1)
				{
					Result = StringEx.ReplaceNoCase(
							originalName,
							this.txtFind.Text,
							this.rbRemoveText.Checked ? string.Empty : this.txtReplace.Text,
							this.chkIgnoreCaseDuringFind.Checked
						);
				}
				else if (this.rbRemoveText.Checked)  // accounts for wildcards in the find string.
				{
					int FoundAt = StringEx.WildcardIndexOfAnyString(originalName, this.txtFind.Text, 0, this.chkIgnoreCaseDuringFind.Checked, '?');
					if (FoundAt >= 0)
					{
						Result = originalName.Substring(0, FoundAt) + (
							(originalName.Length <= FoundAt + this.txtFind.Text.Length) ?
								string.Empty :
								originalName.Substring(FoundAt + this.txtFind.Text.Length));
					}
				}
				else  // The wild, wildcards...
				{
					Dictionary<int, int> FindWildcards = new Dictionary<int, int>();
					Dictionary<int, int> ReplaceWildcards = new Dictionary<int, int>();
					Dictionary<int, int> MusicalWildcards = new Dictionary<int, int>();
					StringBuilder find = new StringBuilder();
					StringBuilder replace = new StringBuilder();
					int Index = 0;
					const string Offsets = "abcdefghijklmnopqrstuvwxyz";

					while (true)
					{
						bool WorkDone = true;
						if (Index < this.txtFind.Text.Length)
						{
							WorkDone = false;
							if (this.txtFind.Text[Index] == '?')
							{
								FindWildcards.Add(FindWildcards.Count, Index);
							}
						}
						if (Index < this.txtReplace.Text.Length)
						{
							WorkDone = false;
							if (this.txtReplace.Text[Index] == '?')
							{
								ReplaceWildcards.Add(Index, ReplaceWildcards.Count);
							}
						}
						if (Index < this.txtWildcardMask.Text.Length)
						{
							WorkDone = false;
							int NewIndex = Offsets.IndexOf(this.txtWildcardMask.Text[Index].ToString().ToLower());
							if (NewIndex >= 0)
							{
								MusicalWildcards.Add(MusicalWildcards.Count, NewIndex);
							}
							else
							{
								throw new Exception(string.Format("Invalid wildcard character at offset {0}: '{1}'", Index, this.txtWildcardMask.Text[Index]));
							}
						}
						if (WorkDone)
						{
							break;
						}
						Index++;
					}
					while (MusicalWildcards.Count < ReplaceWildcards.Count)
					{
						MusicalWildcards.Add(MusicalWildcards.Count, MusicalWildcards.Count);
					}
					for (int IndexCheck = 0; IndexCheck < MusicalWildcards.Count; IndexCheck++)
					{
						if (MusicalWildcards[IndexCheck] >= FindWildcards.Count)
						{
							throw new Exception("One or more characters in the wildcard mask reference positions beyond the number of wildcards in the replace value");
						}
					}

					int FoundAt = StringEx.WildcardIndexOfAnyString(originalName, this.txtFind.Text, 0, this.chkIgnoreCaseDuringFind.Checked, '?');
					if (FoundAt >= 0)
					{
						string OriginalValue = originalName.Substring(FoundAt, this.txtFind.Text.Length);
						StringBuilder ThisReplace = new StringBuilder();
						for (Index = 0; Index < this.txtReplace.Text.Length; Index++)
						{
							if (Index < this.txtReplace.Text.Length)
							{
								if (this.txtReplace.Text[Index] == '?')
								{
									try
									{
										ThisReplace.Append(OriginalValue[FindWildcards[MusicalWildcards[ReplaceWildcards[Index]]]]);
									}
									catch
									{
									}
								}
								else
								{
									ThisReplace.Append(this.txtReplace.Text[Index]);
								}
							}
							else
							{
								ThisReplace.Append(OriginalValue[Index]);
							}
						}
						if (this.rbReplaceText.Checked)
						{
							Result = originalName.Substring(0, FoundAt) + ThisReplace.ToString() +
								((originalName.Length <= FoundAt + OriginalValue.Length) ? string.Empty :
								originalName.Substring(FoundAt + OriginalValue.Length));
						}
						else
						{
							Result = StringEx.ReplaceNoCase(originalName, ThisReplace.ToString(), string.Empty, this.chkIgnoreCaseDuringFind.Checked);
						}
					}
				}
			}
			else if (this.rbPrefixRoot.Checked)
			{
				Result = this.txtFind.Text + originalName;
			}
			else if (this.rbSuffixRoot.Checked)
			{
				string ext = Path.GetExtension(originalName);
				Result = Path.GetFileNameWithoutExtension(originalName) + this.txtFind.Text + ext;
			}
			else if (this.rbPrefixExtension.Checked)
			{
				string ext = Path.GetExtension(originalName);
				Result =
					Path.GetFileNameWithoutExtension(originalName) + "." +
					this.txtFind.Text + ((ext.Length == 0 || ext.Length == 1) ? string.Empty : ext.Substring(1));
			}
			else if (this.rbSuffixExtension.Checked)
			{
				Result = originalName + this.txtFind.Text;
			}
			else if (this.rbInsertAt.Checked)
			{
				int Offset = 0;
				Result = originalName;
				if (int.TryParse(this.txtInsertAt.Text, out Offset) && Offset < originalName.Length)
				{
					Result = originalName.Substring(0, Offset) + this.txtFind.Text +
						(Offset == originalName.Length - 1 ? string.Empty : originalName.Substring(Offset));
				}
				else
				{
					Result += this.txtFind.Text;
				}
			}

			if (this.rbBecomeLower.Checked) Result = Result.ToLower();
			if (this.rbBecomeUpper.Checked) Result = Result.ToUpper();
			return Result;
		}

		private void Adjust_Controls()
		{
			this.lblReplace.Visible = this.rbReplaceText.Checked;
			this.lblFind.Text = this.rbReplaceText.Checked || this.rbRemoveText.Checked ? "Find" : "String";
			this.lblWildcardMask.Visible = this.rbReplaceText.Checked;
			this.txtReplace.Visible = this.rbReplaceText.Checked;
			this.txtWildcardMask.Visible = this.rbReplaceText.Checked;
			this.txtInsertAt.Visible = this.rbInsertAt.Checked;
			this.chkIgnoreCaseDuringFind.Visible = this.rbReplaceText.Checked || this.rbRemoveText.Checked;
		}

		#region Form event handlders

		private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			AboutBox about = new AboutBox();
			about.ShowDialog();
		}

		private void btnSelectFolder_Click(object sender, EventArgs e)
		{
			UserChangeFolder();
		}

		private void selectFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UserChangeFolder();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void rbRemoveText_CheckedChanged(object sender, EventArgs e)
		{
			Adjust_Controls();
		}

		private void rbReplaceText_CheckedChanged(object sender, EventArgs e)
		{
			Adjust_Controls();
		}

		private void rbPrefixRoot_CheckedChanged(object sender, EventArgs e)
		{
			Adjust_Controls();
		}

		private void rbSuffixRoot_CheckedChanged(object sender, EventArgs e)
		{
			Adjust_Controls();
		}

		private void rbPrefixExtension_CheckedChanged(object sender, EventArgs e)
		{
			Adjust_Controls();
		}

		private void rbSuffixExtension_CheckedChanged(object sender, EventArgs e)
		{
			Adjust_Controls();
		}

		private void rbInsertAt_CheckedChanged(object sender, EventArgs e)
		{
			Adjust_Controls();
		}

		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			this.lbxFileManifest.SelectedIndices.Clear();
			for (int Index = 0; Index < this.lbxFileManifest.Items.Count; Index++)
			{
				this.lbxFileManifest.SelectedIndices.Add(Index);
			}
		}

		private void btnDeselectAll_Click(object sender, EventArgs e)
		{
			this.lbxFileManifest.SelectedIndices.Clear();
		}

		private void btnInvertAll_Click(object sender, EventArgs e)
		{
			List<int> Selected = new List<int>();
			for (int Index = 0; Index < this.lbxFileManifest.SelectedIndices.Count; Index++)
				Selected.Add(this.lbxFileManifest.SelectedIndices[Index]);

			this.lbxFileManifest.SelectedIndices.Clear();
			for (int Index = 0; Index < this.lbxFileManifest.Items.Count; Index++)
			{
				if (!Selected.Contains(Index))
				{
					this.lbxFileManifest.SelectedIndices.Add(Index);
				}
			}
		}

		private void btnTryIt_Click(object sender, EventArgs e)
		{
			bool Reload_Files = false;

			try
			{
				Dictionary<string, string> FileSelections = new Dictionary<string, string>();

				List<int> Selected = new List<int>();
				for (int Index = 0; Index < this.lbxFileManifest.SelectedIndices.Count; Index++)
					Selected.Add(this.lbxFileManifest.SelectedIndices[Index]);

				for (int Index = 0; Index < this.lbxFileManifest.Items.Count; Index++)
				{
					if (Selected.Contains(Index))
					{
						string ThisFile = this.lbxFileManifest.Items[Index].ToString();
						FileSelections.Add(ThisFile, Build_New_Name(ThisFile));
					}
				}
				Manifest worker = new Manifest(this.txtFolder.Text, FileSelections);
				worker.ShowDialog();
				Reload_Files = worker.Rename_Performed;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message, "Error preparing for rename", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (Reload_Files)
			{
				LoadFiles();
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (LaunchedWithParameter == false)
			{
				AppSettings.SaveSettings();
			}
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var a = new AssemblyVersion(SwissArmyKnife.AssemblyVersion.AssemblySource.Entry);
			var helpfile = Path.Combine(Path.GetDirectoryName(a.Filename), "docs", "BulkFileRename.pdf");

			var p = new Process();
			p.StartInfo.FileName = helpfile;
			p.StartInfo.UseShellExecute = false;
			p.Start();
		}
		private void lbxFileManifest_DoubleClick(object sender, EventArgs e)
		{
			if (this.lbxFileManifest.SelectedIndices.Count > 0)
			{
				this.txtFind.Text = this.lbxFileManifest.SelectedItems[0].ToString();
			}
		}

		private void createWindowsExplorerShellExtensionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dialogBoxAnswer = MessageBox.Show(
				"This will import a registry change to create a new context menu item named \"Bulk File Rename\" to " +
				"open this windows form with the contents of that selected folder. You may need administrator rights " +
				" to authorize this import.\r\n\r\nContinue?",
				"Add Windows Explorer context menu item for direct folder launch",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);
			if (dialogBoxAnswer == DialogResult.No)
			{
				MessageBox.Show("Not added", "Action Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			ApplyRegImportFromTemplate(GetRegistryTemplateForSet(), "BOG.BulkFileRename_Set.reg");
		}

		private void removeWindowsExplorerShellExtensionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dialgBoxAnswer = MessageBox.Show(
				"This will import a registry change to remove the context menu item named \"Bulk File Rename\" if it " +
				"exists. You may need administrator rights to authorize this import.\r\n\r\nContinue?",
				"Remove Windows Explorer context menu item for direct folder launch",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);
			if (dialgBoxAnswer == DialogResult.No)
			{
				MessageBox.Show("Not removed", "Action Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			ApplyRegImportFromTemplate(GetRegistryTemplateForRemove(), "BOG.BulkFileRename_Remove.reg");
		}

		private void ApplyRegImportFromTemplate(string templateText, string regFileName)
		{
			try
			{
				var tempFile = Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()), regFileName);
				if (File.Exists(tempFile)) File.Delete(tempFile);
				var a = new AssemblyVersion(AssemblyVersion.AssemblySource.Entry);
				using (var sw = new StreamWriter(tempFile))
				{
					var content = templateText.Replace("[{[PATH]}]", Path.GetDirectoryName(a.Filename).Replace(@"\", @"\\"));
					sw.Write(content);
				}
				var p = new Process();
				p.StartInfo.FileName = tempFile;
				p.StartInfo.UseShellExecute = true;
				p.Start();
			}
			catch (Exception err)
			{
				MessageBox.Show(
					DetailedException.WithUserContent(ref err),
					"Error during registry file import process",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Registry Templates

		private string GetRegistryTemplateForSet()
		{
			return
		@"Windows Registry Editor Version 5.00

[HKEY_CLASSES_ROOT\Directory\shell\BOG.BulkFileRename]
@=""Bulk File Rename""

[HKEY_CLASSES_ROOT\Directory\shell\BOG.BulkFileRename\command]
@=""\""[{[PATH]}]\\BOG.BulkFileRename.exe\"" \""%1\""""

[-HKEY_CLASSES_ROOT\Directory\shell\Bulk_File_Rename]

";
		}

		private string GetRegistryTemplateForRemove()
		{
			return
		@"Windows Registry Editor Version 5.00

[-HKEY_CLASSES_ROOT\Directory\shell\BOG.BulkFileRename]

[-HKEY_CLASSES_ROOT\Directory\shell\Bulk_File_Rename]
";
		}


		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			menuStrip1 = new MenuStrip();
			mainToolStripMenuItem = new ToolStripMenuItem();
			selectFolderToolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem1 = new ToolStripSeparator();
			exitToolStripMenuItem = new ToolStripMenuItem();
			aboutToolStripMenuItem = new ToolStripMenuItem();
			createWindowsExplorerShellExtensionToolStripMenuItem = new ToolStripMenuItem();
			removeWindowsExplorerShellExtensionToolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem2 = new ToolStripSeparator();
			helpToolStripMenuItem = new ToolStripMenuItem();
			aboutToolStripMenuItem1 = new ToolStripMenuItem();
			splitContainer1 = new SplitContainer();
			gbxMethods = new GroupBox();
			gbxTargetNames = new GroupBox();
			rbBecomeUpper = new RadioButton();
			rbBecomeLower = new RadioButton();
			rbRetainCase = new RadioButton();
			chkIgnoreCaseDuringFind = new CheckBox();
			txtWildcardMask = new System.Windows.Forms.TextBox();
			lblWildcardMask = new Label();
			txtReplace = new System.Windows.Forms.TextBox();
			lblReplace = new Label();
			txtFind = new System.Windows.Forms.TextBox();
			lblFind = new Label();
			txtInsertAt = new System.Windows.Forms.TextBox();
			rbInsertAt = new RadioButton();
			rbSuffixExtension = new RadioButton();
			rbPrefixExtension = new RadioButton();
			rbSuffixRoot = new RadioButton();
			rbPrefixRoot = new RadioButton();
			rbReplaceText = new RadioButton();
			rbRemoveText = new RadioButton();
			gbxFiles = new GroupBox();
			btnSelectFolder = new System.Windows.Forms.Button();
			txtFolder = new System.Windows.Forms.TextBox();
			btnTryIt = new System.Windows.Forms.Button();
			btnInvertAll = new System.Windows.Forms.Button();
			btnDeselectAll = new System.Windows.Forms.Button();
			btnSelectAll = new System.Windows.Forms.Button();
			lbxFileManifest = new ListBox();
			menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			gbxMethods.SuspendLayout();
			gbxTargetNames.SuspendLayout();
			gbxFiles.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] { mainToolStripMenuItem, aboutToolStripMenuItem });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Padding = new Padding(7, 2, 0, 2);
			menuStrip1.Size = new Size(674, 24);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// mainToolStripMenuItem
			// 
			mainToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { selectFolderToolStripMenuItem, toolStripMenuItem1, exitToolStripMenuItem });
			mainToolStripMenuItem.Name = "mainToolStripMenuItem";
			mainToolStripMenuItem.Size = new Size(46, 20);
			mainToolStripMenuItem.Text = "&Main";
			// 
			// selectFolderToolStripMenuItem
			// 
			selectFolderToolStripMenuItem.Name = "selectFolderToolStripMenuItem";
			selectFolderToolStripMenuItem.Size = new Size(141, 22);
			selectFolderToolStripMenuItem.Text = "&Select Folder";
			selectFolderToolStripMenuItem.Click += selectFolderToolStripMenuItem_Click;
			// 
			// toolStripMenuItem1
			// 
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new Size(138, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new Size(141, 22);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// aboutToolStripMenuItem
			// 
			aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { createWindowsExplorerShellExtensionToolStripMenuItem, removeWindowsExplorerShellExtensionToolStripMenuItem, toolStripMenuItem2, helpToolStripMenuItem, aboutToolStripMenuItem1 });
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new Size(44, 20);
			aboutToolStripMenuItem.Text = "&Help";
			// 
			// createWindowsExplorerShellExtensionToolStripMenuItem
			// 
			createWindowsExplorerShellExtensionToolStripMenuItem.Name = "createWindowsExplorerShellExtensionToolStripMenuItem";
			createWindowsExplorerShellExtensionToolStripMenuItem.Size = new Size(297, 22);
			createWindowsExplorerShellExtensionToolStripMenuItem.Text = "Create Windows Explorer Shell Extension";
			createWindowsExplorerShellExtensionToolStripMenuItem.Click += createWindowsExplorerShellExtensionToolStripMenuItem_Click;
			// 
			// removeWindowsExplorerShellExtensionToolStripMenuItem
			// 
			removeWindowsExplorerShellExtensionToolStripMenuItem.Name = "removeWindowsExplorerShellExtensionToolStripMenuItem";
			removeWindowsExplorerShellExtensionToolStripMenuItem.Size = new Size(297, 22);
			removeWindowsExplorerShellExtensionToolStripMenuItem.Text = "Remove Windows Explorer Shell Extension";
			removeWindowsExplorerShellExtensionToolStripMenuItem.Click += removeWindowsExplorerShellExtensionToolStripMenuItem_Click;
			// 
			// toolStripMenuItem2
			// 
			toolStripMenuItem2.Name = "toolStripMenuItem2";
			toolStripMenuItem2.Size = new Size(294, 6);
			// 
			// helpToolStripMenuItem
			// 
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new Size(297, 22);
			helpToolStripMenuItem.Text = "&Help";
			helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
			// 
			// aboutToolStripMenuItem1
			// 
			aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
			aboutToolStripMenuItem1.Size = new Size(297, 22);
			aboutToolStripMenuItem1.Text = "&About";
			aboutToolStripMenuItem1.Click += aboutToolStripMenuItem1_Click;
			// 
			// splitContainer1
			// 
			splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			splitContainer1.Location = new Point(0, 32);
			splitContainer1.Margin = new Padding(4, 3, 4, 3);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(gbxMethods);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(gbxFiles);
			splitContainer1.Size = new Size(674, 445);
			splitContainer1.SplitterDistance = 223;
			splitContainer1.SplitterWidth = 5;
			splitContainer1.TabIndex = 1;
			// 
			// gbxMethods
			// 
			gbxMethods.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gbxMethods.Controls.Add(gbxTargetNames);
			gbxMethods.Controls.Add(chkIgnoreCaseDuringFind);
			gbxMethods.Controls.Add(txtWildcardMask);
			gbxMethods.Controls.Add(lblWildcardMask);
			gbxMethods.Controls.Add(txtReplace);
			gbxMethods.Controls.Add(lblReplace);
			gbxMethods.Controls.Add(txtFind);
			gbxMethods.Controls.Add(lblFind);
			gbxMethods.Controls.Add(txtInsertAt);
			gbxMethods.Controls.Add(rbInsertAt);
			gbxMethods.Controls.Add(rbSuffixExtension);
			gbxMethods.Controls.Add(rbPrefixExtension);
			gbxMethods.Controls.Add(rbSuffixRoot);
			gbxMethods.Controls.Add(rbPrefixRoot);
			gbxMethods.Controls.Add(rbReplaceText);
			gbxMethods.Controls.Add(rbRemoveText);
			gbxMethods.Location = new Point(5, 5);
			gbxMethods.Margin = new Padding(4, 3, 4, 3);
			gbxMethods.MinimumSize = new Size(216, 437);
			gbxMethods.Name = "gbxMethods";
			gbxMethods.Padding = new Padding(4, 3, 4, 3);
			gbxMethods.Size = new Size(216, 437);
			gbxMethods.TabIndex = 0;
			gbxMethods.TabStop = false;
			gbxMethods.Text = "Method";
			// 
			// gbxTargetNames
			// 
			gbxTargetNames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gbxTargetNames.Controls.Add(rbBecomeUpper);
			gbxTargetNames.Controls.Add(rbBecomeLower);
			gbxTargetNames.Controls.Add(rbRetainCase);
			gbxTargetNames.Location = new Point(7, 324);
			gbxTargetNames.Margin = new Padding(4, 3, 4, 3);
			gbxTargetNames.Name = "gbxTargetNames";
			gbxTargetNames.Padding = new Padding(4, 3, 4, 3);
			gbxTargetNames.Size = new Size(202, 106);
			gbxTargetNames.TabIndex = 15;
			gbxTargetNames.TabStop = false;
			gbxTargetNames.Text = "Target Names will ...";
			// 
			// rbBecomeUpper
			// 
			rbBecomeUpper.AutoSize = true;
			rbBecomeUpper.Location = new Point(8, 76);
			rbBecomeUpper.Margin = new Padding(4, 3, 4, 3);
			rbBecomeUpper.Name = "rbBecomeUpper";
			rbBecomeUpper.Size = new Size(103, 19);
			rbBecomeUpper.TabIndex = 2;
			rbBecomeUpper.Text = "Become Upper";
			rbBecomeUpper.UseVisualStyleBackColor = true;
			// 
			// rbBecomeLower
			// 
			rbBecomeLower.AutoSize = true;
			rbBecomeLower.Location = new Point(7, 50);
			rbBecomeLower.Margin = new Padding(4, 3, 4, 3);
			rbBecomeLower.Name = "rbBecomeLower";
			rbBecomeLower.Size = new Size(103, 19);
			rbBecomeLower.TabIndex = 1;
			rbBecomeLower.Text = "Become Lower";
			rbBecomeLower.UseVisualStyleBackColor = true;
			// 
			// rbRetainCase
			// 
			rbRetainCase.AutoSize = true;
			rbRetainCase.Checked = true;
			rbRetainCase.Location = new Point(8, 23);
			rbRetainCase.Margin = new Padding(4, 3, 4, 3);
			rbRetainCase.Name = "rbRetainCase";
			rbRetainCase.Size = new Size(86, 19);
			rbRetainCase.TabIndex = 0;
			rbRetainCase.TabStop = true;
			rbRetainCase.Text = "Retain Case";
			rbRetainCase.UseVisualStyleBackColor = true;
			// 
			// chkIgnoreCaseDuringFind
			// 
			chkIgnoreCaseDuringFind.AutoSize = true;
			chkIgnoreCaseDuringFind.Location = new Point(35, 298);
			chkIgnoreCaseDuringFind.Margin = new Padding(4, 3, 4, 3);
			chkIgnoreCaseDuringFind.Name = "chkIgnoreCaseDuringFind";
			chkIgnoreCaseDuringFind.Size = new Size(148, 19);
			chkIgnoreCaseDuringFind.TabIndex = 14;
			chkIgnoreCaseDuringFind.Text = "Ignore case during find";
			chkIgnoreCaseDuringFind.UseVisualStyleBackColor = true;
			// 
			// txtWildcardMask
			// 
			txtWildcardMask.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			txtWildcardMask.Location = new Point(112, 268);
			txtWildcardMask.Margin = new Padding(4, 3, 4, 3);
			txtWildcardMask.Name = "txtWildcardMask";
			txtWildcardMask.Size = new Size(96, 23);
			txtWildcardMask.TabIndex = 13;
			txtWildcardMask.Visible = false;
			// 
			// lblWildcardMask
			// 
			lblWildcardMask.AutoSize = true;
			lblWildcardMask.Location = new Point(12, 271);
			lblWildcardMask.Margin = new Padding(4, 0, 4, 0);
			lblWildcardMask.Name = "lblWildcardMask";
			lblWildcardMask.Size = new Size(95, 15);
			lblWildcardMask.TabIndex = 12;
			lblWildcardMask.Text = "Wildcard Pattern";
			lblWildcardMask.Visible = false;
			// 
			// txtReplace
			// 
			txtReplace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			txtReplace.Location = new Point(112, 239);
			txtReplace.Margin = new Padding(4, 3, 4, 3);
			txtReplace.Name = "txtReplace";
			txtReplace.Size = new Size(96, 23);
			txtReplace.TabIndex = 11;
			txtReplace.Visible = false;
			// 
			// lblReplace
			// 
			lblReplace.AutoSize = true;
			lblReplace.Location = new Point(12, 242);
			lblReplace.Margin = new Padding(4, 0, 4, 0);
			lblReplace.Name = "lblReplace";
			lblReplace.Size = new Size(48, 15);
			lblReplace.TabIndex = 10;
			lblReplace.Text = "Replace";
			lblReplace.Visible = false;
			// 
			// txtFind
			// 
			txtFind.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			txtFind.Location = new Point(112, 210);
			txtFind.Margin = new Padding(4, 3, 4, 3);
			txtFind.Name = "txtFind";
			txtFind.Size = new Size(96, 23);
			txtFind.TabIndex = 9;
			// 
			// lblFind
			// 
			lblFind.AutoSize = true;
			lblFind.Location = new Point(12, 213);
			lblFind.Margin = new Padding(4, 0, 4, 0);
			lblFind.Name = "lblFind";
			lblFind.Size = new Size(38, 15);
			lblFind.TabIndex = 8;
			lblFind.Text = "String";
			// 
			// txtInsertAt
			// 
			txtInsertAt.Location = new Point(96, 182);
			txtInsertAt.Margin = new Padding(4, 3, 4, 3);
			txtInsertAt.Name = "txtInsertAt";
			txtInsertAt.Size = new Size(59, 23);
			txtInsertAt.TabIndex = 7;
			txtInsertAt.Text = "1";
			// 
			// rbInsertAt
			// 
			rbInsertAt.AutoSize = true;
			rbInsertAt.Location = new Point(10, 182);
			rbInsertAt.Margin = new Padding(4, 3, 4, 3);
			rbInsertAt.Name = "rbInsertAt";
			rbInsertAt.Size = new Size(72, 19);
			rbInsertAt.TabIndex = 6;
			rbInsertAt.Text = "Insert At:";
			rbInsertAt.UseVisualStyleBackColor = true;
			rbInsertAt.CheckedChanged += rbInsertAt_CheckedChanged;
			// 
			// rbSuffixExtension
			// 
			rbSuffixExtension.AutoSize = true;
			rbSuffixExtension.Location = new Point(10, 156);
			rbSuffixExtension.Margin = new Padding(4, 3, 4, 3);
			rbSuffixExtension.Name = "rbSuffixExtension";
			rbSuffixExtension.Size = new Size(109, 19);
			rbSuffixExtension.TabIndex = 5;
			rbSuffixExtension.Text = "Suffix Extension";
			rbSuffixExtension.UseVisualStyleBackColor = true;
			rbSuffixExtension.CheckedChanged += rbSuffixExtension_CheckedChanged;
			// 
			// rbPrefixExtension
			// 
			rbPrefixExtension.AutoSize = true;
			rbPrefixExtension.Location = new Point(10, 129);
			rbPrefixExtension.Margin = new Padding(4, 3, 4, 3);
			rbPrefixExtension.Name = "rbPrefixExtension";
			rbPrefixExtension.Size = new Size(109, 19);
			rbPrefixExtension.TabIndex = 4;
			rbPrefixExtension.Text = "Prefix Extension";
			rbPrefixExtension.UseVisualStyleBackColor = true;
			rbPrefixExtension.CheckedChanged += rbPrefixExtension_CheckedChanged;
			// 
			// rbSuffixRoot
			// 
			rbSuffixRoot.AutoSize = true;
			rbSuffixRoot.Location = new Point(10, 103);
			rbSuffixRoot.Margin = new Padding(4, 3, 4, 3);
			rbSuffixRoot.Name = "rbSuffixRoot";
			rbSuffixRoot.Size = new Size(83, 19);
			rbSuffixRoot.TabIndex = 3;
			rbSuffixRoot.Text = "Suffix Root";
			rbSuffixRoot.UseVisualStyleBackColor = true;
			rbSuffixRoot.CheckedChanged += rbSuffixRoot_CheckedChanged;
			// 
			// rbPrefixRoot
			// 
			rbPrefixRoot.AutoSize = true;
			rbPrefixRoot.Location = new Point(10, 76);
			rbPrefixRoot.Margin = new Padding(4, 3, 4, 3);
			rbPrefixRoot.Name = "rbPrefixRoot";
			rbPrefixRoot.Size = new Size(83, 19);
			rbPrefixRoot.TabIndex = 2;
			rbPrefixRoot.Text = "Prefix Root";
			rbPrefixRoot.UseVisualStyleBackColor = true;
			rbPrefixRoot.CheckedChanged += rbPrefixRoot_CheckedChanged;
			// 
			// rbReplaceText
			// 
			rbReplaceText.AutoSize = true;
			rbReplaceText.Location = new Point(10, 50);
			rbReplaceText.Margin = new Padding(4, 3, 4, 3);
			rbReplaceText.Name = "rbReplaceText";
			rbReplaceText.Size = new Size(90, 19);
			rbReplaceText.TabIndex = 1;
			rbReplaceText.Text = "Replace Text";
			rbReplaceText.UseVisualStyleBackColor = true;
			rbReplaceText.CheckedChanged += rbReplaceText_CheckedChanged;
			// 
			// rbRemoveText
			// 
			rbRemoveText.AutoSize = true;
			rbRemoveText.Checked = true;
			rbRemoveText.Location = new Point(10, 23);
			rbRemoveText.Margin = new Padding(4, 3, 4, 3);
			rbRemoveText.Name = "rbRemoveText";
			rbRemoveText.Size = new Size(92, 19);
			rbRemoveText.TabIndex = 0;
			rbRemoveText.TabStop = true;
			rbRemoveText.Text = "Remove Text";
			rbRemoveText.UseVisualStyleBackColor = true;
			rbRemoveText.CheckedChanged += rbRemoveText_CheckedChanged;
			// 
			// gbxFiles
			// 
			gbxFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gbxFiles.Controls.Add(btnSelectFolder);
			gbxFiles.Controls.Add(txtFolder);
			gbxFiles.Controls.Add(btnTryIt);
			gbxFiles.Controls.Add(btnInvertAll);
			gbxFiles.Controls.Add(btnDeselectAll);
			gbxFiles.Controls.Add(btnSelectAll);
			gbxFiles.Controls.Add(lbxFileManifest);
			gbxFiles.Location = new Point(4, 5);
			gbxFiles.Margin = new Padding(4, 3, 4, 3);
			gbxFiles.Name = "gbxFiles";
			gbxFiles.Padding = new Padding(4, 3, 4, 3);
			gbxFiles.Size = new Size(438, 437);
			gbxFiles.TabIndex = 0;
			gbxFiles.TabStop = false;
			gbxFiles.Text = "Folder Content";
			// 
			// btnSelectFolder
			// 
			btnSelectFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnSelectFolder.AutoSize = true;
			btnSelectFolder.Image = (Image)resources.GetObject("btnSelectFolder.Image");
			btnSelectFolder.Location = new Point(382, 31);
			btnSelectFolder.Margin = new Padding(4, 3, 4, 3);
			btnSelectFolder.Name = "btnSelectFolder";
			btnSelectFolder.Size = new Size(45, 38);
			btnSelectFolder.TabIndex = 7;
			btnSelectFolder.UseVisualStyleBackColor = true;
			btnSelectFolder.Click += btnSelectFolder_Click;
			// 
			// txtFolder
			// 
			txtFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			txtFolder.BackColor = SystemColors.Control;
			txtFolder.BorderStyle = BorderStyle.None;
			txtFolder.Location = new Point(8, 22);
			txtFolder.Margin = new Padding(4, 3, 4, 3);
			txtFolder.Multiline = true;
			txtFolder.Name = "txtFolder";
			txtFolder.ScrollBars = ScrollBars.Vertical;
			txtFolder.Size = new Size(367, 47);
			txtFolder.TabIndex = 6;
			txtFolder.Text = "Select a folder";
			// 
			// btnTryIt
			// 
			btnTryIt.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnTryIt.Location = new Point(327, 395);
			btnTryIt.Margin = new Padding(4, 3, 4, 3);
			btnTryIt.Name = "btnTryIt";
			btnTryIt.Size = new Size(100, 36);
			btnTryIt.TabIndex = 5;
			btnTryIt.Text = "Try It";
			btnTryIt.UseVisualStyleBackColor = true;
			btnTryIt.Click += btnTryIt_Click;
			// 
			// btnInvertAll
			// 
			btnInvertAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnInvertAll.Location = new Point(215, 400);
			btnInvertAll.Margin = new Padding(4, 3, 4, 3);
			btnInvertAll.Name = "btnInvertAll";
			btnInvertAll.Size = new Size(88, 27);
			btnInvertAll.TabIndex = 4;
			btnInvertAll.Text = "Invert All";
			btnInvertAll.UseVisualStyleBackColor = true;
			btnInvertAll.Click += btnInvertAll_Click;
			// 
			// btnDeselectAll
			// 
			btnDeselectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnDeselectAll.Location = new Point(120, 400);
			btnDeselectAll.Margin = new Padding(4, 3, 4, 3);
			btnDeselectAll.Name = "btnDeselectAll";
			btnDeselectAll.Size = new Size(88, 27);
			btnDeselectAll.TabIndex = 3;
			btnDeselectAll.Text = "Deselect All";
			btnDeselectAll.UseVisualStyleBackColor = true;
			btnDeselectAll.Click += btnDeselectAll_Click;
			// 
			// btnSelectAll
			// 
			btnSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnSelectAll.Location = new Point(26, 400);
			btnSelectAll.Margin = new Padding(4, 3, 4, 3);
			btnSelectAll.Name = "btnSelectAll";
			btnSelectAll.Size = new Size(88, 27);
			btnSelectAll.TabIndex = 2;
			btnSelectAll.Text = "Select All";
			btnSelectAll.UseVisualStyleBackColor = true;
			btnSelectAll.Click += btnSelectAll_Click;
			// 
			// lbxFileManifest
			// 
			lbxFileManifest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			lbxFileManifest.FormattingEnabled = true;
			lbxFileManifest.Location = new Point(8, 80);
			lbxFileManifest.Margin = new Padding(4, 3, 4, 3);
			lbxFileManifest.Name = "lbxFileManifest";
			lbxFileManifest.SelectionMode = SelectionMode.MultiExtended;
			lbxFileManifest.Size = new Size(422, 304);
			lbxFileManifest.TabIndex = 1;
			lbxFileManifest.Visible = false;
			lbxFileManifest.DoubleClick += lbxFileManifest_DoubleClick;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(674, 475);
			Controls.Add(splitContainer1);
			Controls.Add(menuStrip1);
			Icon = (Icon)resources.GetObject("$this.Icon");
			MainMenuStrip = menuStrip1;
			Margin = new Padding(4, 3, 4, 3);
			MinimumSize = new Size(681, 509);
			Name = "MainForm";
			Text = "BOG.BulkFileRename";
			FormClosing += MainForm_FormClosing;
			Load += MainForm_Load;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			gbxMethods.ResumeLayout(false);
			gbxMethods.PerformLayout();
			gbxTargetNames.ResumeLayout(false);
			gbxTargetNames.PerformLayout();
			gbxFiles.ResumeLayout(false);
			gbxFiles.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem mainToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem selectFolderToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
		private System.Windows.Forms.GroupBox gbxMethods;
		private System.Windows.Forms.GroupBox gbxFiles;
		private System.Windows.Forms.RadioButton rbReplaceText;
		private System.Windows.Forms.RadioButton rbRemoveText;
		private System.Windows.Forms.RadioButton rbSuffixRoot;
		private System.Windows.Forms.RadioButton rbPrefixRoot;
		private System.Windows.Forms.TextBox txtFind;
		private System.Windows.Forms.Label lblFind;
		private System.Windows.Forms.TextBox txtInsertAt;
		private System.Windows.Forms.RadioButton rbInsertAt;
		private System.Windows.Forms.RadioButton rbSuffixExtension;
		private System.Windows.Forms.RadioButton rbPrefixExtension;
		private System.Windows.Forms.GroupBox gbxTargetNames;
		private System.Windows.Forms.CheckBox chkIgnoreCaseDuringFind;
		private System.Windows.Forms.TextBox txtWildcardMask;
		private System.Windows.Forms.Label lblWildcardMask;
		private System.Windows.Forms.TextBox txtReplace;
		private System.Windows.Forms.Label lblReplace;
		private System.Windows.Forms.RadioButton rbBecomeUpper;
		private System.Windows.Forms.RadioButton rbBecomeLower;
		private System.Windows.Forms.RadioButton rbRetainCase;
		private System.Windows.Forms.ListBox lbxFileManifest;
		private System.Windows.Forms.Button btnDeselectAll;
		private System.Windows.Forms.Button btnSelectAll;
		private System.Windows.Forms.Button btnTryIt;
		private System.Windows.Forms.Button btnInvertAll;
		private System.Windows.Forms.TextBox txtFolder;
		private System.Windows.Forms.Button btnSelectFolder;

		private void MainForm_Load(object sender, EventArgs e)
		{

		}
	}
		#endregion
}
