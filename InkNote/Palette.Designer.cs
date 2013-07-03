namespace InkNote
{
    partial class Palette
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (mCopiedBmpData != null) mCopiedBmpData.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Palette));
            this.panelColorRed = new System.Windows.Forms.Panel();
            this.panelColorBlue = new System.Windows.Forms.Panel();
            this.panelColorYellow = new System.Windows.Forms.Panel();
            this.panelColorGreen = new System.Windows.Forms.Panel();
            this.panelColorWhite = new System.Windows.Forms.Panel();
            this.panelColorBlack = new System.Windows.Forms.Panel();
            this.pictSelect = new System.Windows.Forms.PictureBox();
            this.pictCopy = new System.Windows.Forms.PictureBox();
            this.pictSave = new System.Windows.Forms.PictureBox();
            this.pictPenBallS = new System.Windows.Forms.PictureBox();
            this.pictPenBallL = new System.Windows.Forms.PictureBox();
            this.pictPenRectH = new System.Windows.Forms.PictureBox();
            this.pictPenRectV = new System.Windows.Forms.PictureBox();
            this.pictDelete = new System.Windows.Forms.PictureBox();
            this.pictGrid = new System.Windows.Forms.PictureBox();
            this.pictUndo = new System.Windows.Forms.PictureBox();
            this.pictColor = new System.Windows.Forms.PictureBox();
            this.pictErase = new System.Windows.Forms.PictureBox();
            this.pictPick = new System.Windows.Forms.PictureBox();
            this.pictCleaner = new System.Windows.Forms.PictureBox();
            this.pictNew = new System.Windows.Forms.PictureBox();
            this.pictSelInk = new System.Windows.Forms.PictureBox();
            this.pictPaste = new System.Windows.Forms.PictureBox();
            this.pictTransparent = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenBallS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenBallL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenRectH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenRectV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictUndo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictErase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCleaner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictNew)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictSelInk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPaste)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictTransparent)).BeginInit();
            this.SuspendLayout();
            // 
            // panelColorRed
            // 
            this.panelColorRed.BackColor = System.Drawing.Color.Red;
            this.panelColorRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorRed.Location = new System.Drawing.Point(2, 2);
            this.panelColorRed.Name = "panelColorRed";
            this.panelColorRed.Size = new System.Drawing.Size(17, 17);
            this.panelColorRed.TabIndex = 0;
            this.panelColorRed.Click += new System.EventHandler(this.panelColorRed_Click);
            // 
            // panelColorBlue
            // 
            this.panelColorBlue.BackColor = System.Drawing.Color.Blue;
            this.panelColorBlue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorBlue.Location = new System.Drawing.Point(25, 2);
            this.panelColorBlue.Name = "panelColorBlue";
            this.panelColorBlue.Size = new System.Drawing.Size(17, 17);
            this.panelColorBlue.TabIndex = 1;
            this.panelColorBlue.Click += new System.EventHandler(this.panelColorBlue_Click);
            // 
            // panelColorYellow
            // 
            this.panelColorYellow.BackColor = System.Drawing.Color.Yellow;
            this.panelColorYellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorYellow.Location = new System.Drawing.Point(48, 2);
            this.panelColorYellow.Name = "panelColorYellow";
            this.panelColorYellow.Size = new System.Drawing.Size(17, 17);
            this.panelColorYellow.TabIndex = 3;
            this.panelColorYellow.Click += new System.EventHandler(this.panelColorYellow_Click);
            // 
            // panelColorGreen
            // 
            this.panelColorGreen.BackColor = System.Drawing.Color.Lime;
            this.panelColorGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorGreen.Location = new System.Drawing.Point(71, 2);
            this.panelColorGreen.Name = "panelColorGreen";
            this.panelColorGreen.Size = new System.Drawing.Size(17, 17);
            this.panelColorGreen.TabIndex = 4;
            this.panelColorGreen.Click += new System.EventHandler(this.panelColorGreen_Click);
            // 
            // panelColorWhite
            // 
            this.panelColorWhite.BackColor = System.Drawing.Color.White;
            this.panelColorWhite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorWhite.Location = new System.Drawing.Point(94, 2);
            this.panelColorWhite.Name = "panelColorWhite";
            this.panelColorWhite.Size = new System.Drawing.Size(17, 17);
            this.panelColorWhite.TabIndex = 5;
            this.panelColorWhite.Click += new System.EventHandler(this.panelColorWhite_Click);
            // 
            // panelColorBlack
            // 
            this.panelColorBlack.BackColor = System.Drawing.Color.Black;
            this.panelColorBlack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColorBlack.Location = new System.Drawing.Point(117, 2);
            this.panelColorBlack.Name = "panelColorBlack";
            this.panelColorBlack.Size = new System.Drawing.Size(17, 17);
            this.panelColorBlack.TabIndex = 6;
            this.panelColorBlack.Click += new System.EventHandler(this.panelColorBlack_Click);
            // 
            // pictSelect
            // 
            this.pictSelect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictSelect.Image = global::InkNote.Properties.Resources.Image7;
            this.pictSelect.Location = new System.Drawing.Point(94, 24);
            this.pictSelect.Name = "pictSelect";
            this.pictSelect.Size = new System.Drawing.Size(18, 18);
            this.pictSelect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictSelect.TabIndex = 9;
            this.pictSelect.TabStop = false;
            this.pictSelect.Click += new System.EventHandler(this.pictSelect_Click);
            // 
            // pictCopy
            // 
            this.pictCopy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictCopy.Image = global::InkNote.Properties.Resources.Image6;
            this.pictCopy.Location = new System.Drawing.Point(48, 24);
            this.pictCopy.Name = "pictCopy";
            this.pictCopy.Size = new System.Drawing.Size(18, 18);
            this.pictCopy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictCopy.TabIndex = 8;
            this.pictCopy.TabStop = false;
            this.pictCopy.Click += new System.EventHandler(this.pictCopy_Click);
            // 
            // pictSave
            // 
            this.pictSave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictSave.Image = global::InkNote.Properties.Resources.Image5;
            this.pictSave.Location = new System.Drawing.Point(25, 24);
            this.pictSave.Name = "pictSave";
            this.pictSave.Size = new System.Drawing.Size(18, 18);
            this.pictSave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictSave.TabIndex = 7;
            this.pictSave.TabStop = false;
            this.pictSave.Click += new System.EventHandler(this.pictSave_Click);
            // 
            // pictPenBallS
            // 
            this.pictPenBallS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictPenBallS.Image = global::InkNote.Properties.Resources.Image1;
            this.pictPenBallS.Location = new System.Drawing.Point(2, 47);
            this.pictPenBallS.Name = "pictPenBallS";
            this.pictPenBallS.Size = new System.Drawing.Size(18, 18);
            this.pictPenBallS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictPenBallS.TabIndex = 10;
            this.pictPenBallS.TabStop = false;
            this.pictPenBallS.Click += new System.EventHandler(this.pictPenBallS_Click);
            // 
            // pictPenBallL
            // 
            this.pictPenBallL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictPenBallL.Image = global::InkNote.Properties.Resources.Image2;
            this.pictPenBallL.Location = new System.Drawing.Point(25, 47);
            this.pictPenBallL.Name = "pictPenBallL";
            this.pictPenBallL.Size = new System.Drawing.Size(18, 18);
            this.pictPenBallL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictPenBallL.TabIndex = 11;
            this.pictPenBallL.TabStop = false;
            this.pictPenBallL.Click += new System.EventHandler(this.pictPenBallL_Click);
            // 
            // pictPenRectH
            // 
            this.pictPenRectH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictPenRectH.Image = global::InkNote.Properties.Resources.Image3;
            this.pictPenRectH.Location = new System.Drawing.Point(48, 47);
            this.pictPenRectH.Name = "pictPenRectH";
            this.pictPenRectH.Size = new System.Drawing.Size(18, 18);
            this.pictPenRectH.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictPenRectH.TabIndex = 12;
            this.pictPenRectH.TabStop = false;
            this.pictPenRectH.Click += new System.EventHandler(this.pictPenRectH_Click);
            // 
            // pictPenRectV
            // 
            this.pictPenRectV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictPenRectV.Image = global::InkNote.Properties.Resources.Image4;
            this.pictPenRectV.Location = new System.Drawing.Point(71, 47);
            this.pictPenRectV.Name = "pictPenRectV";
            this.pictPenRectV.Size = new System.Drawing.Size(18, 18);
            this.pictPenRectV.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictPenRectV.TabIndex = 13;
            this.pictPenRectV.TabStop = false;
            this.pictPenRectV.Click += new System.EventHandler(this.pictPenRectV_Click);
            // 
            // pictDelete
            // 
            this.pictDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictDelete.Image = global::InkNote.Properties.Resources.Image8;
            this.pictDelete.Location = new System.Drawing.Point(140, 24);
            this.pictDelete.Name = "pictDelete";
            this.pictDelete.Size = new System.Drawing.Size(18, 18);
            this.pictDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictDelete.TabIndex = 14;
            this.pictDelete.TabStop = false;
            this.pictDelete.Click += new System.EventHandler(this.pictDelete_Click);
            // 
            // pictGrid
            // 
            this.pictGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictGrid.Image = global::InkNote.Properties.Resources.Image9;
            this.pictGrid.Location = new System.Drawing.Point(117, 24);
            this.pictGrid.Name = "pictGrid";
            this.pictGrid.Size = new System.Drawing.Size(18, 18);
            this.pictGrid.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictGrid.TabIndex = 15;
            this.pictGrid.TabStop = false;
            this.pictGrid.Click += new System.EventHandler(this.pictGrid_Click);
            // 
            // pictUndo
            // 
            this.pictUndo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictUndo.Image = global::InkNote.Properties.Resources.Image10;
            this.pictUndo.Location = new System.Drawing.Point(163, 24);
            this.pictUndo.Name = "pictUndo";
            this.pictUndo.Size = new System.Drawing.Size(18, 18);
            this.pictUndo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictUndo.TabIndex = 16;
            this.pictUndo.TabStop = false;
            this.pictUndo.Click += new System.EventHandler(this.pictUndo_Click);
            // 
            // pictColor
            // 
            this.pictColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictColor.Image = global::InkNote.Properties.Resources.Image17;
            this.pictColor.Location = new System.Drawing.Point(140, 2);
            this.pictColor.Name = "pictColor";
            this.pictColor.Size = new System.Drawing.Size(18, 18);
            this.pictColor.TabIndex = 17;
            this.pictColor.TabStop = false;
            this.pictColor.Click += new System.EventHandler(this.pictColor_Click);
            // 
            // pictErase
            // 
            this.pictErase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictErase.Image = global::InkNote.Properties.Resources.Image12;
            this.pictErase.Location = new System.Drawing.Point(140, 47);
            this.pictErase.Name = "pictErase";
            this.pictErase.Size = new System.Drawing.Size(18, 18);
            this.pictErase.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictErase.TabIndex = 18;
            this.pictErase.TabStop = false;
            this.pictErase.Click += new System.EventHandler(this.pictErase_Click);
            // 
            // pictPick
            // 
            this.pictPick.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictPick.Image = global::InkNote.Properties.Resources.Image13;
            this.pictPick.Location = new System.Drawing.Point(94, 47);
            this.pictPick.Name = "pictPick";
            this.pictPick.Size = new System.Drawing.Size(18, 18);
            this.pictPick.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictPick.TabIndex = 19;
            this.pictPick.TabStop = false;
            this.pictPick.Click += new System.EventHandler(this.pictPick_Click);
            // 
            // pictCleaner
            // 
            this.pictCleaner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictCleaner.Image = global::InkNote.Properties.Resources.Image14;
            this.pictCleaner.Location = new System.Drawing.Point(163, 47);
            this.pictCleaner.Name = "pictCleaner";
            this.pictCleaner.Size = new System.Drawing.Size(18, 18);
            this.pictCleaner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictCleaner.TabIndex = 20;
            this.pictCleaner.TabStop = false;
            this.pictCleaner.Click += new System.EventHandler(this.pictCleaner_Click);
            // 
            // pictNew
            // 
            this.pictNew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictNew.Image = global::InkNote.Properties.Resources.Image15;
            this.pictNew.Location = new System.Drawing.Point(2, 24);
            this.pictNew.Name = "pictNew";
            this.pictNew.Size = new System.Drawing.Size(18, 18);
            this.pictNew.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictNew.TabIndex = 21;
            this.pictNew.TabStop = false;
            this.pictNew.Click += new System.EventHandler(this.pictNew_Click);
            // 
            // pictSelInk
            // 
            this.pictSelInk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictSelInk.Image = global::InkNote.Properties.Resources.Image18;
            this.pictSelInk.Location = new System.Drawing.Point(117, 47);
            this.pictSelInk.Name = "pictSelInk";
            this.pictSelInk.Size = new System.Drawing.Size(18, 18);
            this.pictSelInk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictSelInk.TabIndex = 22;
            this.pictSelInk.TabStop = false;
            this.pictSelInk.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictSelInk_MouseClick);
            // 
            // pictPaste
            // 
            this.pictPaste.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictPaste.Image = global::InkNote.Properties.Resources.image19;
            this.pictPaste.Location = new System.Drawing.Point(71, 24);
            this.pictPaste.Name = "pictPaste";
            this.pictPaste.Size = new System.Drawing.Size(18, 18);
            this.pictPaste.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictPaste.TabIndex = 23;
            this.pictPaste.TabStop = false;
            this.pictPaste.Click += new System.EventHandler(this.pictPaste_Click);
            // 
            // pictTransparent
            // 
            this.pictTransparent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictTransparent.Image = global::InkNote.Properties.Resources.Image20;
            this.pictTransparent.Location = new System.Drawing.Point(163, 2);
            this.pictTransparent.Name = "pictTransparent";
            this.pictTransparent.Size = new System.Drawing.Size(18, 18);
            this.pictTransparent.TabIndex = 24;
            this.pictTransparent.TabStop = false;
            this.pictTransparent.Click += new System.EventHandler(this.pictTransparent_Click);
            // 
            // Palette
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 67);
            this.Controls.Add(this.pictTransparent);
            this.Controls.Add(this.pictPaste);
            this.Controls.Add(this.pictSelInk);
            this.Controls.Add(this.pictNew);
            this.Controls.Add(this.pictCleaner);
            this.Controls.Add(this.pictPick);
            this.Controls.Add(this.pictErase);
            this.Controls.Add(this.pictColor);
            this.Controls.Add(this.pictUndo);
            this.Controls.Add(this.pictGrid);
            this.Controls.Add(this.pictDelete);
            this.Controls.Add(this.pictPenRectV);
            this.Controls.Add(this.pictPenRectH);
            this.Controls.Add(this.pictPenBallL);
            this.Controls.Add(this.pictPenBallS);
            this.Controls.Add(this.pictSelect);
            this.Controls.Add(this.pictCopy);
            this.Controls.Add(this.pictSave);
            this.Controls.Add(this.panelColorBlack);
            this.Controls.Add(this.panelColorWhite);
            this.Controls.Add(this.panelColorGreen);
            this.Controls.Add(this.panelColorYellow);
            this.Controls.Add(this.panelColorBlue);
            this.Controls.Add(this.panelColorRed);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Palette";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Palette - InkNote";
            this.Activated += new System.EventHandler(this.Palette_Activated);
            this.Deactivate += new System.EventHandler(this.Palette_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Palette_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Palette_FormClosed);
            this.Load += new System.EventHandler(this.Palette_Load);
            this.VisibleChanged += new System.EventHandler(this.Palette_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenBallS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenBallL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenRectH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPenRectV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictUndo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictErase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCleaner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictNew)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictSelInk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPaste)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictTransparent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelColorRed;
        private System.Windows.Forms.Panel panelColorBlue;
        private System.Windows.Forms.Panel panelColorYellow;
        private System.Windows.Forms.Panel panelColorGreen;
        private System.Windows.Forms.Panel panelColorWhite;
        private System.Windows.Forms.Panel panelColorBlack;
        private System.Windows.Forms.PictureBox pictSave;
        private System.Windows.Forms.PictureBox pictCopy;
        private System.Windows.Forms.PictureBox pictSelect;
        private System.Windows.Forms.PictureBox pictPenBallS;
        private System.Windows.Forms.PictureBox pictPenBallL;
        private System.Windows.Forms.PictureBox pictPenRectH;
        private System.Windows.Forms.PictureBox pictPenRectV;
        private System.Windows.Forms.PictureBox pictDelete;
        private System.Windows.Forms.PictureBox pictGrid;
        private System.Windows.Forms.PictureBox pictUndo;
        private System.Windows.Forms.PictureBox pictColor;
        private System.Windows.Forms.PictureBox pictErase;
        private System.Windows.Forms.PictureBox pictPick;
        private System.Windows.Forms.PictureBox pictCleaner;
        private System.Windows.Forms.PictureBox pictNew;
        private System.Windows.Forms.PictureBox pictSelInk;
        private System.Windows.Forms.PictureBox pictPaste;
        private System.Windows.Forms.PictureBox pictTransparent;
    }
}