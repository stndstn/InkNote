using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Ink;
using System.Configuration;

namespace InkNote
{
    public partial class Palette : Form
    {
        static public float DEFAULT_PEN_WIDTH;
        static public float DEFAULT_PEN_HEIGHT;
        public FormNote mActiveNote = null;
        List<FormNote> mNotes = null;
        public bool isClosing = false;

        public enum MODE
        {
            INK_DRAW,
            INK_SEL,
            INK_POINT_ERASE,
            INK_STROKE_ERASE,
            IMG_PICK
        }
        public MODE mMode = MODE.INK_DRAW;

        public Color mColor;
        public byte mTransparency;
        public float mWidth;
        public float mHeight;
        public PenTip mPenTip;

        public bool IsImageCopied
        {
            get { return (mCopiedBmpData==null ? false : true); }
        }

        public BitmapPosData mCopiedBmpData = null;

        //[BrowsableAttribute(false)]
        protected override bool ShowWithoutActivation { get{ return true;} }

        public Palette()
        {
            InitializeComponent();
        }

        private void Palette_Load(object sender, EventArgs e)
        {
            mNotes = new List<FormNote>();
            string dirPath = string.Empty;
            if (Program.s_appDataDir.Length > 0)
                dirPath = Program.s_appDataDir;
            else
                dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InkNote";
            try
            {
                string[] pathes = System.IO.Directory.GetFiles(dirPath, "*.ikn");
                foreach (string path in pathes)
                {
                    FormNote newNote = new FormNote(this, path);
                    newNote.MdiParent = this.MdiParent;
                    newNote.Show();
                    mActiveNote = newNote;
                    mNotes.Add(newNote);
                }
            }
            catch (Exception ex)
            {
            }

            if (mActiveNote == null)
            {
                FormNote newNote = new FormNote(this);
                newNote.MdiParent = this.MdiParent;
                newNote.Show();
                mActiveNote = newNote;
                mNotes.Add(newNote);
            }
            mColor = mActiveNote.mInkPicture.DefaultDrawingAttributes.Color;
            mWidth = mActiveNote.mInkPicture.DefaultDrawingAttributes.Width;
            mHeight = mActiveNote.mInkPicture.DefaultDrawingAttributes.Height;
            mPenTip = mActiveNote.mInkPicture.DefaultDrawingAttributes.PenTip;
            mTransparency = mActiveNote.mInkPicture.DefaultDrawingAttributes.Transparency;
            mMode = MODE.INK_DRAW;
            mActiveNote.setInkMode(mMode);
            OnInkDrawMode();
        }

        public void SetVisiblePaletteAndNotes()
        {
            foreach (FormNote note in mNotes)
            {
                note.Visible = true;
            }

            this.Visible = true;
        }

        private void Palette_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("Palette_FormClosed");
        }

