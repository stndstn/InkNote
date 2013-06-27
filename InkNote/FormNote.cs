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
        /*
        bool mIsImagePicked = false;
        public bool IsImagePicked
        {
            get { return mIsImagePicked; }
        }
        */
        Bitmap mBmpBG = null;
        Bitmap mBmpTempBG = null;
        BitmapPosData mPickedBmpDataTmp = null;
        Point mPickedImageOffset = Point.Empty;
        public BitmapPosData mPickedBmpDataRef = null;
        //BitmapPosData mPickedBmpDataBackup = null;
        public InkPicture mInkPicture = null;
        Size mBmpSize = Size.Empty;
        List<BitmapPosData> mBgBitmaps = new List<BitmapPosData>();
        string dataPath = string.Empty;
        Palette mFormPalette = null;
        private Size mActivatedFormSize = Size.Empty;
        public Point mPreviousLocation = Point.Empty;
        private bool mIsActivated = false;

        public bool IsImagePicked
        {
            get { return (mPickedBmpDataRef == null ? false : true); }
        }

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
            this.panel1.MouseMove += new MouseEventHandler(panel1_MouseMove);
            this.panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);
            this.panel1.MouseUp += new MouseEventHandler(panel1_MouseUp);
            mInkPicture = new InkPicture();
            Palette.DEFAULT_PEN_HEIGHT = mInkPicture.DefaultDrawingAttributes.Height;
            Palette.DEFAULT_PEN_WIDTH = mInkPicture.DefaultDrawingAttributes.Width;
            this.panel1.Controls.Add(mInkPicture);
            mInkPicture.Dock = DockStyle.Fill;
            mBmpSize = this.ClientSize;
            mInkPicture.SelectionChanged += new InkOverlaySelectionChangedEventHandler(mInkPicture_SelectionChanged);
            mInkPicture.Stroke += new InkCollectorStrokeEventHandler(mInkPicture_Stroke);
        }

        void mInkPicture_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            mFormPalette.OnInkDrawMode();
        }

        public void setInkMode(Palette.MODE mode)
        {
            switch (mode)
            {
                case Palette.MODE.INK_DRAW:
                    mInkPicture.InkEnabled = true;
                    mInkPicture.Enabled = true;
                    mInkPicture.EditingMode = InkOverlayEditingMode.Ink;
                    break;
                case Palette.MODE.INK_SEL:
                    mInkPicture.InkEnabled = true;
                    mInkPicture.Enabled = true;
                    mInkPicture.EditingMode = InkOverlayEditingMode.Select;
                    break;
                case Palette.MODE.INK_POINT_ERASE:
                    mInkPicture.InkEnabled = true;
                    mInkPicture.Enabled = true;
                    mInkPicture.EditingMode = InkOverlayEditingMode.Delete;
                    mInkPicture.EraserMode = InkOverlayEraserMode.PointErase;
                    break;
                case Palette.MODE.INK_STROKE_ERASE:
                    mInkPicture.InkEnabled = true;
                    mInkPicture.Enabled = true;
                    mInkPicture.EditingMode = InkOverlayEditingMode.Delete;
                    mInkPicture.EraserMode = InkOverlayEraserMode.StrokeErase;
                    break;
                case Palette.MODE.IMG_PICK:
                    mInkPicture.InkEnabled = false;
                    mInkPicture.Enabled = false;
                    mInkPicture.EditingMode = InkOverlayEditingMode.Ink;
                    break;
                default:
                    break;
            }
        }

        void mInkPicture_SelectionChanged(object sender, EventArgs e)
        {
            if (mInkPicture.Selection.Count > 0)
            {
                mFormPalette.OnInkSelected();
            }
            else
            {
                mFormPalette.OnInkSelectMode();
            }
        }
        /*
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
         * */
        Size DrawBmpToBg(BitmapPosData bmpdt, Image imgBg)
        {
            Graphics g = Graphics.FromImage(imgBg);
            g.SetClip(bmpdt.region, System.Drawing.Drawing2D.CombineMode.Replace);
            g.DrawImage(bmpdt.bitmap, bmpdt.location);
            Size reqSize = new Size(bmpdt.location.X + bmpdt.bitmap.Width, bmpdt.location.Y + bmpdt.bitmap.Height);
            if (reqSize.Height > mBmpSize.Height) mBmpSize.Height = reqSize.Height;
            if (reqSize.Width > mBmpSize.Width) mBmpSize.Width = reqSize.Width;

            if (bmpdt.isPicked)
            {
                g.ResetClip();
                Color drawC = Color.FromArgb(255, 255 - mBgColor.R, 255 - mBgColor.G, 255 - mBgColor.B);
                Pen p = new Pen(drawC, 2);
                g.DrawPolygon(p, bmpdt.Points);
            }
            g.Dispose();

            return reqSize;
        }
        public void CopyPickedImage(BitmapPosData pickedData)
        {
            BitmapPosData dataNew = new BitmapPosData(pickedData);
            dataNew.location.X = 0;
            dataNew.location.Y = 0;
            dataNew.region.Translate(-pickedData.location.X, -pickedData.location.Y);
            mBgBitmaps.Add(dataNew);
            DrawBackGroundImageAndGrid();
        }
        void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("panel1_MouseDown");
            bool needRedraw = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (mInkPicture.InkEnabled == false)
                {
                    if (mPickedBmpDataRef != null)
                    {
                        mPickedBmpDataRef = null;
                    }
                    if (mPickedBmpDataTmp != null)
                    {
                        mPickedBmpDataTmp.Dispose();
                        mPickedBmpDataTmp = null;
                    }
                    foreach (BitmapPosData bmpData in mBgBitmaps)
                    {
                        if (bmpData.region.IsVisible(e.Location))
                        {
                            mPickedBmpDataRef = bmpData;
                            needRedraw = true;
                            //mPickedBmpData.ptGripDelta = new Point(e.X - bmpData.location.X, e.Y - bmpData.location.Y);
                            //mBgBitmaps.Remove(bmpData);
                            bmpData.isPicked = true;
                            mPickedBmpDataTmp = new BitmapPosData(bmpData);
                            mPickedBmpDataTmp.ptGripDelta = new Point(e.X - bmpData.location.X, e.Y - bmpData.location.Y);
                            //make transparent image
                            for (int y = 0; y < mPickedBmpDataTmp.bitmap.Height; y++)
                            {
                                for (int x = 0; x < mPickedBmpDataTmp.bitmap.Width; x++)
                                {
                                    Color c = mPickedBmpDataTmp.bitmap.GetPixel(x, y);
                                    Color cc = Color.FromArgb(127, c.R, c.G, c.B);
                                    mPickedBmpDataTmp.bitmap.SetPixel(x, y, cc);
                                }
                            }
                            //break;
                        }
                        else
                        {
                            if (bmpData.isPicked)
                            {
                                bmpData.isPicked = false;
                                needRedraw = true;
                            }
                        }
                    }

                    if (needRedraw)
                    {
                        //rebuild background image
                        DrawBackGroundImageAndGrid();

                        if (IsImagePicked)
                        {
                            mFormPalette.OnImagePicked();
                        }
                    }
                    /*
                    else if (mFormPalette.IsImageCopied)
                    {
                        if (mFormPalette.mCopiedBmpData != null)
                        {
                            mFormPalette.mCopiedBmpData.region.Translate(
                                e.Location.X - (mFormPalette.mCopiedBmpData.location.X + mFormPalette.mCopiedBmpData.bitmap.Width / 2), 
                                e.Location.Y - (mFormPalette.mCopiedBmpData.location.Y + mFormPalette.mCopiedBmpData.bitmap.Height / 2));
                            mFormPalette.mCopiedBmpData.location.X = e.Location.X - mFormPalette.mCopiedBmpData.bitmap.Width / 2;
                            mFormPalette.mCopiedBmpData.location.Y = e.Location.Y - mFormPalette.mCopiedBmpData.bitmap.Height / 2;
                            DrawBmpToBg(mFormPalette.mCopiedBmpData, mBmpBG);
                            this.mInkPicture.BackgroundImage = mBmpBG;
                            mBgBitmaps.Add(mFormPalette.mCopiedBmpData);
                            mFormPalette.mCopiedBmpData = null;
                            mFormPalette.OnImagePickMode();
                        }
                    }
                     */
                }
            }
        }

        void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine("panel1_MouseUp");
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (IsImagePicked)
                {
                    if (mPickedBmpDataRef != null)
                    {
                        //BitmapPosData data = new BitmapPosData(mPickedBmpDataRef);
                        //data.region = mPickedBmpDataTmp.region.Clone();
                        //data.location = mPickedBmpDataTmp.location;
                        //DrawBmpToBg(data, mBmpBG);
                        mPickedBmpDataRef.region = mPickedBmpDataTmp.region.Clone();
                        mPickedBmpDataRef.location = mPickedBmpDataTmp.location;
                        DrawBackGroundImageAndGrid();
                        mInkPicture.BackgroundImage = mBmpBG;
                        //mBgBitmaps.Add(data);
                        mFormPalette.OnImagePickMode();
                    }
                }
            }
        }

        void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("panel1_MouseMove x:{0} y:{1} sender:{2}", e.Location.X, e.Location.Y, sender);
            if (IsImagePicked && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if ((e.X < this.mInkPicture.Width)
                 && (e.X > this.mInkPicture.Location.X)
                 && (e.Y < this.mInkPicture.Height)
                 && (e.Y > this.mInkPicture.Location.Y)
                    )
                {
                    mPickedBmpDataTmp.region.Translate(
                        e.X - (mPickedBmpDataTmp.location.X + mPickedBmpDataTmp.ptGripDelta.X),
                        e.Y - (mPickedBmpDataTmp.location.Y + mPickedBmpDataTmp.ptGripDelta.Y));
                    mPickedBmpDataTmp.location = new Point(
                        e.X - mPickedBmpDataTmp.ptGripDelta.X,
                        e.Y - mPickedBmpDataTmp.ptGripDelta.Y);
                    if (mBmpTempBG != null)
                    {
                        mBmpTempBG.Dispose();
                    }
                    mBmpTempBG = new Bitmap(mBmpBG);
                    DrawBmpToBg(mPickedBmpDataTmp, mBmpTempBG);
                    this.mInkPicture.BackgroundImage = mBmpTempBG;
                }
            }
                /*
            else if (mFormPalette.IsImageCopied && mIsActivated)
            {
                if ((e.X + mFormPalette.mCopiedBmpData.bitmap.Width / 2 < this.mInkPicture.Width)
                 && (e.X - mFormPalette.mCopiedBmpData.bitmap.Width / 2 > this.mInkPicture.Location.X)
                 && (e.Y + mFormPalette.mCopiedBmpData.bitmap.Height / 2 < this.mInkPicture.Height)
                 && (e.Y - mFormPalette.mCopiedBmpData.bitmap.Height / 2 > this.mInkPicture.Location.Y)
                    )
                {
                    mFormPalette.mCopiedBmpData.region.Translate(e.Location.X - (mFormPalette.mCopiedBmpData.location.X + mFormPalette.mCopiedBmpData.bitmap.Width / 2), e.Location.Y - (mFormPalette.mCopiedBmpData.location.Y + mFormPalette.mCopiedBmpData.bitmap.Height / 2));
                    mFormPalette.mCopiedBmpData.location = new Point(e.Location.X - mFormPalette.mCopiedBmpData.bitmap.Width / 2, e.Location.Y - mFormPalette.mCopiedBmpData.bitmap.Height / 2);
                    if (mBmpTempBG != null)
                    {
                        mBmpTempBG.Dispose();
                    }
                    mBmpTempBG = new Bitmap(mBmpBG);
                    DrawBmpToBg(mFormPalette.mCopiedBmpData, mBmpTempBG);
                    this.mInkPicture.BackgroundImage = mBmpTempBG;
                }
            }
                 */
        }
        /*
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

                        Bitmap bmpDt = System.Drawing.Image.FromHbitmap(hBmp);
                        Graphics gd = Graphics.FromImage(bmpDt);
                        RectangleF rc = rg.GetBounds(gd);
                        Bitmap bmpClip = new Bitmap((int)rc.Width, (int)rc.Height);
                        Graphics gt = Graphics.FromImage(bmpClip);
                        Region rgnTempPict = rg.Clone();
                        rgnTempPict.Translate(-rc.X, -rc.Y);
                        gt.SetClip(rgnTempPict, System.Drawing.Drawing2D.CombineMode.Replace);
                        RectangleF rc2 = rg.GetBounds(gt);
                        gt.DrawImage(bmpDt, new Rectangle(0, 0, bmpClip.Width, bmpClip.Height), new Rectangle((int)rc.X, (int)rc.Y, (int)rc.Width, (int)rc.Height), GraphicsUnit.Pixel);
                        mPickedBmpData = new BitmapPosData();
                        mPickedBmpData.bmp = bmpClip;
                        mPickedBmpData.location = new Point(0, 0);
                        mPickedBmpData.region = rgnTempPict.Clone();

                        //temp image for moving (transparent) 
                        if (mPickedBmpDataTmp != null)
                        {
                            mPickedBmpDataTmp.Dispose();
                            mPickedBmpDataTmp = null;
                        }
                        mPickedBmpDataTmp = new BitmapPosData();
                        mPickedBmpDataTmp.location = mPickedBmpData.location;
                        mPickedBmpDataTmp.region = mPickedBmpData.region.Clone();
                        mPickedBmpDataTmp.bmp = new Bitmap(mPickedBmpData.bmp);
                        for (int y = 0; y < mPickedBmpDataTmp.bmp.Height; y++)
                        {
                            for (int x = 0; x < mPickedBmpDataTmp.bmp.Width; x++)
                            {
                                Color c = mPickedBmpDataTmp.bmp.GetPixel(x, y);
                                Color cc = Color.FromArgb(127, c.R, c.G, c.B);
                                mPickedBmpDataTmp.bmp.SetPixel(x, y, cc);
                            }
                        }

                        rgnTempPict.Dispose();

                        gd.Dispose();
                        gt.Dispose();

                        DeleteObject(hBmp);
                        GC.Collect();
                    }
                }
                public void SelPict()
                {
                    Console.WriteLine("SelPict");
                    //this.Visible = false;
                    foreach (Form c in this.MdiParent.MdiChildren)
                    {
                        c.Visible = false;
                    }
                    try
                    {
                        DialogResult res = System.Windows.Forms.DialogResult.Cancel;
                        FormSelRegion frm = new FormSelRegion();
                        do
                        {
                            res = frm.ShowDialog();
                            Console.WriteLine("FormSelRegion.DialogResult is {0}", res.ToString());
                        } while (res == System.Windows.Forms.DialogResult.No);
                        if (res == System.Windows.Forms.DialogResult.Yes)
                        {
                            Console.WriteLine("FormSelRegion.DialogResult==Yes");
                            CopyDesktopImageInClippedRegion(frm.SelRegion);

                            foreach (Form c in this.MdiParent.MdiChildren)
                            {
                                c.Visible = true;
                                if (c.GetType() == typeof(FormNote))
                                {
                                    FormNote note = (FormNote)c;
                                    note.Location = note.mPreviousLocation;
                                }
                            }
                            mIsImagePicked = true;
                            this.Cursor = System.Windows.Forms.Cursors.Hand;
                            this.mInkPicture.Enabled = false;
                            this.mInkPicture.InkEnabled = false;
                            this.Activate();
                        }
                        this.Visible = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
        */
        private void FormNote_Load(object sender, EventArgs e)
        {
            if (dataPath != string.Empty)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                LoadData(dataPath);
            }
            else
            {
                //new memo
                DrawBackGroundImageAndGrid();
                mActivatedFormSize = this.Size;
            }
        }

        private void FormNote_SizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("FormNote_SizeChanged Location x={0} y={1}", this.Location.X, this.Location.Y);
            DrawBackGroundImageAndGrid();
        }

        public void DrawBackGroundImageAndGrid()
        {
            Console.WriteLine("DrawBackGroundImageAndGrid");
            if (mBmpBG != null)
            {
                mBmpBG.Dispose();
            }

            if (mInkPicture == null) return;

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
            Console.WriteLine("LoadData >>");
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(path);
            XmlNodeList dataNodes = doc.GetElementsByTagName("data");
            if (dataNodes.Count > 0)
            {
                XmlNode dataNode = dataNodes[0];
                int x = 0;
                int y = 0;
                int w = 0;
                int h = 0;
                if (dataNode.Attributes["x"] != null && dataNode.Attributes["y"] != null)
                {
                    x = Int32.Parse(dataNode.Attributes["x"].Value);
                    y = Int32.Parse(dataNode.Attributes["y"].Value);
                }
                if (dataNode.Attributes["width"] != null && dataNode.Attributes["height"] != null)
                {
                    w = Int32.Parse(dataNode.Attributes["width"].Value);
                    h = Int32.Parse(dataNode.Attributes["height"].Value);
                }
                x = x < 0 ? 0 : x;
                x = (x + w) > Screen.PrimaryScreen.WorkingArea.Width ? Screen.PrimaryScreen.WorkingArea.Width - w : x;
                y = y < 0 ? 0 : y;
                y = (y + h) > Screen.PrimaryScreen.WorkingArea.Height ? Screen.PrimaryScreen.WorkingArea.Height - h : y;
                this.Location = new Point(x, y);
                this.Size = new Size(w, h);

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
                Bitmap bmp = null;
                Region rgn = null;
                Point loc = new Point();
                Point[] ptPolygon = null;
                loc.X = Int32.Parse(imageNode.Attributes["x"].Value);
                loc.Y = Int32.Parse(imageNode.Attributes["y"].Value);


                foreach(XmlNode node in imageNode.ChildNodes)
                {
                    if (node.Name == "region")
                    {
                        XmlCDataSection cdata_region = (XmlCDataSection)node.FirstChild;
                        byte[] decoded = System.Convert.FromBase64String(cdata_region.Data);
                        Region regionTmp = new Region();
                        System.Drawing.Drawing2D.RegionData region2Data = regionTmp.GetRegionData();
                        region2Data.Data = decoded;
                        rgn = new Region(region2Data);
                        regionTmp.Dispose();
                    }
                    else if (node.Name == "polygon")
                    {
                        string[] xdata = null;
                        string[] ydata = null;
                        foreach (XmlNode c in node.ChildNodes)
                        {
                            if(c.Name == "xdata")
                            {
                                xdata = c.InnerText.Split(',');                            
                            }
                            else if (c.Name == "ydata")
                            {
                                ydata = c.InnerText.Split(',');
                            }
                        }
                        if (xdata != null && ydata != null)
                        {
                            int count = xdata.Length;
                            ptPolygon = new Point[count];
                            for (int i = 0; i < count; i++)
                            {
                                int x = Convert.ToInt32(xdata[i]);
                                int y = Convert.ToInt32(ydata[i]);
                                ptPolygon[i] = new Point(x, y);
                            }
                        }
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
                            bmp = new Bitmap(bmpTmp.Width, bmpTmp.Height);
                            for (int x = 0; x < bmpTmp.Width; x++)
                            {
                                for (int y = 0; y < bmpTmp.Height; y++)
                                {
                                    Color c = bmpTmp.GetPixel(x, y);
                                    bmp.SetPixel(x, y, c);
                                }
                            }
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
                BitmapPosData bmpdt = new BitmapPosData(bmp, rgn, loc, ptPolygon);
                mBgBitmaps.Add(bmpdt);
                DrawBmpToBg(bmpdt, mBmpBG);

            }
            Console.WriteLine("<< LoadData");
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

                XmlElement elmPolygon = doc.CreateElement("polygon");
                XmlElement elmXData = doc.CreateElement("xdata");
                XmlElement elmYData = doc.CreateElement("ydata");
                StringBuilder xdata = new StringBuilder();
                StringBuilder ydata = new StringBuilder();
                for(int i = 0; i < bmpData.ptPolygon.Length; i++)
                {
                    if (i > 0)
                    {
                        xdata.Append(",");
                        ydata.Append(",");
                    }
                    xdata.Append(bmpData.ptPolygon[i].X);
                    ydata.Append(bmpData.ptPolygon[i].Y);
                }
                elmXData.InnerText = xdata.ToString();
                elmYData.InnerText = ydata.ToString();
                elmPolygon.AppendChild(elmXData);
                elmPolygon.AppendChild(elmYData);
                elmImage.AppendChild(elmPolygon);

                XmlElement elmBmpData = doc.CreateElement("bmpdata");
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bmpData.bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
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
            }
        }
        /*
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
         */
/*
        private void SaveImage()
        {
            Console.WriteLine("SaveImage");

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
*/
        //Forms.KetPreview should be true
        private void FormNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C))
            {
                CopyToClipboard();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeletePickedImage();
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

        private void FormNote_Activated(object sender, EventArgs e)
        {
            mIsActivated = true;
            mFormPalette.mActiveNote = this;
            mFormPalette.MdiParent.Visible = true;
            mFormPalette.SetVisiblePaletteAndNotes();

            Console.WriteLine("FormNote_Activated");
            Console.WriteLine("Location x={0}, y={1}", this.Location.X, this.Location.Y);

            UpdateBorderStyle();

            mFormPalette.Left = this.Left;
            mFormPalette.Top = this.Bottom;
        }

        public void UpdateBorderStyle()
        {
            if (mIsActivated)
            {
                if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.SizableToolWindow)
                {
                    Size sizePrev = this.Size;
                    Console.WriteLine("sizePrev w={0}, h={1}", sizePrev.Width, sizePrev.Height);
                    Size tempActivatedSize = mActivatedFormSize;
                    Console.WriteLine("tempActivatedSize w={0}, h={1}", tempActivatedSize.Width, tempActivatedSize.Height);
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                    if (tempActivatedSize != Size.Empty)
                    {
                        mActivatedFormSize = tempActivatedSize;
                    }
                    this.Size = mActivatedFormSize;
                    Size sizeAfter = this.Size;
                    Console.WriteLine("sizeAfter w={0}, h={1}", sizeAfter.Width, sizeAfter.Height);
                    int dx = (sizeAfter.Width - sizePrev.Width) / 2;
                    int dy = sizeAfter.Height - sizePrev.Height - dx;
                    Point pt = this.Location;
                    Console.WriteLine("Location x={0}, y={1}", this.Location.X, this.Location.Y);
                    pt.Offset(-dx, -dy);
                    this.Location = pt;
                    Console.WriteLine("Location x={0}, y={1}", this.Location.X, this.Location.Y);
                }
            }
            else
            {
                if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                {
                    Size sizePrev = this.Size;
                    Console.WriteLine("sizePrev w={0}, h={1}", sizePrev.Width, sizePrev.Height);
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    Size sizeAfter = this.Size;
                    Console.WriteLine("sizeAfter w={0}, h={1}", sizeAfter.Width, sizeAfter.Height);
                    int dx = (sizePrev.Width - sizeAfter.Width) / 2;
                    int dy = sizePrev.Height - sizeAfter.Height - dx;
                    Point pt = this.Location;
                    Console.WriteLine("Location x={0}, y={1}", this.Location.X, this.Location.Y);
                    pt.Offset(dx, dy);
                    this.Location = pt;
                    Console.WriteLine("Location x={0}, y={1}", this.Location.X, this.Location.Y);
                }
            }
        }

        private void FormNote_Deactivate(object sender, EventArgs e)
        {
            Console.WriteLine("FormNote_Deactivate");
            Console.WriteLine("Location x={0}, y={1}", this.Location.X, this.Location.Y);
            mIsActivated = false;
            UpdateBorderStyle();
        }

        private void FormNote_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mFormPalette.isClosing == false)
            {
                if (MessageBox.Show("Delete this note?", "InkNote", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
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
            else
            {
                /*
                if (mInkPicture != null)
                {
                    mInkPicture.Dispose();
                    mInkPicture = null;
                }
                 */
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
        /*
        public void DeleteSelectedStrokes()
        {
            if (mInkPicture.EditingMode == InkOverlayEditingMode.Select)
            {
                mInkPicture.Ink.DeleteStrokes(mInkPicture.Selection);
            }
        }
         */
        public void DeletePickedImage()
        {
            if (IsImagePicked)
            {
                //dispose selected image
                if (mPickedBmpDataRef != null)
                {
                    foreach(BitmapPosData data in mBgBitmaps)
                    {
                        if (data.bitmap == mPickedBmpDataRef.bitmap
                        && data.location == mPickedBmpDataRef.location
                        && data.region == mPickedBmpDataRef.region)
                        {
                            bool bRet = mBgBitmaps.Remove(data);
                            mPickedBmpDataRef = null;
                            break;
                        }
                    }
                }

                //rebuild background image
                DrawBackGroundImageAndGrid();
            }
        }

        private void FormNote_Resize(object sender, EventArgs e)
        {
            Console.WriteLine("FormNote_Resize Location x={0} y={1}", this.Location.X, this.Location.Y);
            if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.SizableToolWindow)
            {
                mActivatedFormSize.Width = this.Width;
                mActivatedFormSize.Height = this.Height;
                Console.WriteLine(" new size w={0} h={1}", mActivatedFormSize.Width, mActivatedFormSize.Height);
            }
        }

        private void FormNote_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false)
                mPreviousLocation = this.Location;
        }

    }
}
