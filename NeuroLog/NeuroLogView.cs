using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NX.Log.NeuroLog;

namespace NeuroLogViewer
{
    public partial class NeuroLogView : Form
    {
        private List<string> _detailList = null;
        public NeuroLogView()
        {
            InitializeComponent();
            this._detailList = new List<string>();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.dataGridView.Rows.Clear();
                this.richTextView.Clear();
                this._detailList.Clear();

                OvermindLogManager ovLog = new OvermindLogManager(this.openFileDialog.FileName);
                ovLog.SeekToFirst();
                LogEntry entry = null;
                int i = 1;
                while ((entry = ovLog.ReadNext()) != null)
                {
                    this._detailList.Add(entry.Details);
                    this.dataGridView.Rows.Add(new string[] { i.ToString(), entry.Timestamp.ToString("dd/MM/yyyy HH:mm:ss.fff"), ((OvermindLogManager.LogEntryType)entry.EntryType).ToString(), entry.Message });                    
                   // this.dataGridView.Rows.Add();
                    i++;
                }
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine(this.dataGridView.Rows[this.dataGridView.CurrentCell.RowIndex].Cells[0].Value.ToString());
            this.richTextView.Text = this._detailList[int.Parse(this.dataGridView.Rows[this.dataGridView.CurrentCell.RowIndex].Cells[0].Value.ToString())-1];
        }

        private void analyseBytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ByteAnalyser(this.richTextView.SelectedText.Trim()).ShowDialog(this);
        }

        private void dataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0)
            {
                if (int.Parse(e.CellValue1.ToString()) > int.Parse(e.CellValue2.ToString()))                
                    e.SortResult = 1;                
                else if (int.Parse(e.CellValue1.ToString()) < int.Parse(e.CellValue2.ToString()))                
                    e.SortResult = -1;                
                else                
                    e.SortResult = 0;                
                e.Handled = true;
            }
        }
    }
}