        private void Palette_VisibleChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_VisibleChanged");
        }

        private void Palette_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_Activated");
        }

        private void Palette_Deactivate(object sender, EventArgs e)
        {
            Console.WriteLine("Palette_Deactivate");
        }

        private void UpdateDrawingAttributesAndModeOfNotes()
        {
            foreach (FormNote note in mNotes)
            {
                note.mInkPicture.DefaultDrawingAttributes.Color = mColor;
                note.mInkPicture.DefaultDrawingAttributes.Height = mHeight;
                note.mInkPicture.DefaultDrawingAttributes.Width = mWidth;
                note.mInkPicture.DefaultDrawingAttributes.PenTip = mPenTip;
                note.mInkPicture.DefaultDrawingAttributes.Transparency = mTransparency;
                note.setInkMode(mMode);
            }
        }
        private void panelColorRed_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Red;
            mMode = MODE.INK_DRAW;
            UpdateDrawingAttributesAndModeOfNotes();
            OnInkDrawMode();
            ActivatePaintingScreen();
        }

        private void panelColorBlue_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Blue;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void panelColorYellow_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Yellow;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void panelColorGreen_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Lime;
            mActiveNote.mInkPicture.DefaultDrawingAttributes.Transparency = (byte)127;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void panelColorWhite_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.White;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void panelColorBlack_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Black;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void Palette_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Palette_FormClosing");
            foreach (FormNote note in mNotes)
            {
                note.Save();
            }
            if (isClosing == false)
            {
                e.Cancel = true;
                this.MdiParent.WindowState = FormWindowState.Minimized;
            }
        }

        private void pictSave_Click(object sender, EventArgs e)
        {
            string dirPath = string.Empty;
            if (Program.s_appDataDir.Length > 0)
                dirPath = Program.s_appDataDir;
            else
                dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InkNote";

            System.Windows.Forms.FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Please select a folder to save the all note data.";
            dlg.SelectedPath = dirPath;
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = dlg.SelectedPath;
                if (System.IO.Directory.Exists(folderName))
                {
                    Program.s_appDataDir = folderName;
                    //ConfigurationManager.AppSettings..Set("DataDir", folderName);

                    // Get the configuration  
                    // that applies to the all user.
                    Configuration appConfig =
                      ConfigurationManager.OpenExeConfiguration(
                       ConfigurationUserLevel.None);

                    // Map the roaming configuration file. This 
                    // enables the application to access  
                    // the configuration file using the 
                    // System.Configuration.Configuration class
                    ExeConfigurationFileMap configFileMap =
                      new ExeConfigurationFileMap();
                    configFileMap.ExeConfigFilename =
                      appConfig.FilePath;

                    // Get the mapped configuration file.
                    Configuration config =
                      ConfigurationManager.OpenMappedExeConfiguration(
                        configFileMap, ConfigurationUserLevel.None);

                    config.AppSettings.Settings.Add("DataDir", folderName);

                    try
                    {
                        // Synchronize the application configuration 
                        // if needed. The following two steps seem 
                        // to solve some out of synch issues  
                        // between roaming and default 
                        // configuration.
                        config.Save(ConfigurationSaveMode.Modified);

                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        Console.WriteLine("[Exception error: {0}]",
                            ex.ToString());
                    }

                    foreach (Form c in this.MdiParent.MdiChildren)
                    {
                        if (c is FormNote)
                        {
                            FormNote note = c as FormNote;
                            note.Save(folderName);
                        }
                    }
                }
            }

            /*
            if (mActiveNote == null) return;
            mActiveNote.Save();
             */
            mActiveNote.setInkMode(mMode);
            ActivatePaintingScreen();
        }

        private void pictCopy_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            if (mActiveNote.IsImagePicked)
            {
                //DrawBmpToBg(mPickedBmpDataBackup, mBmpBG);
                if (mCopiedBmpData != null)
                {
                    mCopiedBmpData.Dispose();
                    mCopiedBmpData = null;
                }
                mCopiedBmpData = new BitmapPosData(mActiveNote.mPickedBmpDataRef);
                pictPaste.Visible = true;
            }
            else
            {
                mActiveNote.CopyToClipboard();
                mActiveNote.setInkMode(mMode);
                ActivatePaintingScreen();
            }
        }

        private void pictSelect_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            SelPict();
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
                    if (mCopiedBmpData != null)
                    {
                        mCopiedBmpData.Dispose();
                        mCopiedBmpData = null;
                    }
                    System.Threading.Thread.Sleep(500);
                    mCopiedBmpData = BitmapPosData.CreateFromDesktopImageInClippedRegion(frm.SelRegion, frm.RegionPath);
                    mActiveNote.CopyPickedImage(mCopiedBmpData);
                    mMode = MODE.IMG_PICK;
                    mActiveNote.setInkMode(mMode);
                }
                foreach (Form c in this.MdiParent.MdiChildren)
                {
                    c.Visible = true;
                    if (c.GetType() == typeof(FormNote))
                    {
                        FormNote note = (FormNote)c;
                        note.Location = note.mPreviousLocation;
                    }
                }
                this.Cursor = System.Windows.Forms.Cursors.Hand;
                mActiveNote.Activate();
                this.Visible = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
#if false
        public void CopyDesktopImageInClippedRegion(Region rg, Point[] ptPath)
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
                for (int i = 0; i < ptPath.Length; i++)
                {
                    ptPath[i].Offset((int)-rc.X, (int)-rc.Y);
                }
                mCopiedBmpData = new FormNote.BitmapPosData(bmpClip, rgnTempPict.Clone(), new Point(0, 0), ptPath);
                /*
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
                */
                rgnTempPict.Dispose();

                gd.Dispose();
                gt.Dispose();

                DeleteObject(hBmp);
                GC.Collect();
            }
        }
