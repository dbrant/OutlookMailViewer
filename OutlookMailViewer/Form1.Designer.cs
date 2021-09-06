namespace OutlookMailViewer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewFolders = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControlContents = new System.Windows.Forms.TabControl();
            this.tabPageHtml = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tabPagePlainText = new System.Windows.Forms.TabPage();
            this.textBoxPlainText = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBoxHeaders = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPageAttachments = new System.Windows.Forms.TabPage();
            this.contextMenuStripAttachments = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemSaveAttachment = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.chkSearchSubject = new System.Windows.Forms.CheckBox();
            this.chkSearchBody = new System.Windows.Forms.CheckBox();
            this.chkSearchFrom = new System.Windows.Forms.CheckBox();
            this.chkSearchTo = new System.Windows.Forms.CheckBox();
            this.chkSearchAttachments = new System.Windows.Forms.CheckBox();
            this.chkSearchHeaders = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnSearchClose = new System.Windows.Forms.Button();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFind = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewMessages = new OutlookMailViewer.ListViewDblBuf();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewDetails = new OutlookMailViewer.ListViewDblBuf();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewAttachments = new OutlookMailViewer.ListViewDblBuf();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControlContents.SuspendLayout();
            this.tabPageHtml.SuspendLayout();
            this.tabPagePlainText.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPageAttachments.SuspendLayout();
            this.contextMenuStripAttachments.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1024, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(8, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewFolders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1009, 580);
            this.splitContainer1.SplitterDistance = 301;
            this.splitContainer1.TabIndex = 2;
            // 
            // treeViewFolders
            // 
            this.treeViewFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewFolders.HideSelection = false;
            this.treeViewFolders.ImageIndex = 0;
            this.treeViewFolders.ImageList = this.imageList1;
            this.treeViewFolders.Location = new System.Drawing.Point(0, 0);
            this.treeViewFolders.Name = "treeViewFolders";
            this.treeViewFolders.SelectedImageIndex = 0;
            this.treeViewFolders.Size = new System.Drawing.Size(301, 580);
            this.treeViewFolders.TabIndex = 0;
            this.treeViewFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFolders_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder");
            this.imageList1.Images.SetKeyName(1, "folderopen");
            this.imageList1.Images.SetKeyName(2, "mail");
            this.imageList1.Images.SetKeyName(3, "mailopen");
            this.imageList1.Images.SetKeyName(4, "mailattach");
            this.imageList1.Images.SetKeyName(5, "trash");
            this.imageList1.Images.SetKeyName(6, "inbox");
            this.imageList1.Images.SetKeyName(7, "inboxempty");
            this.imageList1.Images.SetKeyName(8, "information");
            this.imageList1.Images.SetKeyName(9, "documentsub");
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listViewMessages);
            this.splitContainer2.Panel1.Controls.Add(this.panelSearch);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControlContents);
            this.splitContainer2.Size = new System.Drawing.Size(704, 580);
            this.splitContainer2.SplitterDistance = 329;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabControlContents
            // 
            this.tabControlContents.Controls.Add(this.tabPageHtml);
            this.tabControlContents.Controls.Add(this.tabPagePlainText);
            this.tabControlContents.Controls.Add(this.tabPage3);
            this.tabControlContents.Controls.Add(this.tabPage4);
            this.tabControlContents.Controls.Add(this.tabPageAttachments);
            this.tabControlContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlContents.Location = new System.Drawing.Point(0, 0);
            this.tabControlContents.Name = "tabControlContents";
            this.tabControlContents.SelectedIndex = 0;
            this.tabControlContents.Size = new System.Drawing.Size(704, 247);
            this.tabControlContents.TabIndex = 1;
            // 
            // tabPageHtml
            // 
            this.tabPageHtml.Controls.Add(this.webBrowser1);
            this.tabPageHtml.Location = new System.Drawing.Point(4, 22);
            this.tabPageHtml.Name = "tabPageHtml";
            this.tabPageHtml.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHtml.Size = new System.Drawing.Size(696, 221);
            this.tabPageHtml.TabIndex = 0;
            this.tabPageHtml.Text = "HTML";
            this.tabPageHtml.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(3, 3);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(690, 215);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser1_Navigating);
            // 
            // tabPagePlainText
            // 
            this.tabPagePlainText.Controls.Add(this.textBoxPlainText);
            this.tabPagePlainText.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlainText.Name = "tabPagePlainText";
            this.tabPagePlainText.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlainText.Size = new System.Drawing.Size(696, 221);
            this.tabPagePlainText.TabIndex = 1;
            this.tabPagePlainText.Text = "Plain text";
            this.tabPagePlainText.UseVisualStyleBackColor = true;
            // 
            // textBoxPlainText
            // 
            this.textBoxPlainText.AcceptsTab = true;
            this.textBoxPlainText.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxPlainText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPlainText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPlainText.Location = new System.Drawing.Point(3, 3);
            this.textBoxPlainText.Multiline = true;
            this.textBoxPlainText.Name = "textBoxPlainText";
            this.textBoxPlainText.ReadOnly = true;
            this.textBoxPlainText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPlainText.Size = new System.Drawing.Size(690, 215);
            this.textBoxPlainText.TabIndex = 0;
            this.textBoxPlainText.WordWrap = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textBoxHeaders);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(696, 221);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Headers";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textBoxHeaders
            // 
            this.textBoxHeaders.AcceptsTab = true;
            this.textBoxHeaders.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxHeaders.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxHeaders.Location = new System.Drawing.Point(3, 3);
            this.textBoxHeaders.Multiline = true;
            this.textBoxHeaders.Name = "textBoxHeaders";
            this.textBoxHeaders.ReadOnly = true;
            this.textBoxHeaders.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxHeaders.Size = new System.Drawing.Size(690, 215);
            this.textBoxHeaders.TabIndex = 1;
            this.textBoxHeaders.WordWrap = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listViewDetails);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(696, 221);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Details";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPageAttachments
            // 
            this.tabPageAttachments.Controls.Add(this.listViewAttachments);
            this.tabPageAttachments.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttachments.Name = "tabPageAttachments";
            this.tabPageAttachments.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAttachments.Size = new System.Drawing.Size(696, 221);
            this.tabPageAttachments.TabIndex = 4;
            this.tabPageAttachments.Text = "Attachments";
            this.tabPageAttachments.UseVisualStyleBackColor = true;
            // 
            // contextMenuStripAttachments
            // 
            this.contextMenuStripAttachments.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSaveAttachment});
            this.contextMenuStripAttachments.Name = "contextMenuStripAttachments";
            this.contextMenuStripAttachments.Size = new System.Drawing.Size(108, 26);
            // 
            // menuItemSaveAttachment
            // 
            this.menuItemSaveAttachment.Name = "menuItemSaveAttachment";
            this.menuItemSaveAttachment.Size = new System.Drawing.Size(107, 22);
            this.menuItemSaveAttachment.Text = "Save...";
            this.menuItemSaveAttachment.Click += new System.EventHandler(this.menuItemSaveAttachment_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 612);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1024, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.btnSearchClose);
            this.panelSearch.Controls.Add(this.btnSearch);
            this.panelSearch.Controls.Add(this.chkSearchAttachments);
            this.panelSearch.Controls.Add(this.chkSearchHeaders);
            this.panelSearch.Controls.Add(this.chkSearchTo);
            this.panelSearch.Controls.Add(this.chkSearchFrom);
            this.panelSearch.Controls.Add(this.chkSearchBody);
            this.panelSearch.Controls.Add(this.chkSearchSubject);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.label1);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(704, 50);
            this.panelSearch.TabIndex = 1;
            this.panelSearch.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(6, 22);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(137, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // chkSearchSubject
            // 
            this.chkSearchSubject.AutoSize = true;
            this.chkSearchSubject.Checked = true;
            this.chkSearchSubject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSearchSubject.Location = new System.Drawing.Point(163, 3);
            this.chkSearchSubject.Name = "chkSearchSubject";
            this.chkSearchSubject.Size = new System.Drawing.Size(62, 17);
            this.chkSearchSubject.TabIndex = 2;
            this.chkSearchSubject.Text = "Subject";
            this.chkSearchSubject.UseVisualStyleBackColor = true;
            // 
            // chkSearchBody
            // 
            this.chkSearchBody.AutoSize = true;
            this.chkSearchBody.Checked = true;
            this.chkSearchBody.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSearchBody.Location = new System.Drawing.Point(163, 26);
            this.chkSearchBody.Name = "chkSearchBody";
            this.chkSearchBody.Size = new System.Drawing.Size(69, 17);
            this.chkSearchBody.TabIndex = 3;
            this.chkSearchBody.Text = "Message";
            this.chkSearchBody.UseVisualStyleBackColor = true;
            // 
            // chkSearchFrom
            // 
            this.chkSearchFrom.AutoSize = true;
            this.chkSearchFrom.Location = new System.Drawing.Point(261, 3);
            this.chkSearchFrom.Name = "chkSearchFrom";
            this.chkSearchFrom.Size = new System.Drawing.Size(60, 17);
            this.chkSearchFrom.TabIndex = 4;
            this.chkSearchFrom.Text = "Sender";
            this.chkSearchFrom.UseVisualStyleBackColor = true;
            // 
            // chkSearchTo
            // 
            this.chkSearchTo.AutoSize = true;
            this.chkSearchTo.Location = new System.Drawing.Point(261, 26);
            this.chkSearchTo.Name = "chkSearchTo";
            this.chkSearchTo.Size = new System.Drawing.Size(71, 17);
            this.chkSearchTo.TabIndex = 5;
            this.chkSearchTo.Text = "Recipient";
            this.chkSearchTo.UseVisualStyleBackColor = true;
            // 
            // chkSearchAttachments
            // 
            this.chkSearchAttachments.AutoSize = true;
            this.chkSearchAttachments.Location = new System.Drawing.Point(357, 26);
            this.chkSearchAttachments.Name = "chkSearchAttachments";
            this.chkSearchAttachments.Size = new System.Drawing.Size(85, 17);
            this.chkSearchAttachments.TabIndex = 7;
            this.chkSearchAttachments.Text = "Attachments";
            this.chkSearchAttachments.UseVisualStyleBackColor = true;
            // 
            // chkSearchHeaders
            // 
            this.chkSearchHeaders.AutoSize = true;
            this.chkSearchHeaders.Location = new System.Drawing.Point(357, 3);
            this.chkSearchHeaders.Name = "chkSearchHeaders";
            this.chkSearchHeaders.Size = new System.Drawing.Size(66, 17);
            this.chkSearchHeaders.TabIndex = 6;
            this.chkSearchHeaders.Text = "Headers";
            this.chkSearchHeaders.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(478, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(82, 24);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Find";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnSearchClose
            // 
            this.btnSearchClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearchClose.FlatAppearance.BorderSize = 0;
            this.btnSearchClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchClose.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchClose.Image")));
            this.btnSearchClose.Location = new System.Drawing.Point(678, 0);
            this.btnSearchClose.Name = "btnSearchClose";
            this.btnSearchClose.Size = new System.Drawing.Size(24, 24);
            this.btnSearchClose.TabIndex = 9;
            this.btnSearchClose.UseVisualStyleBackColor = true;
            this.btnSearchClose.Click += new System.EventHandler(this.btnSearchClose_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFind});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // mnuFind
            // 
            this.mnuFind.Image = ((System.Drawing.Image)(resources.GetObject("mnuFind.Image")));
            this.mnuFind.Name = "mnuFind";
            this.mnuFind.Size = new System.Drawing.Size(106, 22);
            this.mnuFind.Text = "Find...";
            this.mnuFind.Click += new System.EventHandler(this.mnuFind_Click);
            // 
            // listViewMessages
            // 
            this.listViewMessages.AllowColumnReorder = true;
            this.listViewMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader6,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewMessages.FullRowSelect = true;
            this.listViewMessages.HideSelection = false;
            this.listViewMessages.Location = new System.Drawing.Point(0, 50);
            this.listViewMessages.Name = "listViewMessages";
            this.listViewMessages.Size = new System.Drawing.Size(704, 279);
            this.listViewMessages.SmallImageList = this.imageList1;
            this.listViewMessages.TabIndex = 2;
            this.listViewMessages.UseCompatibleStateImageBehavior = false;
            this.listViewMessages.View = System.Windows.Forms.View.Details;
            this.listViewMessages.VirtualMode = true;
            this.listViewMessages.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewMessages_ColumnClick);
            this.listViewMessages.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listViewMessages_RetrieveVirtualItem);
            this.listViewMessages.SelectedIndexChanged += new System.EventHandler(this.listViewMessages_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Subject";
            this.columnHeader1.Width = 260;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Date";
            this.columnHeader6.Width = 160;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "From";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "To";
            this.columnHeader3.Width = 200;
            // 
            // listViewDetails
            // 
            this.listViewDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.listViewDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDetails.FullRowSelect = true;
            this.listViewDetails.HideSelection = false;
            this.listViewDetails.Location = new System.Drawing.Point(3, 3);
            this.listViewDetails.Name = "listViewDetails";
            this.listViewDetails.Size = new System.Drawing.Size(690, 215);
            this.listViewDetails.SmallImageList = this.imageList1;
            this.listViewDetails.TabIndex = 1;
            this.listViewDetails.UseCompatibleStateImageBehavior = false;
            this.listViewDetails.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Property";
            this.columnHeader4.Width = 200;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Value";
            this.columnHeader5.Width = 400;
            // 
            // listViewAttachments
            // 
            this.listViewAttachments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.listViewAttachments.ContextMenuStrip = this.contextMenuStripAttachments;
            this.listViewAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAttachments.FullRowSelect = true;
            this.listViewAttachments.HideSelection = false;
            this.listViewAttachments.Location = new System.Drawing.Point(3, 3);
            this.listViewAttachments.Name = "listViewAttachments";
            this.listViewAttachments.Size = new System.Drawing.Size(690, 215);
            this.listViewAttachments.SmallImageList = this.imageList1;
            this.listViewAttachments.TabIndex = 2;
            this.listViewAttachments.UseCompatibleStateImageBehavior = false;
            this.listViewAttachments.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Name";
            this.columnHeader7.Width = 240;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Size";
            this.columnHeader8.Width = 80;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Type";
            this.columnHeader9.Width = 160;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 634);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControlContents.ResumeLayout(false);
            this.tabPageHtml.ResumeLayout(false);
            this.tabPagePlainText.ResumeLayout(false);
            this.tabPagePlainText.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPageAttachments.ResumeLayout(false);
            this.contextMenuStripAttachments.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeViewFolders;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabControl tabControlContents;
        private System.Windows.Forms.TabPage tabPageHtml;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TabPage tabPagePlainText;
        private System.Windows.Forms.TextBox textBoxPlainText;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox textBoxHeaders;
        private System.Windows.Forms.TabPage tabPage4;
        private ListViewDblBuf listViewDetails;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabPage tabPageAttachments;
        private ListViewDblBuf listViewAttachments;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAttachments;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveAttachment;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private ListViewDblBuf listViewMessages;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Button btnSearchClose;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.CheckBox chkSearchAttachments;
        private System.Windows.Forms.CheckBox chkSearchHeaders;
        private System.Windows.Forms.CheckBox chkSearchTo;
        private System.Windows.Forms.CheckBox chkSearchFrom;
        private System.Windows.Forms.CheckBox chkSearchBody;
        private System.Windows.Forms.CheckBox chkSearchSubject;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuFind;
    }
}

