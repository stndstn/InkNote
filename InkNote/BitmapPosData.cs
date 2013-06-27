using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace InkNote
{
    public class BitmapPosData : IDisposable
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


        public Region region;
        public Point location;
        public Point ptGripDelta;
        public Bitmap bitmap;
        public bool isPicked;
        public Point[] ptPolygon;
        public Point[] Points
        {
            get
            {
                Point[] ptCurPath = (Point[])ptPolygon.Clone();
                for (int i = 0; i < ptCurPath.Length; i++)
                {
                    ptCurPath[i].Offset(location);
                }
                return ptCurPath;
            }
        }
        public BitmapPosData(Bitmap bmp, Region reg, Point loc, Point[] pts)
        {
            region = reg;
            bitmap = bmp;
            location = loc;
            ptGripDelta = Point.Empty;
            isPicked = false;
            ptPolygon = pts;
        }
        public BitmapPosData(BitmapPosData value)
        {
            region = new Region(value.region.GetRegionData());
            bitmap = new Bitmap(value.bitmap);
            location = value.location;
            ptGripDelta = value.ptGripDelta;
            if (value.ptPolygon != null)
            {
                ptPolygon = (Point[])value.ptPolygon.Clone();
            }
            isPicked = false;
        }
        static public BitmapPosData CreateFromDesktopImageInClippedRegion(Region rg, Point[] ptPath)
        {
            Console.WriteLine("CopyDesktopImageToClippedRegion");

            BitmapPosData data = null;
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
                for (int i = 0; i < ptPath.Length; i++)
                {
                    ptPath[i].Offset((int)-rc.X, (int)-rc.Y);
                }
                data = new BitmapPosData(bmpClip, rgnTempPict.Clone(), new Point(0, 0), ptPath);
                rgnTempPict.Dispose();

                gd.Dispose();
                gt.Dispose();

                DeleteObject(hBmp);
                GC.Collect();
            }
            return data;
        }
        public void Dispose()
        {
            if (bitmap != null) bitmap.Dispose();
            if (region != null) region.Dispose();
        }
    }
}
