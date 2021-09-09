using MsgKit;
using PSTParse;
using PSTParse.LTP;
using PSTParse.Message_Layer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OutlookMailViewer
{
    /// <summary>
    /// Copyright 2018-2021 Dmitry Brant.
    /// </summary>
    public partial class Form1 : Form
    {
        private PSTFile currentFile;
        private MailFolder currentFolder;
        private List<PSTParse.Message_Layer.Message> displayedMessages = new List<PSTParse.Message_Layer.Message>();

        private bool allowNextWebViewLink;
        private SortOrder sortOrder = SortOrder.Date;
        private bool sortAscending = false;
        private Font messageUnreadFont;

        private enum SortOrder
        {
            None = -1, Subject = 0, Date
        }

        public Form1()
        {
            InitializeComponent();
            Text = Application.ProductName;
            Utils.FixDialogFont(this);
            Utils.FixWindowTheme(treeViewFolders);
            Utils.FixWindowTheme(listViewMessages);
            Utils.FixWindowTheme(listViewDetails);
            Utils.FixWindowTheme(listViewAttachments);

            textBoxPlainText.Font = new Font(FontFamily.GenericMonospace, 10f);
            textBoxHeaders.Font = new Font(FontFamily.GenericMonospace, 10f);
            messageUnreadFont = new Font(listViewMessages.Font, FontStyle.Bold);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;
            OpenPST(files[0]);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openDlg = new OpenFileDialog();
            openDlg.DefaultExt = ".mpo";
            openDlg.CheckFileExists = true;
            openDlg.Title = "Open Outlook mail archive...";
            openDlg.Filter = "Outlook files (*.pst, *.ost)|*.pst;*.ost|All Files (*.*)|*.*";
            openDlg.FilterIndex = 1;
            if (openDlg.ShowDialog() == DialogResult.Cancel) return;
            OpenPST(openDlg.FileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void OpenPST(string fileName)
        {
            if (currentFile != null)
            {
                try { currentFile.Dispose(); }
                catch { }
            }
            try
            {
                Cursor = Cursors.WaitCursor;
                currentFile = new PSTFile(fileName);

                treeViewFolders.Nodes.Clear();
                LayoutFolders(null, currentFile.TopOfPST);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + ":\n\n" + ex.StackTrace.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        
        private void LayoutFolders(TreeNode parent, MailFolder folder)
        {
            string nodeText = folder.DisplayName;
            if (folder.Messages.Count > 0)
            {
                nodeText += " (" + folder.Messages.Count + ")";
            }

            var node = parent != null
                ? parent.Nodes.Add(nodeText)
                : treeViewFolders.Nodes.Add(nodeText);
            node.Tag = folder;
            if (folder.DisplayName.ToLower().Contains("inbox"))
            {
                node.ImageKey = node.SelectedImageKey = "inbox";
            }
            else
            {
                node.ImageKey = "folder";
                node.SelectedImageKey = "folderopen";
            }
            if (folder.Messages.Count > 0)
            {
                node.NodeFont = new Font(treeViewFolders.Font, FontStyle.Bold);
            }
            foreach (var child in folder.SubFolders)
            {
                LayoutFolders(node, child);
            }
            treeViewFolders.ExpandAll();
        }

        private void treeViewFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewFolders.SelectedNode == null)
            {
                return;
            }
            
            currentFolder = (MailFolder)treeViewFolders.SelectedNode.Tag;
            UpdatePropertyList(currentFolder.PC);
            RegenerateListBySearch();
            listViewMessages.Invalidate();
        }

        private void listViewMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMessages.SelectedIndices.Count == 0)
            {
                return;
            }
            var message = displayedMessages[listViewMessages.SelectedIndices[0]];

            allowNextWebViewLink = true;
            string headers = message.Headers;
            string html = message.HtmlBody;
            string plainText = message.BodyPlainText;
            webBrowser1.DocumentText = html != null ? html : (plainText != null ? plainText.Replace("\n", "<br />") : "");
            textBoxPlainText.Text = plainText != null ? plainText : "";
            textBoxHeaders.Text = headers != null ? headers : "";

            UpdatePropertyList(message.PC);

            listViewAttachments.Items.Clear();
            foreach (var attachment in message.Attachments)
            {
                var item = listViewAttachments.Items.Add(attachment.FileName);
                item.ImageKey = "documentsub";
                item.SubItems.Add(attachment.Size.ToString());
                item.SubItems.Add(attachment.Method.ToString());
            }
        }

        private void UpdatePropertyList(PropertyContext PC)
        {
            listViewDetails.Items.Clear();
            foreach (var prop in PC.Properties)
            {
                if (prop.Value.ID == MessageProperty.BodyPlainText || prop.Value.ID == MessageProperty.BodyCompressedRTF
                    || prop.Value.ID == MessageProperty.BodyHtml || prop.Value.ID == MessageProperty.Headers)
                {
                    continue;
                }
                var item = listViewDetails.Items.Add("0x" + Convert.ToString((int)prop.Value.ID, 16) + " - " + prop.Value.ID.ToString());
                item.ImageKey = "information";
                item.SubItems.Add(MessagePropertyTypes.PropertyToString(currentFile.Header.isUnicode, prop.Value, true));
            }
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (allowNextWebViewLink)
            {
                allowNextWebViewLink = false;
                return;
            }
            // suppress link clicks for now.
            e.Cancel = true;
        }

        private void listViewMessages_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var newSortOrder = (SortOrder)e.Column;
            if (newSortOrder == sortOrder)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortAscending = true;
            }
            sortOrder = newSortOrder;
            UpdateSort();
            listViewMessages.Invalidate();
        }

        private void listViewMessages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var message = displayedMessages[e.ItemIndex];
            e.Item = new ListViewItem(message.Subject);
            e.Item.Tag = message;
            e.Item.SubItems.Add(message.ClientSubmitTime.ToString());

            string fromStr = message.SentRepresentingName != null ? message.SentRepresentingName : message.SenderName;
            e.Item.SubItems.Add(fromStr);
            e.Item.SubItems.Add(String.Join("; ", message.To.Select(r => r.EmailAddress)));

            if (!message.Read)
            {
                e.Item.Font = messageUnreadFont;
            }

            if (message.Attachments.Count > 0)
            {
                e.Item.ImageIndex = 4;
            }
            else if (!message.Read)
            {
                e.Item.ImageIndex = 2;
            }
            else
            {
                e.Item.ImageIndex = 3;
            }
        }

        private void menuItemSaveAttachment_Click(object sender, EventArgs e)
        {
            if (listViewMessages.SelectedIndices.Count == 0 || listViewAttachments.SelectedIndices.Count == 0)
            {
                return;
            }
            var message = displayedMessages[listViewMessages.SelectedIndices[0]];
            if (listViewAttachments.SelectedIndices[0] >= message.Attachments.Count)
            {
                return;
            }
            var attachment = message.Attachments[listViewAttachments.SelectedIndices[0]];
            var saveDlg = new SaveFileDialog();
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "Save attachment...";
            saveDlg.FileName = attachment.FileName;
            if (saveDlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                using (var f = new FileStream(saveDlg.FileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = attachment.Data;
                    f.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Outlook PST mail database viewer.\n\nCopyright 2018-2021 Dmitry Brant.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuFind_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = true;
            txtSearch.Focus();
            txtSearch.SelectAll();
        }

        private void btnSearchClose_Click(object sender, EventArgs e)
        {
            panelSearch.Visible = false;
            RegenerateListBySearch();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RegenerateListBySearch();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                RegenerateListBySearch();
            }
        }

        private void RegenerateListBySearch()
        {
            displayedMessages.Clear();
            if (currentFolder != null && currentFolder.Messages != null)
            {
                if (!panelSearch.Visible || txtSearch.Text.Length == 0)
                {
                    displayedMessages.AddRange(currentFolder.Messages);
                }
                else
                {
                    var searchTerm = txtSearch.Text.ToLowerInvariant();
                    foreach (var message in currentFolder.Messages)
                    {
                        if (
                            (chkSearchSubject.Checked && message.Subject.ToLowerInvariant().Contains(searchTerm)) ||
                            (chkSearchBody.Checked && message.HtmlBody != null && message.HtmlBody.ToLowerInvariant().Contains(searchTerm)) ||
                            (chkSearchBody.Checked && message.BodyPlainText != null && message.BodyPlainText.ToLowerInvariant().Contains(searchTerm)) ||
                            (chkSearchFrom.Checked && message.SentRepresentingName != null && message.SentRepresentingName.ToLowerInvariant().Contains(searchTerm)) ||
                            (chkSearchFrom.Checked && message.SenderName != null && message.SenderName.ToLowerInvariant().Contains(searchTerm)) ||
                            (chkSearchTo.Checked && message.To != null && string.Join("; ", message.To.Select(r => r.EmailAddress.ToLowerInvariant())).Contains(searchTerm)) ||
                            (chkSearchHeaders.Checked && message.Headers != null && message.Headers.ToLowerInvariant().Contains(searchTerm)) ||
                            (chkSearchAttachments.Checked && message.Attachments != null && message.Attachments.Find(a => a.FileName != null && a.FileName.ToLowerInvariant().Contains(searchTerm)) != null)
                            )
                        {
                            displayedMessages.Add(message);
                        }
                    }
                }
            }
            UpdateSort();
            listViewMessages.VirtualListSize = displayedMessages.Count;
        }

        private void UpdateSort()
        {
            if (sortOrder == SortOrder.None)
            {
                return;
            }
            else if (sortOrder == SortOrder.Subject)
            {
                displayedMessages.Sort((a, b) => (a.Subject != null && b.Subject != null)
                ? (sortAscending ? a.Subject.CompareTo(b.Subject) : b.Subject.CompareTo(a.Subject))
                : 0);
            }
            else if (sortOrder == SortOrder.Date)
            {
                displayedMessages.Sort((a, b) => (a.ClientSubmitTime != null && b.ClientSubmitTime != null)
                ? (sortAscending ? a.ClientSubmitTime.CompareTo(b.ClientSubmitTime) : b.ClientSubmitTime.CompareTo(a.ClientSubmitTime))
                : 0);
            }
        }

        private void mnuSaveSelected_Click(object sender, EventArgs e)
        {
            if (listViewMessages.SelectedIndices.Count == 0)
            {
                return;
            }

            string selectedPath = ".";
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.ShowNewFolderButton = true;
                dlg.Description = "Select the folder where the file(s) will be saved:";
                if (dlg.ShowDialog(this) != DialogResult.OK) { return; }
                selectedPath = dlg.SelectedPath;
            }

            foreach (int index in listViewMessages.SelectedIndices)
            {
                var message = displayedMessages[index];

                var senderName = message.SentRepresentingName ?? (message.SenderName ?? "");
                var fromEmail = message.From != null && message.From.Count > 0 ? message.From[0].EmailAddress : "";
                var emailSender = new Sender(fromEmail, senderName);
                var attachmentStreams = new List<Stream>();

                try
                {
                    using (var email = new Email(emailSender, message.Subject ?? ""))
                    {
                        if (message.HtmlBody != null)
                        {
                            email.BodyHtml = message.HtmlBody;
                        }
                        if (message.BodyPlainText != null)
                        {
                            email.BodyText = message.BodyPlainText;
                        }
                        if (message.BodyRTF != null)
                        {
                            email.BodyRtf = message.BodyRTF;
                        }
                        if (message.Attachments != null)
                        {
                            foreach (var att in message.Attachments)
                            {
                                if (att.Data == null) { continue; }
                                var stream = new MemoryStream(att.Data);
                                attachmentStreams.Add(stream);
                                email.Attachments.Add(stream, att.FileName, renderingPosition: (int)att.RenderingPosition, isInline: att.RenderedInBody);
                            }
                        }
                        if (message.MessageId != null)
                        {
                            email.InternetMessageId = message.MessageId;
                        }
                        if (message.ReplyToId != null)
                        {
                            email.InReplyToId = message.ReplyToId;
                        }
                        email.Importance = (MsgKit.Enums.MessageImportance)message.Imporance;

                        if (message.MessageDeliveryTime != null)
                        {
                            email.ReceivedOn = message.MessageDeliveryTime;
                        }
                        if (message.ClientSubmitTime != null)
                        {
                            email.SentOn = message.ClientSubmitTime;
                        }
                        foreach (var rec in message.To)
                        {
                            email.Recipients.AddTo(rec.EmailAddress, rec.DisplayName);
                        }
                        foreach (var rec in message.CC)
                        {
                            email.Recipients.AddCc(rec.EmailAddress, rec.DisplayName);
                        }
                        foreach (var rec in message.BCC)
                        {
                            email.Recipients.AddBcc(rec.EmailAddress, rec.DisplayName);
                        }

                        var fileName = "Message";
                        try
                        {
                            DateTime dt = DateTime.Now;
                            if (message.MessageDeliveryTime != null)
                            {
                                dt = message.MessageDeliveryTime;
                            }
                            if (message.ClientSubmitTime != null)
                            {
                                dt = message.ClientSubmitTime;
                            }
                            fileName += " on " + dt.ToShortDateString() + " " + dt.ToLongTimeString();
                            fileName = fileName.Replace(':', '-');
                        }
                        catch { }
                        email.Save(Path.Combine(selectedPath, fileName + ".msg"));
                    }
                }
                finally
                {
                    foreach (var s in attachmentStreams)
                    {
                        try { s.Dispose(); } catch { }
                    }
                }
            }
        }
    }
}
