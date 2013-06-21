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
        public float mWidth;
        public float mHeight;
        public PenTip mPenTip;

        //[BrowsableAttribute(false)]
        protected override bool ShowWithoutActivation { get{ return true;} }

        public Palette()
        {
            InitializeComponent();
        }

        private void Palette_Load(object sender, EventArgs e)
        {
            mNotes = new List<FormNote>();
            string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InkNote";
            string[] pathes = System.IO.Directory.GetFiles(dirPath, "*.ikn");
            foreach (string path in pathes)
            {
                FormNote newNote = new FormNote(this, path);
                newNote.MdiParent = this.MdiParent;
                newNote.Show();
                mActiveNote = newNote;
                mNotes.Add(newNote);
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
            UpdateDrawingAttributesAndModeOfNotes();
            OnInkDrawMode();
            ActivatePaintingScreen();
        }

        private void panelColorYellow_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Yellow;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            ActivatePaintingScreen();
        }

        private void panelColorGreen_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Lime;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            ActivatePaintingScreen();
        }

        private void panelColorWhite_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.White;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
            ActivatePaintingScreen();
        }

        private void panelColorBlack_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mColor = Color.Black;
            mMode = MODE.INK_DRAW;
            OnInkDrawMode();
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
            if (mActiveNote == null) return;
            mActiveNote.Save();
            mActiveNote.restoreInkMode(mMode);
            ActivatePaintingScreen();
        }

        private void pictCopy_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            if (mActiveNote.IsImagePicked)
            {
                mActiveNote.CopyPickedImage();
            }
            else
            {
                mActiveNote.CopyToClipboard();
                mActiveNote.restoreInkMode(mMode);
                ActivatePaintingScreen();
            }
        }

        private void pictSelect_Click(object sender, EventArgs e)
        {
            if (mActiveNote == null) return;
            mActiveNote.SelPict();
        }

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
                if (mMode == MODE.IMG_PICK)
                {
                    mActiveNote.DeletePickedImage();
                }
                mActiveNote.restoreInkMode(mMode);
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
            mActiveNote.restoreInkMode(mMode);
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
            pictDelete.Visible = false;
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
            pictSelInk.Visible = true;
            pictUndo.Visible = true;
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
            pictCleaner.Visible = true;
            pictCopy.Visible = true;
            pictDelete.Visible = false;
            pictErase.Visible = false;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = true;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            pictSelInk.Visible = true;
            pictUndo.Visible = true;
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
            pictCleaner.Visible = false;
            pictCopy.Visible = true;
            pictDelete.Visible = false;
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
            pictSelInk.Visible = true;
            pictUndo.Visible = true;
        }

        public void OnInkSelectMode()
        {
            pictCleaner.Visible = false;
            pictColor.Visible = false;
            pictCopy.Visible = true;
            pictDelete.Visible = false;
            pictErase.Visible = false;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = true;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            pictSelInk.Visible = false;
            pictUndo.Visible = false;
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
            pictCleaner.Visible = false;
            pictCopy.Visible = false;
            pictDelete.Visible = true;
            pictErase.Visible = false;
            pictGrid.Visible = false;
            pictNew.Visible = false;
            pictPenBallL.Visible = false;
            pictPenBallS.Visible = false;
            pictPenRectH.Visible = false;
            pictPenRectV.Visible = false;
            pictPick.Visible = false;
            pictSave.Visible = false;
            pictSelect.Visible = false;
            pictSelInk.Visible = false;
            pictUndo.Visible = false;
        }

        public void OnImagePickMode()
        {
            panelColorBlack.Visible = true;
            panelColorBlue.Visible = true;
            panelColorGreen.Visible = true;
            panelColorRed.Visible = true;
            panelColorWhite.Visible = true;
            panelColorYellow.Visible = true;
            pictColor.Visible = false;
            pictCleaner.Visible = true;
            pictCopy.Visible = true;
            pictDelete.Visible = true;
            pictErase.Visible = true;
            pictGrid.Visible = true;
            pictNew.Visible = true;
            pictPenBallL.Visible = true;
            pictPenBallS.Visible = true;
            pictPenRectH.Visible = true;
            pictPenRectV.Visible = true;
            pictPick.Visible = false;
            pictSave.Visible = true;
            pictSelect.Visible = true;
            pictSelInk.Visible = true;
            pictUndo.Visible = false;
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
            pictCleaner.Visible = false;
            pictCopy.Visible = true;
            pictDelete.Visible = true;
            pictErase.Visible = false;
            pictGrid.Visible = false;
            pictNew.Visible = false;
            pictPenBallL.Visible = false;
            pictPenBallS.Visible = false;
            pictPenRectH.Visible = false;
            pictPenRectV.Visible = false;
            pictPick.Visible = false;
            pictSave.Visible = false;
            pictSelect.Visible = false;
            pictSelInk.Visible = false;
            pictUndo.Visible = false;
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
            //mActiveNote.mInkPicture.Enabled = true;
            //mActiveNote.mInkPicture.InkEnabled = true;
            //mActiveNote.mInkPicture.EditingMode = InkOverlayEditingMode.Select;
            OnInkSelectMode();
        }

    }
}
