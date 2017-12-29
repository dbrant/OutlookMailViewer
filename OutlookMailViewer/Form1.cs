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
        private bool allowNextWebViewLink;

        public Form1()
        {
            InitializeComponent();
            Text = Application.ProductName;
            Utils.FixDialogFont(this);
            Utils.FixWindowTheme(treeViewFolders);
            Utils.FixWindowTheme(listViewMessages);
            
            textBoxPlainText.Font = new Font(FontFamily.GenericMonospace, 10f);
            textBoxHeaders.Font = new Font(FontFamily.GenericMonospace, 10f);
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
                currentFile = new PSTFile(fileName);

                treeViewFolders.Nodes.Clear();
                LayoutFolders(null, currentFile.TopOfPST);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        
        private void LayoutFolders(TreeNode parent, MailFolder folder)
        {
            string nodeText = folder.DisplayName;
            if (folder.Count() > 0)
            {
                nodeText += " (" + folder.Count() + ")";
            }

            var node = parent != null
                ? parent.Nodes.Add(nodeText)
                : treeViewFolders.Nodes.Add(nodeText);
            node.Tag = folder;
            node.ImageKey = node.SelectedImageKey = folder.DisplayName.ToLower().Contains("inbox") ? "inbox" : "folder";
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
            try
            {
                Utils.LockWindowUpdate(listViewMessages);
                listViewMessages.Items.Clear();
                MailFolder folder = (MailFolder)treeViewFolders.SelectedNode.Tag;

                foreach (var item in folder)
                {
                    if (item is PSTParse.Message_Layer.Message)
                    {
                        var message = item as PSTParse.Message_Layer.Message;
                        var listItem = listViewMessages.Items.Add(message.Subject);
                        listItem.Tag = message;
                        listItem.ImageKey = "textgeneric";
                        listItem.SubItems.Add(String.Join("; ", message.To.Select(r => r.EmailAddress)));
                        listItem.SubItems.Add(String.Join("; ", message.From.Select(r => r.EmailAddress)));
                    }
                }
            }
            finally
            {
                Utils.UnlockWindowUpdate();
            }
        }

        private void listViewMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMessages.SelectedItems.Count == 0)
            {
                return;
            }
            var message = (PSTParse.Message_Layer.Message)listViewMessages.SelectedItems[0].Tag;

            allowNextWebViewLink = true;
            webBrowser1.DocumentText = message.HtmlBody != null ? message.HtmlBody : "";
            textBoxPlainText.Text = message.BodyPlainText != null ? message.BodyPlainText : "";
            textBoxHeaders.Text = message.Headers != null ? message.Headers : "";

            if (message.HtmlBody != null)
            {
                tabControlContents.SelectedTab = tabPageHtml;
            }
            else if (message.BodyPlainText != null)
            {
                tabControlContents.SelectedTab = tabPagePlainText;
            }

            listViewDetails.Items.Clear();
            foreach (var prop in message.AllProperties)
            {
                var item = listViewDetails.Items.Add("0x" + Convert.ToString(prop.Key, 16));
                item.ImageKey = "information";
                item.SubItems.Add(prop.Value);
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
    }
}
