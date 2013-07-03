using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InkNote
{
    public partial class FormTransparent : Form
    {
        public Point mShowLocation = Point.Empty;

        public int Value
        {
            get { return trackBar1.Value; }
        }
        public FormTransparent(byte val)
        {
            InitializeComponent();
            trackBar1.Value = val;
            label1.Text = trackBar1.Value.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            this.Opacity = ((double)(255 - trackBar1.Value))/ 255;
        }
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Opacity = 1;
        }

        private void FormTransparent_Shown(object sender, EventArgs e)
        {
            this.Location = this.mShowLocation;
        }
    }
}
