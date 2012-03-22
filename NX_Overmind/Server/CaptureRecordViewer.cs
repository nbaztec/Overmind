using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using NX.Collections;
using NX_Overmind.Actions;

namespace NX_Overmind
{
    public partial class CaptureRecordViewer : Form
    {
        private DisposableDirectory _ddir = null;
        private List<FileInfo> _fileList = new List<FileInfo>();

        public CaptureRecordViewer()
        {
            InitializeComponent();
            this.toolStripComboBoxAutoplaySpeed.SelectedIndex = 2;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Compression7z cmp7z = new Compression7z();
                cmp7z.OnComplete += new Compression7z.ProcessEventHandler(cmp7z_OnComplete);
                if (this._ddir != null)
                {
                    this._ddir.Dispose();
                    this._ddir = null;
                }
                this._ddir = new DisposableDirectory();
                this.progressBarLoading.Visible = true;
                cmp7z.ExtractFile(false, this.openFileDialog1.FileName, this._ddir.DirectoryPath, Compression7z.Type._7z, Compression7z.Compression.Ultra, null);                
            }
        }

        void cmp7z_OnComplete(CompressionEventArgs e, bool errorOccured)
        {
            if (errorOccured)
                MessageBox.Show("Error Occurred in extracting files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else                            
                this.LoadFiles(this._ddir.DirectoryPath);
                
        }

        private void LoadFiles(string dirPath)
        {
            this._fileList.Clear();
            FileInfo[] array = new DirectoryInfo(dirPath).GetFiles();
            Array.Sort(array, delegate(FileInfo f1, FileInfo f2)
            {
                return f1.CreationTimeUtc.CompareTo(f2.CreationTimeUtc);
            });
            foreach (FileInfo fi in array)
            {
                //if (fi.Name[0] == '_')
                //    continue;
                //FileInfo _fi = new FileInfo(fi.DirectoryName + "\\_" + fi.Name);
                //File.Move(fi.FullName, _fi.FullName);
                //this._fileList.Enqueue(_fi);
                this._fileList.Add(fi);
            }
            this.BeginInvoke(new MethodInvoker(delegate
            {
                this.trackBarTimeline.Minimum = 0;                
                this.trackBarTimeline.Maximum = this._fileList.Count - 1;
                this.trackBarTimeline.Value = 0;
                this.progressBarLoading.Visible = false;
            }));
        }

        private void trackBarTimeline_ValueChanged(object sender, EventArgs e)
        {
            if (this._fileList.Count != 0)            
            {
                CapturePacket cp = new CapturePacket();
                //cp.Load(this._fileList.Dequeue().FullName);
                //cp.ReadDecoded(this._fileList.Dequeue().FullName);
                cp.ReadDecoded(this._fileList[this.trackBarTimeline.Value].FullName);
                //this._fileList.RemoveAt(0);                
                string s = HookEventHelper.StreamToString(cp.Log);
                if (s != "")
                {
                    if(this.eraseEventLogToolStripMenuItem.Checked)
                        this.textLog.Text = (s + Environment.NewLine);
                    else
                        this.textLog.AppendText(s + Environment.NewLine);
                }
                //this.textLog.AppendText(cp.KeyLog.Trim() + Environment.NewLine);
                //this.textLog.AppendText(cp.MouseLog.Trim() + Environment.NewLine);
                if (cp.ScreenShot.Length != 0)
                    this.pictureScreen.Image = Image.FromStream(cp.ScreenShot);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.trackBarTimeline.Value = 0;
            this.pictureScreen.Image = null;
            this.textLog.Clear();            
        }

        private void startStopToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.timerPlay.Enabled = this.startStopToolStripMenuItem.Checked;
        }

        private void timerPlay_Tick(object sender, EventArgs e)
        {
            if(this.trackBarTimeline.Value < this.trackBarTimeline.Maximum)
                this.trackBarTimeline.Value++;
        }

        private void toolStripComboBoxAutoplaySpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.toolStripComboBoxAutoplaySpeed.SelectedIndex)
            {
                case 0:
                    this.timerPlay.Interval = 2000;
                    break;
                case 1:
                    this.timerPlay.Interval = 1500;
                    break;
                case 2:
                    this.timerPlay.Interval = 900;
                    break;
                case 3:
                    this.timerPlay.Interval = 500;
                    break;
                case 4:
                    this.timerPlay.Interval = 200;
                    break;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        private void CaptureRecordViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._ddir != null)
                this._ddir.Dispose();
        }
    }
}
