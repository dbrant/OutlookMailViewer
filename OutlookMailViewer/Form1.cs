using PSTParse;
using PSTParse.Message_Layer;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OutlookMailViewer
{
    /// <summary>
    /// Copyright 2018 Dmitry Brant.
    /// </summary>
    public partial class Form1 : Form
    {
        private PSTFile currentFile;
        private MailFolder currentFolder;

        private bool allowNextWebViewLink;
        private bool sortAscending;
        private Font messageUnreadFont;

        public Form1()
        {
            InitializeComponent();
            Text = Application.ProductName;
            Utils.FixDialogFont(this);
            Utils.FixWindowTheme(treeViewFolders);
            Utils.FixWindowTheme(listViewMessages);
            Utils.FixWindowTheme(listViewDetails);

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
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            
            sortAscending = false;
            SortByDate();
            listViewMessages.VirtualListSize = currentFolder.Messages.Count;
        }

        private void listViewMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMessages.SelectedIndices.Count == 0)
            {
                return;
            }
            var message = currentFolder.Messages[listViewMessages.SelectedIndices[0]];

            allowNextWebViewLink = true;
            webBrowser1.DocumentText = message.HtmlBody != null
                ? message.HtmlBody
                : (message.BodyPlainText != null ? message.BodyPlainText.Replace("\n", "<br />") : "");
            textBoxPlainText.Text = message.BodyPlainText != null ? message.BodyPlainText : "";
            textBoxHeaders.Text = message.Headers != null ? message.Headers : "";
            
            listViewDetails.Items.Clear();
            foreach (var prop in message.AllProperties)
            {
                if (prop.ID == MessageProperty.BodyPlainText || prop.ID == MessageProperty.BodyCompressedRTF
                    || prop.ID == MessageProperty.BodyHtml || prop.ID == MessageProperty.Headers)
                {
                    continue;
                }
                var item = listViewDetails.Items.Add("0x" + Convert.ToString((int)prop.ID, 16) + " - " + prop.ID.ToString());
                item.ImageKey = "information";
                item.SubItems.Add(MessagePropertyTypes.PropertyToString(currentFile.Header.isUnicode, prop.ID, prop.Data));
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
            sortAscending = !sortAscending;
            if (e.Column == 0)
            {
                currentFolder.Messages.Sort((a, b) => (a.Subject != null && b.Subject != null)
                ? (sortAscending ? a.Subject.CompareTo(b.Subject) : b.Subject.CompareTo(a.Subject))
                : 0);
            }
            else if (e.Column == 1)
            {
                SortByDate();
            }
            listViewMessages.Invalidate();
        }

        private void SortByDate()
        {
            currentFolder.Messages.Sort((a, b) => (a.ClientSubmitTime != null && b.ClientSubmitTime != null)
                ? (sortAscending ? a.ClientSubmitTime.CompareTo(b.ClientSubmitTime) : b.ClientSubmitTime.CompareTo(a.ClientSubmitTime))
                : 0);
        }
        
        private void listViewMessages_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var message = currentFolder.Messages[e.ItemIndex];
            e.Item = new ListViewItem(message.Subject);
            e.Item.Tag = message;
            e.Item.ImageIndex = 2;
            e.Item.SubItems.Add(message.ClientSubmitTime.ToString());
            e.Item.SubItems.Add(message.From.Count > 0
                ? String.Join("; ", message.From.Select(r => r.EmailAddress))
                : message.FromHeaderField);
            e.Item.SubItems.Add(String.Join("; ", message.To.Select(r => r.EmailAddress)));
            if (!message.Read)
            {
                e.Item.Font = messageUnreadFont;
            }
        }
    }
}
