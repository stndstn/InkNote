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
        private DialogResult dlgRes = DialogResult.None;
        public BitmapPosData mSelectedBitmapData = null;
        public Region SelRegion
        {
            get { return mSelRegion;}
        }
        Bitmap mBgBmp = null;
        Region mSelRegion = null;
        Point[] mRegionPath = null;
        public Point[] RegionPath
        {
            get { return mRegionPath; }
        }
        Point mOriginal = Point.Empty;
        public Point OriginalPos
        {
            get { return mOriginal; }
        }

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
            Console.WriteLine("mInkPicture_Stroke >>");
            this.DialogResult = dlgRes;
            this.Close();
            Console.WriteLine("<< mInkPicture_Stroke");
        }

        void mInkPicture_NewPackets(object sender, InkCollectorNewPacketsEventArgs e)
        {
            //Console.WriteLine("mInkPicture_NewPackets PacketCount={0}", e.PacketCount);
            //if (this.DialogResult != System.Windows.Forms.DialogResult.None) return;
            if (dlgRes != System.Windows.Forms.DialogResult.None) return;

            float[] intersections = e.Stroke.SelfIntersections;
            if (intersections.Length > 0)
            {
                String msg = "SelfIntersections=";
                foreach (float f in intersections)
                {
                    msg += f + " ";
                }
                Console.WriteLine(msg);

                try
                {
                    int ipt1 = (int)Math.Round(intersections[0], 0);
                    int ipt2 = (int)Math.Round(intersections[intersections.Length - 1], 0);
                    int count = ipt2 - ipt1;
                    Point[] pts = e.Stroke.GetPoints();
                    Point[] ptPath = new Point[count];
                    //Graphics g = Graphics.FromImage(mBgBmp);
                    Graphics g = mInkPicture.CreateGraphics();
                    mInkPicture.Renderer.InkSpaceToPixel(g, ref pts);
                    Array.Copy(pts, ipt1, ptPath, 0, count);
                    Pen p = new Pen(Color.Red);
                    g.DrawPolygon(p, ptPath);

                    dlgRes = MessageBox.Show("Clip and Copy this region?", "Selection", MessageBoxButtons.YesNoCancel);
                    if (dlgRes == System.Windows.Forms.DialogResult.Yes)
                    {
                        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                        path.AddClosedCurve(ptPath);
                        if (mSelRegion != null) mSelRegion.Dispose();
                        mSelRegion = new Region(path);
                        mRegionPath = ptPath;
                    }
                    g.Dispose();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Form1_Load >>");

            mBgBmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            mInkPicture.BackgroundImage = mBgBmp;            //mInkPicture.InkEnabled = true;
            mInkPicture.Ink.DeleteStrokes();
            mInkPicture.Refresh();
            dlgRes = DialogResult.None;
            Console.WriteLine("<< Form1_Load");
        }

        private void FormSelRegion_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("FormSelRegion_FormClosing >>");
            if (mBgBmp != null) mBgBmp.Dispose();
            if (mSelectedBitmapData != null) mSelectedBitmapData.Dispose();
            Console.WriteLine("<< FormSelRegion_FormClosing");
        }

        public void Dispose()
        {
            Console.WriteLine("FormSelRegion.Dispose() >>");
            base.Dispose();
            Console.WriteLine("<< FormSelRegion.Dispose()");
        }
    }
}
