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
        public FormNote activeNote = null;
        List<FormNote> notes = null;
        public bool isClosing = false;

        //[BrowsableAttribute(false)]
        protected override bool ShowWithoutActivation { get{ return true;} }

        public Palette()
        {
            InitializeComponent();
        }

        private void Palette_Load(object sender, EventArgs e)
        {
            notes = new List<FormNote>();
            string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InkNote";
            string[] pathes = System.IO.Directory.GetFiles(dirPath, "*.ikn");
            foreach (string path in pathes)
            {
                FormNote newNote = new FormNote(this, path);
                newNote.MdiParent = this.MdiParent;
                newNote.Show();
                activeNote = newNote;
                notes.Add(newNote);
            }
            if (activeNote == null)
            {
                FormNote newNote = new FormNote(this);
                newNote.MdiParent = this.MdiParent;
                newNote.Show();
                activeNote = newNote;
                notes.Add(newNote);
            }

        }

        public void SetVisiblePaletteAndNotes()
        {
            foreach (FormNote note in notes)
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

        private void panelColorRed_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Color = Color.Red;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorBlue_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Color = Color.Blue;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorYellow_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Color = Color.Yellow;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorGreen_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Color = Color.Lime;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorWhite_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Color = Color.White;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void panelColorBlack_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Color = Color.Black;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void Palette_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Palette_FormClosing");
            foreach (FormNote note in notes)
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
            if (activeNote == null) return;
            activeNote.Save();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictCopy_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.CopyToClipboard();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictSelect_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.SelPict();
        }

        public void deleteNote(FormNote note)
        {
            notes.Remove(note);
            if (activeNote == note)
            {
                activeNote = null;
            }
        }
        private void pictDelete_Click(object sender, EventArgs e)
        {
            if (activeNote != null)
            {
                if (activeNote.mInkPicture.InkEnabled == false)
                {
                    activeNote.DeleteSelectedImage();
                }
            }
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void ActivatePaintingScreen()
        {
            if (activeNote == null) return;
            activeNote.Activate();
        }

        private void pictPenBallS_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Width = 100;
            activeNote.mInkPicture.DefaultDrawingAttributes.Height = 100;
            activeNote.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Ball;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictPenBallL_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Width = 200;
            activeNote.mInkPicture.DefaultDrawingAttributes.Height = 200;
            activeNote.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Ball;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictPenRectH_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Width = 200;
            activeNote.mInkPicture.DefaultDrawingAttributes.Height = 100;
            activeNote.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Rectangle;
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictPenRectV_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.DefaultDrawingAttributes.Width = 100;
            activeNote.mInkPicture.DefaultDrawingAttributes.Height = 200;
            activeNote.mInkPicture.DefaultDrawingAttributes.PenTip = PenTip.Rectangle;
            restoreInkMode();
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
            dlg.Color = activeNote.mBgColor;

            // Update the text box color if the user clicks OK  
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                activeNote.mBgColor = dlg.Color;
                activeNote.DrawBackGroundImageAndGrid();
            }
        }

        private void pictUndo_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.Undo();
            restoreInkMode();
            ActivatePaintingScreen();
        }

        private void pictErase_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.EditingMode = InkOverlayEditingMode.Delete;
            activeNote.mInkPicture.EraserMode = InkOverlayEraserMode.PointErase;
            ActivatePaintingScreen();
        }

        private void restoreInkMode()
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.InkEnabled = true;
            activeNote.mInkPicture.Enabled = true;
            pictPick.Enabled = true;
            activeNote.mInkPicture.EditingMode = InkOverlayEditingMode.Ink;
        }

        private void pictPick_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.InkEnabled = false;
            activeNote.mInkPicture.Enabled = false;
            ((PictureBox)sender).Enabled = false;
        }

        private void pictCleaner_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;
            activeNote.mInkPicture.EditingMode = InkOverlayEditingMode.Delete;
            activeNote.mInkPicture.EraserMode = InkOverlayEraserMode.StrokeErase;
        }

        private void pictNew_Click(object sender, EventArgs e)
        {
            FormNote newNote = new FormNote(this);
            newNote.MdiParent = this.MdiParent;
            newNote.Show();
            activeNote = newNote;
            notes.Add(newNote);
        }

        private void pictColor_Click(object sender, EventArgs e)
        {
            if (activeNote == null) return;

            ColorDialog dlg = new ColorDialog();
            // Keeps the user from selecting a custom color.
            dlg.AllowFullOpen = false;
            // Allows the user to get help. (The default is false.)
            dlg.ShowHelp = true;
            // Sets the initial color select to the current text color.
            dlg.Color = activeNote.mBgColor;

            // Update the text box color if the user clicks OK  
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                activeNote.mInkPicture.DefaultDrawingAttributes.Color = dlg.Color;
                restoreInkMode();
                ActivatePaintingScreen();
            }
        }

    }
}
