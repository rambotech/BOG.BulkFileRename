using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BOG.BulkFileRename
{
    public partial class Manifest : Form
    {
        private string _Folder = string.Empty;
        private bool _Rename_Performed = false;

        public bool Rename_Performed
        {
            get { return _Rename_Performed; }
        }

        public Manifest(string folder, Dictionary<string, string> files)
        {
            InitializeComponent();

            _Folder = folder;
            this.lblFolder.Text = "in " + folder;

            List<string> OriginalCollisions = new List<string>();
            List<string> TargetCollisions = new List<string>();

            foreach (string Original in files.Keys)
            {
                string Target = files[Original];
                string Status = string.Empty;
                bool SelectIt = true;
                bool OverwriteIt = false;
                if (files[Original].Length == 0)
                {
                    Status = "Blank name";
                    SelectIt = false;
                }
                else if (files[Original].IndexOfAny(new char[] { '*', '?' }) >= 0)
                {
                    Status = "Illegal characters in new name";
                    SelectIt = false;
                }
                else if (string.Compare(Original, files[Original], true) == 0)
                {
                    Status = "Name unchanged";
                    SelectIt = false;
                }
                else if (OriginalCollisions.Contains(Original))
                {
                    Status = "Double rename";
                }
                else if (!File.Exists(Path.Combine(folder, Original)))
                {
                    Status = "Source missing";
                    SelectIt = false;
                }
                else if (File.Exists(Path.Combine(folder, files[Original])))
                {
                    Status = "Target exists";
                }
                if (!OriginalCollisions.Contains(files[Original]))
                {
                    OriginalCollisions.Add(files[Original]);
                }
                this.dgvRename.Rows.Add(new object[] {
                    SelectIt,
                    OverwriteIt,
                    Original,
                    files[Original],
                    Status
                });
            }
            this.btnRename.Enabled = this.dgvRename.Rows.Count > 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow r in this.dgvRename.Rows)
                {
                    bool Selected = (bool)r.Cells[0].Value;
                    bool Overwrite = (bool)r.Cells[1].Value;
                    string Original_File = Path.Combine(_Folder, (string)r.Cells[2].Value);
                    string New_Name = Path.Combine(_Folder, (string)r.Cells[3].Value);
                    string Result = Selected ? "OK" : "skipped";

                    if (Selected)
                    {
                        bool DoIt = true;
                        if (File.Exists(New_Name))
                        {
                            if (Overwrite == false)
                            {
                                Result = "Not overwritten";
                                DoIt = false;
                            }
                            else
                            {
                                try
                                {
                                    File.Delete(New_Name);
                                }
                                catch (Exception err1)
                                {
                                    Result = string.Format("ERR (deleting): {0}", err1.Message);
                                }
                            }
                        }
                        if (DoIt)
                        {
                            try
                            {
                                File.Move(Original_File, New_Name);
                            }
                            catch (Exception err2)
                            {
                                Result = string.Format("ERR (renaming): {0}", err2.Message);
                            }
                        }
                    }
                    r.Cells[4].Value = Result;
                }
            }
            catch (Exception unexpected)
            {
                MessageBox.Show(unexpected.Message, "Ouch.. stopping rename process.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.btnRename.Enabled = false;
            _Rename_Performed = true;
        }
    }
}
