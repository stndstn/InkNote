using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InkNote
{
    public partial class MdiContainer : Form
    {
        NotifyIcon mNotifyIcon = null;
        Palette mFormPalette = null;

        public MdiContainer()
        {
            InitializeComponent();
        }

        private void MdiContainer_Load(object sender, EventArgs e)
        {
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

            this.TransparencyKey = Color.FromArgb(255, 220, 33, 55);
            MdiClient Client = new MdiClient();
            Client.Click += new EventHandler(Client_Click);
            this.Controls.Add(Client);
            Client.BackColor = Color.FromArgb(255, 220, 33, 55);

            mFormPalette = new Palette();
            mFormPalette.MdiParent = this;
            mFormPalette.Show();
        }

        void Client_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Maximized;
                //mFormPalette.ActivatePaletteAndNotes();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mFormPalette.isClosing = true;
            this.mFormPalette.Close();
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void MdiContainer_Activated(object sender, EventArgs e)
        {
//            mFormPalette.ActivatePaletteAndNotes();
        }
    }
}
