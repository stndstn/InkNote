using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Ink;

namespace InkNote
{
    public partial class FormNote : Form
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest,
            int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
            int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
            int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobjBmp);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public struct BitmapData
        {
            public Region region;
            public Point location;
            public Bitmap bmp;
        }

        PictureBox mTempPict = null;
        Region mRgnTempPict = null;
        Bitmap mBmpClip = null;
        Bitmap mBmpBG = null;
        bool mIsTempPictMoving = false;
        Point mTempPictMoveOffset = Point.Empty;
        public InkPicture mInkPicture = null;
        Size mBmpSize = Size.Empty;
        List<BitmapData> mBgBitmaps = null;
        new List<BitmapData>();

        public FormNote()
        {
            InitializeComponent();
            mBgBitmaps = new List<BitmapData>();
            mTempPict = new PictureBox();
            mTempPict.Visible = false;
            //mTempPict.MouseMove += new MouseEventHandler(mTempPict_MouseMove);
            //mTempPict.MouseDown += new MouseEventHandler(mTempPict_MouseDown);
            this.panel1.MouseMove += new MouseEventHandler(FormNote_MouseMove);
            this.panel1.MouseDown += new MouseEventHandler(FormNote_MouseDown);
            mInkPicture = new InkPicture();
            
            string s1 = "AEkdAi4qAwRIEEU1GRQyCACsFQLUuOJBMwgAgAwCQ7fiQRGrqtNBCiItgv2l+0bCVKm5VJQs3NixUTcogvy5+XQJZYsCwWVFipQA";
            string s2 = "AE4cA4CABB0CLioDBEgQRTUZFDIIAKwVAtS44kEzCACADAJDt+JBEauq00EKIi2C/aX7RsJUqblUlCzc2LFRNyiC/Ln5dAlliwLBZUWKlAA=";
            byte[] decoded1 = System.Convert.FromBase64String(s1);
            byte[] decoded2 = System.Convert.FromBase64String(s2);
            //mInkPicture.Ink.Load(decoded1);
            //mInkPicture.Refresh();
            mInkPicture.Ink.Load(decoded2);
            mInkPicture.Refresh();

            this.panel1.Controls.Add(mInkPicture);
            mInkPicture.Dock = DockStyle.Fill;
            this.mInkPicture.Controls.Add(mTempPict);
            mBmpSize = this.ClientSize;
        }

        Size DrawBmpToBg(BitmapData bmpdt, Image imgBg)
        {
            Graphics g = Graphics.FromImage(imgBg);
            g.SetClip(bmpdt.region, System.Drawing.Drawing2D.CombineMode.Replace);
            g.DrawImage(bmpdt.bmp, bmpdt.location);
            g.Dispose();
            Size reqSize = new Size(bmpdt.location.X + bmpdt.bmp.Width, bmpdt.location.Y + bmpdt.bmp.Height);
            if (reqSize.Height > mBmpSize.Height) mBmpSize.Height = reqSize.Height;
            if (reqSize.Width > mBmpSize.Width) mBmpSize.Width = reqSize.Width;
        }
        void FormNote_MouseDown(object sender, MouseEventArgs e)
        {
            if (mIsTempPictMoving && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Console.WriteLine("FormNote_MouseDown");
                mIsTempPictMoving = false;
                mInkPicture.Enabled = true;
                BitmapData bmpdt = new BitmapData();
                bmpdt.region = mRgnTempPict.Clone();
                bmpdt.region.Translate(mTempPict.Location.X, mTempPict.Location.Y);
                bmpdt.bmp = (Bitmap)mTempPict.Image.Clone();
                bmpdt.location = mTempPict.Location;
                mBgBitmaps.Add(bmpdt);
                DrawBmpToBg(bmpdt, mBmpBG);
                mTempPict.Visible = false;
                Size reqSize = new Size(mTempPict.Location.X + mTempPict.Width, mTempPict.Location.Y + mTempPict.Height);
                if (reqSize.Height > mBmpSize.Height) mBmpSize.Height = reqSize.Height;
                if (reqSize.Width > mBmpSize.Width) mBmpSize.Width = reqSize.Width;
            }
        }

        void FormNote_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsTempPictMoving)
            {
                Console.WriteLine("FormNote_MouseMove x:{0} y:{1} sender:{2}", e.Location.X, e.Location.Y, sender);
                if( (e.Location.X + mTempPict.Width/2 < this.mInkPicture.Width)
                 && (e.Location.X - mTempPict.Width / 2 > this.mInkPicture.Location.X)
                 && (e.Location.Y + mTempPict.Height / 2 < this.mInkPicture.Height)
                 && (e.Location.Y - mTempPict.Height / 2 > this.mInkPicture.Location.Y)
                    )
                {
                    mTempPict.Location = new Point(e.Location.X - mTempPict.Width/2, e.Location.Y - mTempPict.Height/2);
                }
            }
        }

        /*
        void mTempPict_MouseDown(object sender, MouseEventArgs e)
        {
            if (mIsTempPictMoving && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Console.WriteLine("mTempPict_MouseDown");
                mIsTempPictMoving = false;
                mTempPictMoveOffset = mTempPict.Location;
                mTempPictMoveOffset.Offset(e.Location);
            }
        }
        void mTempPict_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsTempPictMoving)
            {
                if (e.Location.X != 0 && e.Location.Y != 0)
                {
                    Console.WriteLine("mTempPict_MouseMove x:{0} y:{1} sender:{2}", e.Location.X, e.Location.Y, sender);
                    //mTempPict.Location = new Point(e.Location.X - mTempPictMoveOffset.X, e.Location.Y - mTempPictMoveOffset.Y);
                    mTempPict.Location = e.Location;
                }
            }
        }
        */
        public void CopyDesktopImageInClippedRegion(Region rg)
        {
            Console.WriteLine("CopyDesktopImageToClippedRegion");

            int screenX;
            int screenY;
            IntPtr hBmp;
            IntPtr hdcScreen = GetDC(GetDesktopWindow());
            IntPtr hdcCompatible = CreateCompatibleDC(hdcScreen);

            screenX = GetSystemMetrics(0);
            screenY = GetSystemMetrics(1);
            hBmp = CreateCompatibleBitmap(hdcScreen, screenX, screenY);

            if (hBmp != IntPtr.Zero)
            {
                IntPtr hOldBmp = (IntPtr)SelectObject(hdcCompatible, hBmp);
                BitBlt(hdcCompatible, 0, 0, screenX, screenY, hdcScreen, 0, 0, 13369376);

                SelectObject(hdcCompatible, hOldBmp);
                DeleteDC(hdcCompatible);
                ReleaseDC(GetDesktopWindow(), hdcScreen);

                if (mBmpClip != null) mBmpClip.Dispose();

                Bitmap bmpDt = System.Drawing.Image.FromHbitmap(hBmp);
                Graphics gd = Graphics.FromImage(bmpDt);
                RectangleF rc = rg.GetBounds(gd);
                mBmpClip = new Bitmap((int)rc.Width, (int)rc.Height);
                Graphics gt = Graphics.FromImage(mBmpClip);
                //gt.FillRectangle(new SolidBrush(Color.FromArgb(1, 0, 0)), new Rectangle(0, 0, (int)rc.Width, (int)rc.Height));
                if (mRgnTempPict != null) mRgnTempPict.Dispose();
                mRgnTempPict = rg.Clone();
                mRgnTempPict.Translate(-rc.X, -rc.Y);
                gt.SetClip(mRgnTempPict, System.Drawing.Drawing2D.CombineMode.Replace);
                RectangleF rc2 = rg.GetBounds(gt);
                gt.DrawImage(bmpDt, new Rectangle(0, 0, mBmpClip.Width, mBmpClip.Height), new Rectangle((int)rc.X, (int)rc.Y, (int)rc.Width, (int)rc.Height), GraphicsUnit.Pixel);
                mTempPict.Image = mBmpClip;
                mTempPict.Visible = true;
                mTempPict.Dock = DockStyle.None;
                mTempPict.Height = mBmpClip.Height;
                mTempPict.Width = mBmpClip.Width;

                gd.Dispose();
                gt.Dispose();
                
                DeleteObject(hBmp);
                GC.Collect();
            }
        }

        private void CopyRegion(Image imgSrc, Region rgSrc, Image imgDst, Point ptDst)
        {
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            this.Visible = false;
            FormSelRegion frm = new FormSelRegion();
            frm.ShowDialog();
            Region rg = frm.SelRegion;
            CopyDesktopImageInClippedRegion(rg);
            mIsTempPictMoving = true;
            mTempPict.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Visible = true;
            this.mInkPicture.Enabled = false;
        }

        private void FormNote_Load(object sender, EventArgs e)
        {
            DrawGrid();
        }

        private void FormNote_SizeChanged(object sender, EventArgs e)
        {
            DrawGrid();
        }

        private void DrawGrid()
        {
            Bitmap bmpBk = null;
            if(mBmpBG != null)
            {
                bmpBk= (Bitmap)mBmpBG.Clone();
                mBmpBG.Dispose();
            }

            Size sz = mBmpSize;
            if (mInkPicture.Width > sz.Width) sz.Width = mInkPicture.Width;
            if (mInkPicture.Height > sz.Height) sz.Height = mInkPicture.Height;
            mBmpBG = new Bitmap(sz.Width, sz.Height);
            
            Graphics g = Graphics.FromImage(mBmpBG);
            Pen pen = new Pen(Color.Brown);
            for (int x = 0; x < this.mInkPicture.Width; x += 25)
            {
                g.DrawLine(pen, x, 0, x, this.mInkPicture.Height);
            }
            for (int y = 0; y < this.mInkPicture.Height; y += 25)
            {
                g.DrawLine(pen, 0, y, this.mInkPicture.Width, y);
            }
            if (bmpBk != null)
            {
                g.DrawImage(bmpBk, new Point(0, 0));
                bmpBk.Dispose();
            }
            pen.Dispose();
            g.Dispose();
            mInkPicture.BackColor = Color.Wheat;
            mInkPicture.BackgroundImage = mBmpBG;
        }

        private void SaveStroke()
        {
            Console.WriteLine("SaveStroke");
            byte[] savedInk = mInkPicture.Ink.Save(PersistenceFormat.InkSerializedFormat);
            string s1 = System.Convert.ToBase64String(savedInk);
            Console.WriteLine("{0}", s1);
            byte[] savedInk64 = mInkPicture.Ink.Save(PersistenceFormat.Base64InkSerializedFormat);
            char[] chardata = Encoding.UTF8.GetChars(savedInk64);
            String s = new string(chardata, 7, chardata.Length - 8);
            Console.WriteLine("{0}", s);
        }
        private void SaveImage()
        {
            Console.WriteLine("SaveImage");
            //Rectangle rc =  new Rectangle(0, 0, mInkPicture.Width, mInkPicture.Height);
            //Bitmap bmp = new Bitmap(rc.Width, rc.Height);
            //Graphics g = Graphics.FromImage(bmp);
            //g.CopyFromScreen(rc.Location, new Point(0, 0), rc.Size);

            System.IO.Stream imgStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.AddExtension = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((imgStream = saveFileDialog1.OpenFile()) != null)
                {
                    mBmpBG.Save(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgStream.Close();
                }
            }
            //mInkPicture.Image.Save("test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        //Forms.KetPreview should be true
        private void FormNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C))
            {
                //CopyToClipboard();
            }
            else if (e.Control && (e.KeyCode == Keys.S))
            {
                SaveImage();
                SaveStroke();
            }
            else if (e.Control && (e.KeyCode == Keys.Z))
            {
                //Undo();
            }
            else if (e.KeyCode == Keys.F5)
            {
                /*
                if (mUseDesktopImgAsBG)
                {
                    CopyDesktopImage();
                }
                 */
            }

        }
    }
}
