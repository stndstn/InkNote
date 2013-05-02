﻿using System;
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

            this.Icon = Properties.Resources.Icon1;
        }

        void mInkPicture_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            e.Cancel = true;
            mIsRegionClipped = false;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
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

                try
                {
                    int ipt1 = (int)Math.Round(intersections[0], 0);
                    int ipt2 = (int)Math.Round(intersections[intersections.Length - 1], 0);
                    int count = ipt2 - ipt1;
                    Point[] pts = e.Stroke.GetPoints();
                    Point[] ptPath = new Point[count];
                    Graphics g = Graphics.FromImage(mBgBmp);
                    mInkPicture.Renderer.InkSpaceToPixel(g, ref pts);
                    Array.Copy(pts, ipt1, ptPath, 0, count);
                    System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                    path.AddClosedCurve(ptPath);
                    if (mSelRegion != null) mSelRegion.Dispose();
                    mSelRegion = new Region(path);

                    SolidBrush b = new SolidBrush(Color.Red);
                    Pen p = new Pen(b);
                    p.Dispose();
                    b.Dispose();
                    g.Dispose();
                    mInkPicture.Refresh();
                    mIsRegionClipped = true;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
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
