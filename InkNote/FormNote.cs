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

        public class BitmapPosData : IDisposable
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
            this.panel1.MouseMove += new MouseEventHandler(FormNote_MouseMove);
            this.panel1.MouseDown += new MouseEventHandler(FormNote_MouseDown);
            mInkPicture = new InkPicture();
            this.panel1.Controls.Add(mInkPicture);
            mInkPicture.Dock = DockStyle.Fill;
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
                    if (mMovingBmpData != null)
                    {
                        mMovingBmpData.region.Translate(e.Location.X - (mMovingBmpData.location.X + mMovingBmpData.bmp.Width / 2), e.Location.Y - (mMovingBmpData.location.Y + mMovingBmpData.bmp.Height / 2));
                        mMovingBmpData.location.X = e.Location.X - mMovingBmpData.bmp.Width / 2;
                        mMovingBmpData.location.Y = e.Location.Y - mMovingBmpData.bmp.Height / 2;
                        DrawBmpToBg(mMovingBmpData, mBmpBG);
                        mBgBitmaps.Add(mMovingBmpData);
                        mMovingBmpData = null;
                    }
                }
                else
                {
                    if (mInkPicture.InkEnabled == false)
                    {
                        foreach (BitmapPosData bmpData in mBgBitmaps)
                        {
                            if (bmpData.region.IsVisible(e.Location))
                            {
                                mMovingBmpData = bmpData;
                                mIsTempPictMoving = true;
                                mBgBitmaps.Remove(bmpData);
                                break;
                            }
                        }

                        //rebuild background image
                        DrawBackGroundImageAndGrid();
                        //take snapshot of background except moving image
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
                    DrawBmpToBg(mMovingBmpData, mBmpBG);
                    this.mInkPicture.BackgroundImage = mBmpBG;
                }
            }
        }

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
                mMovingBmpData = new BitmapPosData();
                mMovingBmpData.bmp = bmpClip;
                mMovingBmpData.location = new Point(0, 0);
                mMovingBmpData.region = rgnTempPict.Clone();
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
            FormSelRegion frm = new FormSelRegion();
            DialogResult res = frm.ShowDialog();
            Console.WriteLine("FormSelRegion.DialogResult is %s", res.ToString());
            if (res == System.Windows.Forms.DialogResult.OK)
            {
            Console.WriteLine("FormSelRegion.DialogResult==OK");
                Region rg = frm.SelRegion;
                CopyDesktopImageInClippedRegion(rg);
                
                //take snapshot of background except moving image
                if (mBmpTempBG != null)
                {
                    mBmpTempBG.Dispose();
                    mBmpTempBG = null;
                }
                mBmpTempBG = new Bitmap(mBmpBG);

                foreach (Form c in this.MdiParent.MdiChildren)
                {
                    c.Visible = true;
                }
                mIsTempPictMoving = true;
                this.Cursor = System.Windows.Forms.Cursors.Hand;
                this.mInkPicture.Enabled = false;
                this.mInkPicture.InkEnabled = false;
            }
            this.Visible = true;
        }

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
            }
        }

        private void FormNote_SizeChanged(object sender, EventArgs e)
        {
            DrawBackGroundImageAndGrid();
        }

        public void DrawBackGroundImageAndGrid()
        {
            if(mBmpBG != null)
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
                DeleteSelectedImage();
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
            mFormPalette.activeNote = this;
            //mFormPalette.MdiParent.WindowState = FormWindowState.Maximized;
            mFormPalette.MdiParent.Visible = true;
            mFormPalette.SetVisiblePaletteAndNotes();

            Size sizePrev = this.Size;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Size sizeAfter = this.Size;
            int dx = (sizeAfter.Width - sizePrev.Width) / 2;
            int dy = sizeAfter.Height - sizePrev.Height - dx;
            Point pt = this.Location;
            pt.Offset(-dx, -dy);
            this.Location = pt;
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

        public void DeleteSelectedImage()
        {
            if (mIsTempPictMoving)
            {
                mIsTempPictMoving = false;
                //dispose selected image
                if (mMovingBmpData != null)
                {
                    mMovingBmpData.Dispose();
                    mMovingBmpData = null;
                }

                //rebuild background image
                DrawBackGroundImageAndGrid();
            }
        }
    }
}
