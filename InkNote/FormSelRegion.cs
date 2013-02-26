using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Ink;

namespace InkNote
{
    public partial class FormSelRegion : Form
    {

        public InkPicture mInkPicture = null;
        private bool mIsRegionClipped = false;
        public Region SelRegion
        {
            get { return mSelRegion;}
        }
        Bitmap mBgBmp = null;
        Region mSelRegion = null;

        public FormSelRegion()
        {
            InitializeComponent();

            mInkPicture = new InkPicture();
            this.Controls.Add(mInkPicture);
            mInkPicture.Dock = DockStyle.Fill;
            mInkPicture.NewPackets += new InkCollectorNewPacketsEventHandler(mInkPicture_NewPackets);
            mInkPicture.Stroke += new InkCollectorStrokeEventHandler(mInkPicture_Stroke);
            mInkPicture.DefaultDrawingAttributes.Color = Color.Blue;
        }

        void mInkPicture_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            e.Cancel = true; ;
        }

        void mInkPicture_NewPackets(object sender, InkCollectorNewPacketsEventArgs e)
        {
            float[] intersections = e.Stroke.SelfIntersections;
            if (intersections.Length > 0)
            {
                if (mIsRegionClipped) return;

                String msg = "SelfIntersections=";
                foreach (float f in intersections)
                {
                    msg += f + " ";
                }
                Console.WriteLine(msg);

                int ipt1 = (int)Math.Round(intersections[0], 0);
                int ipt2 = (int)Math.Round(intersections[1], 0);
                int count = ipt2 - ipt1;
                Point[] pts = e.Stroke.GetPoints();
                Point[] ptPath = new Point[count];
                //Graphics g = this.CreateGraphics();
                Graphics g = Graphics.FromImage(mBgBmp);
                mInkPicture.Renderer.InkSpaceToPixel(g, ref pts);
                Array.Copy(pts, ipt1, ptPath, 0, count);
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddClosedCurve(ptPath);
                if (mSelRegion != null) mSelRegion.Dispose();
                mSelRegion = new Region(path);

                SolidBrush b = new SolidBrush(Color.Red);
                Pen p = new Pen(b);
                //g.DrawPath(p, path);
                //g.FillRegion(b, rg);
                //rg.Dispose();
                p.Dispose();
                b.Dispose();
                g.Dispose();
                mInkPicture.Refresh();
                mIsRegionClipped = true;
                this.Close();
                //CopyDesktopImageToClipboard();
            }
            else
            {
                mIsRegionClipped = false;
            }

            //throw new NotImplementedException();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mBgBmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            /*
            Graphics g = Graphics.FromImage(mBgBmp);
            Pen p = new Pen(Color.Green);
            g.DrawLine(p, new Point(0, 0), new Point(this.ClientSize.Width, this.ClientSize.Height));
            p.Dispose();
            g.Dispose();
             */
            mInkPicture.BackgroundImage = mBgBmp;
        }
        
        
    }
}