#endif
        public void deleteNote(FormNote note)
        {
            mNotes.Remove(note);
            if (mActiveNote == note)
            {
                mActiveNote = null;
            }
        }
        private void pictDelete_Click(object sender, EventArgs e)
        {
            if (mActiveNote != null)
            {
                SendKeys.Send("{DELETE}");
                if (mMode == MODE.IMG_PICK)
                {
                    //mActiveNote.DeletePickedImage();
                }
                else if (mMode == MODE.INK_SEL)
                {
                    //mActiveNote.DeleteSelectedStrokes();
                }
                //mActiveNote.setInkMode(mMode);
            }
            ActivatePaintingScreen();
        }

        private void ActivatePaintingScreen()
        {
            if (mActiveNote == null) return;
            mActiveNote.Activate();
        }

        private void pictPenBallS_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            OnInkDrawMode();
            mWidth = DEFAULT_PEN_WIDTH;
            mHeight = DEFAULT_PEN_HEIGHT;
            mPenTip = PenTip.Ball;
            mMode = MODE.INK_DRAW;
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void pictPenBallL_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            OnInkDrawMode();
            mWidth = 200;
            mHeight = 200;
            mPenTip = PenTip.Ball;
            mMode = MODE.INK_DRAW;
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void pictPenRectH_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            OnInkDrawMode();
            mWidth = 200;
            mHeight = 100;
            mPenTip = PenTip.Rectangle;
            mMode = MODE.INK_DRAW;
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void pictPenRectV_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            OnInkDrawMode();
            mWidth = 100;
            mHeight = 200;
            mPenTip = PenTip.Rectangle;
            mMode = MODE.INK_DRAW;
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void pictGrid_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            // Keeps the user from selecting a custom color.
            dlg.AllowFullOpen = false;
            // Allows the user to get help. (The default is false.)
            dlg.ShowHelp = true;
            // Sets the initial color select to the current text color.
            dlg.Color = mActiveNote.mBgColor;

            // Update the text box color if the user clicks OK  
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                mActiveNote.mBgColor = dlg.Color;
                mActiveNote.DrawBackGroundImageAndGrid();
            }
        }

        private void pictUndo_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mActiveNote.Undo();
            mActiveNote.setInkMode(mMode);
            UpdateButtonVisibleByCondition();
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void pictErase_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            OnInkEraseStrokeMode();
            mActiveNote.setInkMode(MODE.INK_POINT_ERASE);
            ActivatePaintingScreen();
        }

        public void OnInkDrawMode()
        {
            panelColorBlack.Visible = true;
            panelColorBlue.Visible = true;
            panelColorGreen.Visible = true;
            panelColorRed.Visible = true;
            panelColorWhite.Visible = true;
            panelColorYellow.Visible = true;
            pictColor.Visible = true;
            pictCleaner.Visible = true;
            pictCopy.Visible = true;
            pictErase.Visible = true;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = true;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            UpdateButtonVisibleByCondition();
        }

        public void OnInkEraseStrokeMode()
        {
            panelColorBlack.Visible = true;
            panelColorBlue.Visible = true;
            panelColorGreen.Visible = true;
            panelColorRed.Visible = true;
            panelColorWhite.Visible = true;
            panelColorYellow.Visible = true;
            pictColor.Visible = true;
            pictCopy.Visible = true;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = true;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            UpdateButtonVisibleByCondition();
        }

        public void OnInkErasePointMode()
        {
            panelColorBlack.Visible = true;
            panelColorBlue.Visible = true;
            panelColorGreen.Visible = true;
            panelColorRed.Visible = true;
            panelColorWhite.Visible = true;
            panelColorYellow.Visible = true;
            pictColor.Visible = true;
            pictCopy.Visible = true;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = true;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            UpdateButtonVisibleByCondition();
        }

        public void OnInkSelectMode()
        {
            panelColorBlack.Visible = true;
            panelColorBlue.Visible = true;
            panelColorGreen.Visible = true;
            panelColorRed.Visible = true;
            panelColorWhite.Visible = true;
            panelColorYellow.Visible = true;
            pictColor.Visible = true;
            pictCopy.Visible = true;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = true;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            UpdateButtonVisibleByCondition();
        }

        public void OnInkSelected()
        {
            panelColorBlack.Visible = false;
            panelColorBlue.Visible = false;
            panelColorGreen.Visible = false;
            panelColorRed.Visible = false;
            panelColorWhite.Visible = false;
            panelColorYellow.Visible = false;
            pictColor.Visible = false;
            pictCopy.Visible = false;
            pictGrid.Visible = false;
            pictNew.Visible = false;
            pictPenBallL.Visible = false;
            pictPenBallS.Visible = false;
            pictPenRectH.Visible = false;
            pictPenRectV.Visible = false;
            pictPick.Visible = false;
            pictSave.Visible = false;
            pictSelect.Visible = false;
            UpdateButtonVisibleByCondition();
        }

        public void OnImagePickMode()
        {
            panelColorBlack.Visible = true;
            panelColorBlue.Visible = true;
            panelColorGreen.Visible = true;
            panelColorRed.Visible = true;
            panelColorWhite.Visible = true;
            panelColorYellow.Visible = true;
            pictColor.Visible = true;
            pictCopy.Visible = true;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = false;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            UpdateButtonVisibleByCondition();
        }
        
        public void OnImagePicked()
        {
            panelColorBlack.Visible = false;
            panelColorBlue.Visible = false;
            panelColorGreen.Visible = false;
            panelColorRed.Visible = false;
            panelColorWhite.Visible = false;
            panelColorYellow.Visible = false;
            pictColor.Visible = false;
            pictCopy.Visible = true;
            pictGrid.Visible = false;
            pictNew.Visible = false;
            pictPenBallL.Visible = false;
            pictPenBallS.Visible = false;
            pictPenRectH.Visible = false;
            pictPenRectV.Visible = false;
            pictPick.Visible = false;
            pictSave.Visible = false;
            pictSelect.Visible = false;
            UpdateButtonVisibleByCondition();
        }

        private void UpdateButtonVisibleByCondition()
        {
            pictPaste.Visible = (mCopiedBmpData == null) ? false : true;

            if (mActiveNote != null
                && (mActiveNote.mPickedBmpDataRef != null
                    || mActiveNote.mInkPicture.Selection.Count > 0)
                )
            {
                pictDelete.Visible = true;
            }
            else
            {
                pictDelete.Visible = false;
            }

            if (mActiveNote != null
                && mActiveNote.mInkPicture.InkEnabled
                && mActiveNote.mInkPicture.Ink.Strokes.Count > 0)
            {
                pictUndo.Visible = true;
                pictSelInk.Visible = (mActiveNote.mInkPicture.EditingMode != InkOverlayEditingMode.Select);
                pictCleaner.Visible = !(mActiveNote.mInkPicture.EditingMode == InkOverlayEditingMode.Delete
                                    && mActiveNote.mInkPicture.EraserMode == InkOverlayEraserMode.StrokeErase);
                pictErase.Visible = !(mActiveNote.mInkPicture.EditingMode == InkOverlayEditingMode.Delete
                                    && mActiveNote.mInkPicture.EraserMode == InkOverlayEraserMode.PointErase);
            }
            else
            {
                pictUndo.Visible = false;
                pictSelInk.Visible = false;
                pictCleaner.Visible = false;
                pictErase.Visible = false;
            }
        }

        private void pictPick_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            OnImagePickMode();
            mMode = MODE.IMG_PICK;
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
            //mActiveNote.mInkPicture.InkEnabled = false;
            //mActiveNote.mInkPicture.Enabled = false;
        }

        private void pictCleaner_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            OnInkEraseStrokeMode();
            mMode = MODE.INK_STROKE_ERASE;
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
        }

        private void pictNew_Click(object sender, EventArgs e)
        {
            FormNote newNote = new FormNote(this);
            newNote.MdiParent = this.MdiParent;
            newNote.Show();
            mActiveNote = newNote;
            newNote.mInkPicture.DefaultDrawingAttributes.Color = mColor;
            newNote.mInkPicture.DefaultDrawingAttributes.Height = mHeight;
            newNote.mInkPicture.DefaultDrawingAttributes.Width = mWidth;
            newNote.mInkPicture.DefaultDrawingAttributes.PenTip = mPenTip;
            newNote.setInkMode(mMode);
            mNotes.Add(newNote);
        }

        private void pictColor_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;

            ColorDialog dlg = new ColorDialog();
            // Keeps the user from selecting a custom color.
            dlg.AllowFullOpen = false;
            // Allows the user to get help. (The default is false.)
            dlg.ShowHelp = true;
            // Sets the initial color select to the current text color.
            dlg.Color = mActiveNote.mBgColor;

            // Update the text box color if the user clicks OK  
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                mColor = dlg.Color;
                mMode = MODE.INK_DRAW;
                UpdateDrawingAttributesAndModeOfNotes();
                ActivatePaintingScreen();
                OnInkDrawMode();
            }
        }

        private void pictSelInk_MouseClick(object sender, MouseEventArgs e)
        {
            if (mActiveNote == null) return;
            mMode = MODE.INK_SEL;
            UpdateDrawingAttributesAndModeOfNotes();
            ActivatePaintingScreen();
            OnInkSelectMode();
        }

        private void pictPaste_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            if (IsImageCopied)
            {
                mActiveNote.CopyPickedImage(mCopiedBmpData);
                mMode = MODE.IMG_PICK;
                mActiveNote.setInkMode(mMode);
            }
        }

        private void pictTransparent_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;

            FormTransparent form = new FormTransparent(mTransparency);
            form.mShowLocation = this.Location;
            form.ShowDialog();
            mTransparency = (byte)form.Value;
            mMode = MODE.INK_DRAW;
            UpdateDrawingAttributesAndModeOfNotes();
            OnInkDrawMode();
            ActivatePaintingScreen();
        }

    }
}
