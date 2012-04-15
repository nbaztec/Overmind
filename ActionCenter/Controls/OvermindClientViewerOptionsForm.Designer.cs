namespace NX_Overmind.Actions
{
    partial class OvermindClientViewerOptionsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarShrinkFactor = new System.Windows.Forms.TrackBar();
            this.groupBoxImageQuality = new System.Windows.Forms.GroupBox();
            this.comboBoxPaletteSize = new System.Windows.Forms.ComboBox();
            this.comboBoxBitDepth = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelShrinkFactor = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarShrinkFactor)).BeginInit();
            this.groupBoxImageQuality.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Palette Size:";
            // 
            // trackBarShrinkFactor
            // 
            this.trackBarShrinkFactor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarShrinkFactor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBarShrinkFactor.Location = new System.Drawing.Point(22, 62);
            this.trackBarShrinkFactor.Maximum = 80;
            this.trackBarShrinkFactor.Name = "trackBarShrinkFactor";
            this.trackBarShrinkFactor.Size = new System.Drawing.Size(402, 45);
            this.trackBarShrinkFactor.TabIndex = 1;
            this.trackBarShrinkFactor.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarShrinkFactor.Value = 40;
            this.trackBarShrinkFactor.ValueChanged += new System.EventHandler(this.trackBarShrinkFactor_ValueChanged);
            // 
            // groupBoxImageQuality
            // 
            this.groupBoxImageQuality.Controls.Add(this.comboBoxPaletteSize);
            this.groupBoxImageQuality.Controls.Add(this.comboBoxBitDepth);
            this.groupBoxImageQuality.Controls.Add(this.trackBarShrinkFactor);
            this.groupBoxImageQuality.Controls.Add(this.label2);
            this.groupBoxImageQuality.Controls.Add(this.labelShrinkFactor);
            this.groupBoxImageQuality.Controls.Add(this.label1);
            this.groupBoxImageQuality.Location = new System.Drawing.Point(12, 33);
            this.groupBoxImageQuality.Name = "groupBoxImageQuality";
            this.groupBoxImageQuality.Size = new System.Drawing.Size(456, 141);
            this.groupBoxImageQuality.TabIndex = 2;
            this.groupBoxImageQuality.TabStop = false;
            this.groupBoxImageQuality.Text = "Image Quality";
            // 
            // comboBoxPaletteSize
            // 
            this.comboBoxPaletteSize.FormattingEnabled = true;
            this.comboBoxPaletteSize.Items.AddRange(new object[] {
            "256",
            "128",
            "64",
            "48",
            "32",
            "24",
            "16",
            "8",
            "4",
            "2",
            "1"});
            this.comboBoxPaletteSize.Location = new System.Drawing.Point(91, 35);
            this.comboBoxPaletteSize.Name = "comboBoxPaletteSize";
            this.comboBoxPaletteSize.Size = new System.Drawing.Size(61, 21);
            this.comboBoxPaletteSize.TabIndex = 2;
            this.comboBoxPaletteSize.TextChanged += new System.EventHandler(this.comboBoxBitDepth_TextChanged);
            // 
            // comboBoxBitDepth
            // 
            this.comboBoxBitDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBitDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBitDepth.FormattingEnabled = true;
            this.comboBoxBitDepth.Items.AddRange(new object[] {
            "48",
            "32",
            "24",
            "16",
            "8",
            "4",
            "2",
            "1"});
            this.comboBoxBitDepth.Location = new System.Drawing.Point(363, 35);
            this.comboBoxBitDepth.Name = "comboBoxBitDepth";
            this.comboBoxBitDepth.Size = new System.Drawing.Size(61, 21);
            this.comboBoxBitDepth.TabIndex = 2;
            this.comboBoxBitDepth.TextChanged += new System.EventHandler(this.comboBoxBitDepth_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(303, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Bit-Depth:";
            // 
            // labelShrinkFactor
            // 
            this.labelShrinkFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelShrinkFactor.AutoSize = true;
            this.labelShrinkFactor.Location = new System.Drawing.Point(194, 110);
            this.labelShrinkFactor.Name = "labelShrinkFactor";
            this.labelShrinkFactor.Size = new System.Drawing.Size(69, 13);
            this.labelShrinkFactor.TabIndex = 0;
            this.labelShrinkFactor.Text = "Image Shrink";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(200, 190);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 37);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // OvermindClientViewerOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 244);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBoxImageQuality);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OvermindClientViewerOptionsForm";
            this.Text = "Client Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OvermindClientViewerOptionsForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarShrinkFactor)).EndInit();
            this.groupBoxImageQuality.ResumeLayout(false);
            this.groupBoxImageQuality.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBarShrinkFactor;
        private System.Windows.Forms.GroupBox groupBoxImageQuality;
        private System.Windows.Forms.ComboBox comboBoxPaletteSize;
        private System.Windows.Forms.ComboBox comboBoxBitDepth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelShrinkFactor;
        private System.Windows.Forms.Button buttonOk;
    }
}