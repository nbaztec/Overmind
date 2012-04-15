using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NX_Overmind.Actions
{
    public partial class OvermindClientShell : Form
    {
        public delegate void DataEventHandler(string text);
        public event DataEventHandler TextInputReceived = null;        
        public string Prompt { get; set; }
        
        public OvermindClientShell()
        {
            InitializeComponent();
            this.Prompt = "> ";
            this.richShell.AppendText(this.Prompt);            
        }

        public void AppendLine(string text)
        {
            /*string lst = this.LastLine();
            if (lst.Equals(this.Prompt))
                this.richShell.Lines[this.richShell.Lines.Length - 1] = text;
            else
                this.richShell.AppendText(text);
            //this.PresentNextPrompt();
             */
            this.RemoveLastCommand();
            text = text.TrimStart(new char[] { '\f' });
            //text.Substring(text.IndexOf('\n')-1);
            System.Diagnostics.Trace.WriteLine("["+text+"]");
            this.richShell.Text += text;
            this.richShell.SelectionStart = this.richShell.TextLength;
            this.richShell.ScrollToCaret();
        }

        private void richShell_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if(!Char.IsControl(e.KeyChar))
                this.richShell.AppendText(e.KeyChar.ToString());            
        }

        private void OvermindClientShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private int __lastCmdBegin = 0;
        private bool __lastCmdValid = false;
        private void richShell_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                string cmd = this.LastCommand();                
                System.Diagnostics.Trace.WriteLine(cmd);
                this.__lastCmdBegin = this.richShell.TextLength - cmd.Length;
                this.__lastCmdValid = true;
                //string _t = this.richShell.Lines[this.richShell.Lines.Length-1];
                //this.richShell.Text = this.richShell.Text.Remove(this.richShell.TextLength - cmd.Length);                
                if (this.TextInputReceived != null)
                    this.TextInputReceived(cmd);
            }
        }

        private void RemoveLastCommand()
        {
            if (this.__lastCmdValid)
            {                
                this.richShell.Text = this.richShell.Text.Remove(this.__lastCmdBegin);
                this.__lastCmdValid = false;
            }
        }
        /*private void PresentNextPrompt()
        {
            this.richShell.AppendText(this.Prompt);            
        }*/


        private string LastLine()
        {
            return this.richShell.Lines[this.richShell.Lines.Length - 1];
        }

        private string LastCommand()
        {
            string s = this.LastLine();
            return s.Substring(s.IndexOf('>') + 1);

        }
    }
}
