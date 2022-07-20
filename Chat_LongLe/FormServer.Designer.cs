namespace Chat_LongLe
{
    partial class FormServer
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
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.sendbtn = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.iconbtn = new System.Windows.Forms.Button();
            this.imagebtn = new System.Windows.Forms.Button();
            this.filebtn = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView2 = new System.Windows.Forms.ListView();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Enabled = false;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(23, 33);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(238, 504);
            this.listBox1.TabIndex = 2;
            // 
            // messageBox
            // 
            this.messageBox.Location = new System.Drawing.Point(420, 559);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(450, 26);
            this.messageBox.TabIndex = 3;
            // 
            // sendbtn
            // 
            this.sendbtn.Location = new System.Drawing.Point(889, 552);
            this.sendbtn.Name = "sendbtn";
            this.sendbtn.Size = new System.Drawing.Size(81, 40);
            this.sendbtn.TabIndex = 4;
            this.sendbtn.Text = "Send";
            this.sendbtn.UseVisualStyleBackColor = true;
            this.sendbtn.Click += new System.EventHandler(this.sendbtn_Click);
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(279, 33);
            this.listView1.Name = "listView1";
            this.listView1.RightToLeftLayout = true;
            this.listView1.Size = new System.Drawing.Size(692, 504);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(82, 50);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // iconbtn
            // 
            this.iconbtn.BackgroundImage = global::Chat_LongLe.Properties.Resources.imoji;
            this.iconbtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconbtn.Location = new System.Drawing.Point(366, 552);
            this.iconbtn.Name = "iconbtn";
            this.iconbtn.Size = new System.Drawing.Size(38, 40);
            this.iconbtn.TabIndex = 9;
            this.iconbtn.UseVisualStyleBackColor = true;
            this.iconbtn.Click += new System.EventHandler(this.iconbtn_Click);
            // 
            // imagebtn
            // 
            this.imagebtn.BackgroundImage = global::Chat_LongLe.Properties.Resources.image;
            this.imagebtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imagebtn.Location = new System.Drawing.Point(322, 552);
            this.imagebtn.Name = "imagebtn";
            this.imagebtn.Size = new System.Drawing.Size(38, 40);
            this.imagebtn.TabIndex = 7;
            this.imagebtn.UseVisualStyleBackColor = true;
            this.imagebtn.Click += new System.EventHandler(this.imagebtn_Click);
            // 
            // filebtn
            // 
            this.filebtn.BackgroundImage = global::Chat_LongLe.Properties.Resources.attachment_clip;
            this.filebtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.filebtn.Location = new System.Drawing.Point(278, 552);
            this.filebtn.Name = "filebtn";
            this.filebtn.Size = new System.Drawing.Size(38, 40);
            this.filebtn.TabIndex = 6;
            this.filebtn.UseVisualStyleBackColor = true;
            this.filebtn.Click += new System.EventHandler(this.filebtn_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 100);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(134, 32);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(134, 32);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(134, 32);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // listView2
            // 
            this.listView2.HideSelection = false;
            this.listView2.LargeImageList = this.imageList2;
            this.listView2.Location = new System.Drawing.Point(366, 279);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(398, 267);
            this.listView2.TabIndex = 11;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.Visible = false;
            this.listView2.Click += new System.EventHandler(this.listView2_Click);
            this.listView2.Leave += new System.EventHandler(this.listView2_Leave);
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(30, 30);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FormServer
            // 
            this.AcceptButton = this.sendbtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(998, 608);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.iconbtn);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.imagebtn);
            this.Controls.Add(this.filebtn);
            this.Controls.Add(this.sendbtn);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.listBox1);
            this.Name = "FormServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormServer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormServer_FormClosed);
            this.Load += new System.EventHandler(this.FormServer_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox messageBox;
        private System.Windows.Forms.Button sendbtn;
        private System.Windows.Forms.Button filebtn;
        private System.Windows.Forms.Button imagebtn;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button iconbtn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ImageList imageList2;
    }
}

