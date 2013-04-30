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
        //public bool isClosing = false;
        public FormNote activeNote = null;
        List<FormNote> notes = null;
        NotifyIcon mNotifyIcon = null;
        bool isClosing = false;

        public Palette()
        {
            InitializeComponent();
        }

        private void Palette_Load(object sender, EventArgs e)
        {
            notes = new List<FormNote>();
            //string curDir = System.IO.Directory.GetCurrentDirectory();
            string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InkNote";
            string[] pathes = System.IO.Directory.GetFiles(dirPath, "*.ikn");
            foreach (string path in pathes)
            {
                FormNote newNote = new FormNote(this, path);
                newNote.Show();
                activeNote = newNote;
                notes.Add(newNote);
            }
            if (activeNote == null)
            {
                FormNote newNote = new FormNote(this);
                newNote.Show();
                activeNote = newNote;
                notes.Add(newNote);
            }

            // Create the NotifyIcon.
            this.mNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            // The Icon property sets the icon that will appear
            // in the systray for this application.
            mNotifyIcon.Icon = global::InkNote.Properties.Resources.Icon1;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            mNotifyIcon.ContextMenuStrip = this.contextMenuStrip1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            mNotifyIcon.Text = "InkNote";
            mNotifyIcon.Visible = true;

            mNotifyIcon.MouseClick += new MouseEventHandler(NotifyIcon_MouseClick);

        }

        void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ActivatePaletteAndNotes();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClosing = true;
            this.Close();
        }

        private void ActivatePaletteAndNotes()
        {
            foreach (FormNote note in notes)
            {
                note.Visible = true;
            }

            this.Visible = true;
            this.Activate();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActivatePaletteAndNotes();
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
                note.Visible = false;
            }
            if (isClosing == false)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void pictSave_Click(object sender, EventArgs e)
        {
            //FormNote fm = (FormNote)this.Owner;
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
            /*
            Rectangle rc = fm.mRcSelected; // backup because it will be emptied after CopyDesktopImage() was called.
            if (rc != Rectangle.Empty)
            {
                if (fm.mUseDesktopImgAsBG)
                {
                    fm.CopyDesktopImage();
                    fm.mRcSelected = rc;
                }
            }
            SelDesktopRectScreen s = new SelDesktopRectScreen();
            s.ShowDialog();
            fm.mRcSelected = s.SelectedRect;
            fm.DrawSelRect();
             */
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
            //fm.Clear();
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

        private void Palette_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C))
            {
                //fm.CopyToClipboard();
            }
            else if (e.Control && (e.KeyCode == Keys.S))
            {
                if (activeNote == null) return;
                activeNote.Save();
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
            //activeNote.mInkPicture.EditingMode = InkOverlayEditingMode.Select;
            //activeNote.TurnAroundPenSelMode((PictureBox)sender);
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
            newNote.Show();
            activeNote = newNote;
            notes.Add(newNote);
            ActivatePaintingScreen();            
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
