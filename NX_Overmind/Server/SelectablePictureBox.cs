using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
    public partial class SelectablePictureBox : System.Windows.Forms.PictureBox
    {        
        public new event KeyEventHandler KeyDown = null;
        public new event KeyPressEventHandler KeyPress = null;
        public new event KeyEventHandler KeyUp = null;
        
        public SelectablePictureBox()
        {
            InitializeComponent();
            this.SetStyle(System.Windows.Forms.ControlStyles.Selectable, true);
        }

        public SelectablePictureBox(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this.SetStyle(System.Windows.Forms.ControlStyles.Selectable, true);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Select();            
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (this.KeyDown != null)
                this.KeyDown(this, e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (this.KeyPress != null)
                this.KeyPress(this, e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (this.KeyUp != null)
                this.KeyUp(this, e);
        }
    }
}
