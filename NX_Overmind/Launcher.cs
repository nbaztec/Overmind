using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NX.Controls;
using NX.Log.NeuroLog;

namespace NX_Overmind
{
    public partial class Launcher : GlassForm
    {
        private OvermindLogManager _neuroLog = new OvermindLogManager("NeuroLog", OvermindLogManager.LogEntryType.App_Launcher);

        public Launcher()
        {
            InitializeComponent();            
            this.comboClientType.SelectedIndex = 0;
            this.comboClientSpeed.SelectedIndex = 1;

            this._neuroLog.Erase();
            this._neuroLog.WriteFormat("Launcher Started", "Defaults\n\nClient Type: {0}; Client Speed: {1}\nGlass: {2}, Sheet: {3}", this.comboClientType.Items[this.comboClientType.SelectedIndex], this.comboClientSpeed.Items[this.comboClientSpeed.SelectedIndex], this.GlassEnabled, this.SheetOfGlass);
            //this.GlassEnabled = this.SheetOfGlass = true;
        }

        private void buttonServerStart_Click(object sender, EventArgs e)
        {
            try
            {
                this._neuroLog.WriteFormat("Starting Server", "Port {0}", this.textServerPort.Text);

                ServerForm sf = new ServerForm(int.Parse(this.textServerPort.Text));
                sf.ShowDialog();

                this._neuroLog.Write("Server Closed", "All Good");
            }
            catch (Exception ex)
            {
                this._neuroLog.WriteFormat("Client Terminated with Exception", "Message: {0}\n\nStack Trace:\n", ex.Message, ex.StackTrace);
                MessageBox.Show(ex.Message, "Shoot! An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                this._neuroLog.WriteFormat("Starting Client", "{0}<{1}:{2}>\n\n-= Settings =-\nConnection Type: {3}\nConnection Speed:{4}\nCapture Interval: {5}ms\nLogging: Screen({6}), Keys({7}), Mouse({8})", 
                    this.textClientName.Text, this.textClientAddr.Text, 
                    this.comboClientType.SelectedText, this.comboClientSpeed.SelectedText,
                    this.textClientPort.Text, interval, this.checkClientScreenCapture.Checked, this.checkClientKeys.Checked, this.checkClientMouse.Checked);

                ClientForm cf = new ClientForm(this.textClientAddr.Text, int.Parse(this.textClientPort.Text), cttf, interval, this.textClientName.Text);
                cf.ShowDialog();

                this._neuroLog.Write("Client Closed", "All Good");
            }
            catch (Exception ex)
            {
                this._neuroLog.WriteFormat("Client Terminated with Exception", "Message: {0}\n\nStack Trace:\n", ex.Message, ex.StackTrace);
                MessageBox.Show(ex.Message, "Shoot! An Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void recordViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaptureRecordViewer crv = new CaptureRecordViewer();
            this._neuroLog.Write("Record Viewer Started");
            crv.ShowDialog();
            crv.Dispose();
            this._neuroLog.Write("Record Viewer Disposed");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._neuroLog.Write("Launcher Closed", "All Good");
            this.Close();
        }
    }
}
