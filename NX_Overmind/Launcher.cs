using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NX.Controls;

namespace NX_Overmind
{
    public partial class Launcher : GlassForm
    {
        public Launcher()
        {
            InitializeComponent();            
            this.comboClientType.SelectedIndex = 0;
            this.comboClientSpeed.SelectedIndex = 1;
            //this.GlassEnabled = this.SheetOfGlass = true;
        }

        private void buttonServerStart_Click(object sender, EventArgs e)
        {
            //try
            {
                ServerForm sf = new ServerForm(int.Parse(this.textServerPort.Text));
                sf.ShowDialog();
            }
            //catch (Exception ex)
            {
            //    MessageBox.Show(ex.Message, "Shoot! An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonClientConnect_Click(object sender, EventArgs e)
        {
            try
            {
                ClientForm.ClientTransferType cttf = (ClientForm.ClientTransferType)((this.comboClientType.SelectedIndex + 1) 
                        | (byte)(this.checkClientKeys.Checked  ? ClientForm.ClientTransferType.KeyEventLogging   : 0x00)
                        | (byte)(this.checkClientMouse.Checked ? ClientForm.ClientTransferType.MouseEventLogging : 0x00)
                        | (byte)(this.checkClientScreenCapture.Checked ? 0x00 : ClientForm.ClientTransferType.NoScreenCapture)
                    );
                int interval = 1000;
                switch (this.comboClientSpeed.SelectedIndex)
                {
                    case 0:
                        interval = 250;
                        break;
                    case 1:
                        interval = 100;
                        break;
                    case 2:
                        interval = 1000;
                        break;
                }
                ClientForm cf = new ClientForm(this.textClientAddr.Text, int.Parse(this.textClientPort.Text), cttf, interval, this.textClientName.Text);
                cf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Shoot! An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void recordViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaptureRecordViewer crv = new CaptureRecordViewer();
            crv.ShowDialog();
            crv.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
