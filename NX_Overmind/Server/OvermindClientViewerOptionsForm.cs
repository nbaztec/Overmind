using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NX_Overmind
{
    public partial class OvermindClientViewerOptionsForm : Form
    {
        private bool _hasChanged = false;
        public bool HasValueChanged { get { return this._hasChanged; } }

        public int ImagePaletteSize
        {
            get
            {
                return int.Parse(this.comboBoxPaletteSize.Text);
            }
        }

        public byte ImageBitDepth
        {
            get
            {
                return byte.Parse(this.comboBoxBitDepth.Text);
            }
        }

        public float ImageShrinkFactor
        {
            get
            {
                return this.trackBarShrinkFactor.Value * 0.01f;
            }
        }

        public OvermindClientViewerOptionsForm(int paletteSz, int bitDepth, float shrinkFactr)
        {
            InitializeComponent();
            this.comboBoxPaletteSize.Text = paletteSz.ToString();
            this.comboBoxBitDepth.Text = bitDepth.ToString();
            this.trackBarShrinkFactor.Value = (int)(shrinkFactr*100);
            this.labelShrinkFactor.Text = String.Format("{0}% Image Shrink", this.trackBarShrinkFactor.Value);
            this._hasChanged = false;
        }

        private void trackBarShrinkFactor_ValueChanged(object sender, EventArgs e)
        {
            this.labelShrinkFactor.Text = String.Format("{0}% Image Shrink", this.trackBarShrinkFactor.Value);
            this._hasChanged = true;
        }

        private void OvermindClientViewerOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            try
            {
                int.Parse(this.comboBoxPaletteSize.Text);
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "Please recheck the fields\r\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxBitDepth_TextChanged(object sender, EventArgs e)
        {
            this._hasChanged = true;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }       
    }
}
