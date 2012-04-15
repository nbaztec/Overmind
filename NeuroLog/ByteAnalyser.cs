using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NX.Log.NeuroLog
{
    public partial class ByteAnalyser : Form
    {
        private Dictionary<char, string> _translateDict = null;
        public ByteAnalyser(string inputBytes)
        {
            InitializeComponent();
            this.richTextView.Text = inputBytes;
            this._translateDict = new Dictionary<char,string>();
            this._translateDict.Add('\0',"\\0");
            this._translateDict.Add('\f',"\\f");
            this._translateDict.Add('\b',"\\b");
            this._translateDict.Add('\v',"\\v");
            this._translateDict.Add('\t',"\\t");
            this._translateDict.Add('\r',"\\r");
            this._translateDict.Add('\n', "\\n");    
            
            string buff="";
            try
            {
                foreach (char c in inputBytes)
                {
                    if (Char.IsLetterOrDigit(c))
                        buff += c;
                    else
                    {
                        if (buff != "")
                        {
                            this.AppendHexChar(buff);         
                            buff = "";
                        }
                        this.richTextBoxOutput.Text += c;
                    }                                            
                }
                if (buff != "")
                    this.AppendHexChar(buff);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendHexChar(string buff)
        {
            char ch = (char)Convert.ToByte(buff, 16);
            string s;
            if (Char.IsControl(ch))
                s = this._translateDict.ContainsKey(ch) ? this._translateDict[ch] : ch.ToString();
            else
                s = ch.ToString();

            this.richTextBoxOutput.Text += String.Format("{0, 2}", s);
        }
    }
}
