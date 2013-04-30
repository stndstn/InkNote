using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Xml;
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

        public class BitmapPosData
        {
            public Region region;
            public Point location;
            public Bitmap bmp;

            public void Dispose()
            {
                if (bmp != null) bmp.Dispose();
                if (region != null) region.Dispose();
            }
        }

        //PictureBox mTempPict = null;
        Region mRgnTempPict = null;
        Bitmap mBmpClip = null;
        Bitmap mBmpBG = null;
        Bitmap mBmpTempBG = null;
        bool mIsTempPictMoving = false;
        Point mTempPictMoveOffset = Point.Empty;
        BitmapPosData mMovingBmpData = null;
        public InkPicture mInkPicture = null;
        Size mBmpSize = Size.Empty;
        List<BitmapPosData> mBgBitmaps = new List<BitmapPosData>();
        string dataPath = string.Empty;
        Palette mFormPalette = null;

        public Color mBgColor = Color.Wheat;
        public string Path
        {
            get { return dataPath; }
        }

        public FormNote(Palette palette, string path) : this(palette)
        {
            dataPath = path;
        }
        public FormNote(Palette palette)
        {
            InitializeComponent();
            mFormPalette = palette;
            //mTempPict = new PictureBox();
            //mTempPict.Visible = false;
            //mTempPict.MouseMove += new MouseEventHandler(mTempPict_MouseMove);
            //mTempPict.MouseDown += new MouseEventHandler(mTempPict_MouseDown);
            this.panel1.MouseMove += new MouseEventHandler(FormNote_MouseMove);
            this.panel1.MouseDown += new MouseEventHandler(FormNote_MouseDown);
            mInkPicture = new InkPicture();

            /*
            string s1 = "AEkdAi4qAwRIEEU1GRQyCACsFQLUuOJBMwgAgAwCQ7fiQRGrqtNBCiItgv2l+0bCVKm5VJQs3NixUTcogvy5+XQJZYsCwWVFipQA";
            string s2 = "AE4cA4CABB0CLioDBEgQRTUZFDIIAKwVAtS44kEzCACADAJDt+JBEauq00EKIi2C/aX7RsJUqblUlCzc2LFRNyiC/Ln5dAlliwLBZUWKlAA=";
            byte[] decoded1 = System.Convert.FromBase64String(s1);
            byte[] decoded2 = System.Convert.FromBase64String(s2);
            //mInkPicture.Ink.Load(decoded1);
            //mInkPicture.Refresh();
            mInkPicture.Ink.Load(decoded2);
            mInkPicture.Refresh();
            */
            this.panel1.Controls.Add(mInkPicture);
            mInkPicture.Dock = DockStyle.Fill;
            //this.mInkPicture.Controls.Add(mTempPict);
            mBmpSize = this.ClientSize;

        }

        Size DrawBmpToBg(BitmapPosData bmpdt, Image imgBg)
        {
            Graphics g = Graphics.FromImage(imgBg);
            g.SetClip(bmpdt.region, System.Drawing.Drawing2D.CombineMode.Replace);
            g.DrawImage(bmpdt.bmp, bmpdt.location);
            g.Dispose();
            Size reqSize = new Size(bmpdt.location.X + bmpdt.bmp.Width, bmpdt.location.Y + bmpdt.bmp.Height);
            if (reqSize.Height > mBmpSize.Height) mBmpSize.Height = reqSize.Height;
            if (reqSize.Width > mBmpSize.Width) mBmpSize.Width = reqSize.Width;
            return reqSize;
        }
        void FormNote_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("FormNote_MouseDown");
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (mIsTempPictMoving)
                {
                    mIsTempPictMoving = false;
                    //mInkPicture.Enabled = true;
                    if (mMovingBmpData != null)
                    {
                        mMovingBmpData.region.Translate(e.Location.X - (mMovingBmpData.location.X + mMovingBmpData.bmp.Width / 2), e.Location.Y - (mMovingBmpData.location.Y + mMovingBmpData.bmp.Height / 2));
                        mMovingBmpData.location.X = e.Location.X - mMovingBmpData.bmp.Width / 2;
                        mMovingBmpData.location.Y = e.Location.Y - mMovingBmpData.bmp.Height / 2;
                        DrawBmpToBg(mMovingBmpData, mBmpBG);
                        //BitmapPosData bmpdata = new BitmapPosData();
                        //bmpdata.bmp = new Bitmap(mMovingBmpData.bmp);
                        //bmpdata.location = mMovingBmpData.location;
                        //bmpdata.region = mMovingBmpData.region.Clone();
                        mBgBitmaps.Add(mMovingBmpData);
                        //mMovingBmpData.Dispose();
                        mMovingBmpData = null;
                    }
                        /*
                    else
                    {
                        BitmapPosData bmpdt = new BitmapPosData();
                        bmpdt.region = mRgnTempPict.Clone();
                        bmpdt.region.Translate(mTempPict.Location.X, mTempPict.Location.Y);
                        bmpdt.bmp = (Bitmap)mTempPict.Image.Clone();
                        bmpdt.location = mTempPict.Location;
                        mBgBitmaps.Add(bmpdt);
                        DrawBmpToBg(bmpdt, mBmpBG);
                    }
                         */
                    //mTempPict.Visible = false;
                    //mTempPict.Image.Dispose();
                    //Size reqSize = new Size(mTempPict.Location.X + mTempPict.Width, mTempPict.Location.Y + mTempPict.Height);
                    //if (reqSize.Height > mBmpSize.Height) mBmpSize.Height = reqSize.Height;
                    //if (reqSize.Width > mBmpSize.Width) mBmpSize.Width = reqSize.Width;
                }
                else
                {
                    if (mInkPicture.InkEnabled == false)
                    {
                        foreach (BitmapPosData bmpData in mBgBitmaps)
                        {
                            if (bmpData.region.IsVisible(e.Location))
                            {
                                mRgnTempPict = bmpData.region.Clone();
                                mRgnTempPict.Translate(-bmpData.location.X, -bmpData.location.Y);
                                //mTempPict.Cursor = System.Windows.Forms.Cursors.Hand;
                                //mTempPict.Image = bmpData.bmp;
                                //mTempPict.Visible = true;
                                //mTempPict.Size = bmpData.bmp.Size;
                                mMovingBmpData = bmpData;
                                mIsTempPictMoving = true;
                                mBgBitmaps.Remove(bmpData);
                                break;
                            }
                        }

                        //rebuild background image
                        mBmpBG.Dispose();
                        mBmpBG = null;
                        DrawBackGroundImageAndGrid();
                        if (mBmpTempBG != null)
                        {
                            mBmpTempBG.Dispose();
                            mBmpTempBG = null;
                        }
                        mBmpTempBG = new Bitmap(mBmpBG);
                    }
                }
            }
        }

        void FormNote_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsTempPictMoving)
            {
                Console.WriteLine("FormNote_MouseMove x:{0} y:{1} sender:{2}", e.Location.X, e.Location.Y, sender);
                if ((e.Location.X + mMovingBmpData.bmp.Width / 2 < this.mInkPicture.Width)
                 && (e.Location.X - mMovingBmpData.bmp.Width / 2 > this.mInkPicture.Location.X)
                 && (e.Location.Y + mMovingBmpData.bmp.Height / 2 < this.mInkPicture.Height)
                 && (e.Location.Y - mMovingBmpData.bmp.Height / 2 > this.mInkPicture.Location.Y)
                    )
                {
                    mMovingBmpData.region.Translate(e.Location.X - (mMovingBmpData.location.X + mMovingBmpData.bmp.Width / 2), e.Location.Y - (mMovingBmpData.location.Y + mMovingBmpData.bmp.Height / 2));
                    mMovingBmpData.location = new Point(e.Location.X - mMovingBmpData.bmp.Width / 2, e.Location.Y - mMovingBmpData.bmp.Height / 2);
                    mBmpBG.Dispose();
                    mBmpBG = new Bitmap(mBmpTempBG);
                    this.mInkPicture.BackgroundImage = mBmpBG;
                    //Graphics g = Graphics.FromImage(mBmpBG);
                    //g.DrawImage(mBmpTempBG, new Point(0, 0));
                    DrawBmpToBg(mMovingBmpData, mBmpBG);
                    this.mInkPicture.Invalidate();
                    //g.Dispose();
                    //mTempPict.Location = new Point(e.Location.X - mTempPict.Width/2, e.Location.Y - mTempPict.Height/2);
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
                //mTempPict.Image = mBmpClip;
                //mTempPict.Visible = true;
                //mTempPict.Dock = DockStyle.None;
                //mTempPict.Size = mBmpClip.Size;
                mMovingBmpData = new BitmapPosData();
                mMovingBmpData.bmp = mBmpClip;
                mMovingBmpData.location = new Point(0, 0);
                mMovingBmpData.region = mRgnTempPict;

                gd.Dispose();
                gt.Dispose();
                
                if (mBmpTempBG != null)
                {
                    mBmpTempBG.Dispose();
                    mBmpTempBG = null;
                }
                mBmpTempBG = new Bitmap(mBmpBG);

                DeleteObject(hBmp);
                GC.Collect();
            }
        }

        public void SelPict()
        {
            this.Visible = false;
            FormSelRegion frm = new FormSelRegion();
            frm.ShowDialog();
            Region rg = frm.SelRegion;
            CopyDesktopImageInClippedRegion(rg);
            mIsTempPictMoving = true;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            //mTempPict.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Visible = true;
            this.mInkPicture.Enabled = false;
        }

        private void FormNote_Load(object sender, EventArgs e)
        {
            DrawBackGroundImageAndGrid();
            if (dataPath != string.Empty)
            {
                LoadData(dataPath);
            }
        }

        private void FormNote_SizeChanged(object sender, EventArgs e)
        {
            DrawBackGroundImageAndGrid();
        }

        public void DrawBackGroundImageAndGrid()
        {
            //Bitmap bmpBk = null;
            if(mBmpBG != null)
            {
                //bmpBk= (Bitmap)mBmpBG.Clone();
                mBmpBG.Dispose();
            }

            Size sz = mBmpSize;
            if (mInkPicture.Width > sz.Width) sz.Width = mInkPicture.Width;
            if (mInkPicture.Height > sz.Height) sz.Height = mInkPicture.Height;
            mBmpBG = new Bitmap(sz.Width, sz.Height);
            
            Graphics g = Graphics.FromImage(mBmpBG);

            byte cA = mBgColor.A;
            byte cR = (byte)((mBgColor.R - 50) > 0 ? (mBgColor.R - 50) : 0);
            byte cG = (byte)((mBgColor.G - 50) > 0 ? (mBgColor.G - 50) : 0);
            byte cB = (byte)((mBgColor.B - 50) > 0 ? (mBgColor.B - 50) : 0);
            Color gridColor = Color.FromArgb(cA, cR, cG, cB);

            Pen pen = new Pen(gridColor);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            for (int x = 25; x < this.mInkPicture.Width; x += 25)
            {
                g.DrawLine(pen, x, 0, x, this.mInkPicture.Height);
            }
            for (int y = 25; y < this.mInkPicture.Height; y += 25)
            {
                g.DrawLine(pen, 0, y, this.mInkPicture.Width, y);
            }
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            g.DrawLine(pen, 0, 0, this.mInkPicture.Width, 0);
            g.DrawLine(pen, 0, 0, 0, this.mInkPicture.Height);
            g.DrawLine(pen, this.mInkPicture.Width - 1, 0, this.mInkPicture.Width - 1, this.mInkPicture.Height - 1);
            g.DrawLine(pen, 0, this.mInkPicture.Height - 1, this.mInkPicture.Width - 1, this.mInkPicture.Height - 1);
            //if (bmpBk != null)
            //{
            //    g.DrawImage(bmpBk, new Point(0, 0));
            //    bmpBk.Dispose();
            //}
            pen.Dispose();
            g.Dispose();

            foreach (BitmapPosData bmpData in mBgBitmaps)
            {
                DrawBmpToBg(bmpData, mBmpBG);
            }

            mInkPicture.BackColor = mBgColor;
            mInkPicture.BackgroundImage = mBmpBG;
        }

        public void LoadData(string path)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(path);
            XmlNodeList dataNodes = doc.GetElementsByTagName("data");
            if (dataNodes.Count > 0)
            {
                XmlNode dataNode = dataNodes[0];
                if (dataNode.Attributes["x"] != null && dataNode.Attributes["y"] != null)
                    this.Location = new Point(Int32.Parse(dataNode.Attributes["x"].Value), Int32.Parse(dataNode.Attributes["y"].Value));
                if (dataNode.Attributes["width"] != null && dataNode.Attributes["height"] != null)
                    this.Size = new Size(Int32.Parse(dataNode.Attributes["width"].Value), Int32.Parse(dataNode.Attributes["height"].Value));
                if (dataNode.Attributes["bgcolor"] != null)
                {
                    try
                    {
                        this.mBgColor = Color.FromArgb(Int32.Parse(dataNode.Attributes["bgcolor"].Value));
                    }
                    catch (Exception e)
                    {
                        this.mBgColor = Color.Wheat;
                    }
                }
            }
            XmlNodeList strokeNodes = doc.GetElementsByTagName("stroke");
            if (strokeNodes.Count > 0)
            {
                XmlNode strokeNode = strokeNodes[0];
                XmlCDataSection cdata_stroke = (XmlCDataSection)strokeNode.FirstChild;
                byte[] decoded = System.Convert.FromBase64String(cdata_stroke.Data);
                mInkPicture.Ink.Load(decoded);
                mInkPicture.Refresh();
            }

            XmlNodeList imageNodes = doc.GetElementsByTagName("image");
            foreach(XmlNode imageNode in imageNodes)
            {
                BitmapPosData bmpdt = new BitmapPosData();
                bmpdt.location.X = Int32.Parse(imageNode.Attributes["x"].Value);
                bmpdt.location.Y = Int32.Parse(imageNode.Attributes["y"].Value);

                foreach(XmlNode node in imageNode.ChildNodes)
                {
                    if (node.Name == "region")
                    {
                        XmlCDataSection cdata_region = (XmlCDataSection)node.FirstChild;
                        byte[] decoded = System.Convert.FromBase64String(cdata_region.Data);
                        Region regionTmp = new Region();
                        System.Drawing.Drawing2D.RegionData region2Data = regionTmp.GetRegionData();
                        region2Data.Data = decoded;
                        bmpdt.region = new Region(region2Data);
                        regionTmp.Dispose();
                    }
                    else if (node.Name == "bmpdata")
                    {
                        XmlCDataSection cdata_bmp = (XmlCDataSection)node.FirstChild;
                        byte[] decoded = System.Convert.FromBase64String(cdata_bmp.Data);
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            ms.Write(decoded, 0, decoded.Length);
                            Bitmap bmpTmp = (Bitmap)Bitmap.FromStream(ms, false, true);
                            // Color format of a bitmap loaded from stream is 32rgb,
                            // but default bitmap format is 32Argb
                            // it cause a problem when save this bitmap.
                            // so, create new bitmap and copy its color one by one
                            Bitmap bmp = new Bitmap(bmpTmp.Width, bmpTmp.Height);
                            for (int x = 0; x < bmpTmp.Width; x++)
                            {
                                for (int y = 0; y < bmpTmp.Height; y++)
                                {
                                    Color c = bmpTmp.GetPixel(x, y);
                                    bmp.SetPixel(x, y, c);
                                }
                            }
                            bmpdt.bmp = (Bitmap)bmp;
                        }
                        /*
                        System.IO.FileStream fsw = System.IO.File.OpenWrite("temp.bmp");
                        int len = decoded.Length;
                        fsw.Write(decoded, 0, len);
                        fsw.Close();
                        System.IO.FileStream fsr = System.IO.File.OpenRead("temp.bmp");
                        bmpdt.bmp = (Bitmap)Image.FromStream(fsr);
                        fsr.Close();
                         */
                    }
                }
                mBgBitmaps.Add(bmpdt);
                DrawBmpToBg(bmpdt, mBmpBG);

            }

        }
        public void Save()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            XmlElement dataNode = doc.CreateElement("data");
            dataNode.SetAttribute("x", this.Location.X.ToString());
            dataNode.SetAttribute("y", this.Location.Y.ToString());
            dataNode.SetAttribute("width", this.Size.Width.ToString());
            dataNode.SetAttribute("height", this.Size.Height.ToString());
            dataNode.SetAttribute("bgcolor", this.mBgColor.ToArgb().ToString());

            doc.AppendChild(dataNode);
            XmlNode nodeStroke = doc.CreateElement("stroke");
            dataNode.AppendChild(nodeStroke);
            byte[] savedInk = mInkPicture.Ink.Save(PersistenceFormat.InkSerializedFormat);
            string str_stroke = System.Convert.ToBase64String(savedInk);
            XmlCDataSection cdataStroke = doc.CreateCDataSection(str_stroke);
            nodeStroke.AppendChild(cdataStroke);
            XmlNode nodeImages = doc.CreateElement("images");
            foreach (BitmapPosData bmpData in mBgBitmaps)
            {
                XmlElement elmImage = doc.CreateElement("image");
                elmImage.SetAttribute("x", bmpData.location.X.ToString());
                elmImage.SetAttribute("y", bmpData.location.Y.ToString());
                
                XmlElement elmRegion = doc.CreateElement("region");
                byte[] regionData = bmpData.region.GetRegionData().Data;
                string str_rgn = System.Convert.ToBase64String(regionData);
                XmlCDataSection cdata_rgn = doc.CreateCDataSection(str_rgn);
                elmRegion.AppendChild(cdata_rgn);
                elmImage.AppendChild(elmRegion);

                XmlElement elmBmpData = doc.CreateElement("bmpdata");
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bmpData.bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    string str_bmp = System.Convert.ToBase64String(ms.ToArray());
                    XmlCDataSection cdata_bmp = doc.CreateCDataSection(str_bmp);
                    elmBmpData.AppendChild(cdata_bmp);
                }
                elmImage.AppendChild(elmBmpData);
                nodeImages.AppendChild(elmImage);
            }
            dataNode.AppendChild(nodeImages);
            if (dataPath != string.Empty)
            {
                doc.Save(dataPath);
            }
            else
            {
                string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InkNote";
                if (System.IO.Directory.Exists(dirPath) == false)
                {
                    System.IO.DirectoryInfo dirInfo = System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InkNote");
                }
                string name = Guid.NewGuid().ToString();
                dataPath = dirPath + "\\" + name + ".ikn";
                doc.Save(dataPath);
                /*
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.DefaultExt = "ikn";
                saveFileDialog1.Filter = "InkNote files (*.ikn)|*.ikn";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.InitialDirectory = dirPath;
                //saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //saveFileDialog1.InitialDirectory = System.IO.Directory.GetCurrentDirectory();

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    dataPath = saveFileDialog1.FileName;
                    doc.Save(dataPath);
                }
                 */
            }
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
                CopyToClipboard();
            }
            else if (e.Control && (e.KeyCode == Keys.S))
            {
                Save();
            }
            else if (e.Control && (e.KeyCode == Keys.Z))
            {
                Undo();
            }
        }

        public void TurnAroundPenSelMode(PictureBox btn)
        {
            mInkPicture.InkEnabled = !mInkPicture.InkEnabled;
            if (mInkPicture.InkEnabled)
            {
                this.mInkPicture.Enabled = true;
                btn.Image = Properties.Resources.Image13;
            }
            else
            {
                this.mInkPicture.Enabled = false;
                btn.Image = Properties.Resources.Image1;
            }
        }

        private void FormNote_Activated(object sender, EventArgs e)
        {
            Size sizePrev = this.Size;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Size sizeAfter = this.Size;
            int dx = (sizeAfter.Width - sizePrev.Width) / 2;
            int dy = sizeAfter.Height - sizePrev.Height - dx;
            Point pt = this.Location;
            pt.Offset(-dx, -dy);
            this.Location = pt;
            mFormPalette.activeNote = this;
        }

        private void FormNote_Deactivate(object sender, EventArgs e)
        {
            Size sizePrev = this.Size;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Size sizeAfter = this.Size;
            int dx = (sizePrev.Width - sizeAfter.Width) / 2;
            int dy = sizePrev.Height - sizeAfter.Height - dx;
            Point pt = this.Location;
            pt.Offset(dx, dy);
            this.Location = pt;
        }

        private void FormNote_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Delete this note?", "InkNOte", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                mFormPalette.deleteNote(this);
                if (Path != string.Empty)
                {
                    System.IO.File.Delete(Path);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void CopyToClipboard()
        {
            Console.WriteLine("CopyToClipboard");
            Size size = mInkPicture.Size;
            Point ptSrc = mInkPicture.PointToScreen(new Point(0, 0));
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(ptSrc, new Point(0, 0), size);
            Clipboard.SetImage(bmp);
        }
        public void Undo()
        {
            int count = mInkPicture.Ink.Strokes.Count;
            if (count > 0)
            {
                Stroke lastStroke = mInkPicture.Ink.Strokes[count - 1];
                mInkPicture.Ink.DeleteStroke(lastStroke);
                mInkPicture.Refresh();
            }
        }
        
    }
}
