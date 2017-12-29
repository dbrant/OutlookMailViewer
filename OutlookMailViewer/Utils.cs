using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OutlookMailViewer
{
    public static class Utils
    {

        /// <summary>
        /// Sets the font of a given control, and all child controls, to
        /// the current system font, while preserving font styles.
        /// </summary>
        /// <param name="c0">Control whose font will be set.</param>
        public static void FixDialogFont(Control c0)
        {
            Font old = c0.Font;
            c0.Font = new Font(SystemFonts.MessageBoxFont.FontFamily.Name, old.Size, old.Style);
            if (c0.Controls.Count > 0)
            {
                foreach (Control c in c0.Controls)
                {
                    FixDialogFont(c);
                }
            }
        }

        /// <summary>
        /// Sets the proper visual style for a ListView or TreeView control, so that it looks
        /// more like the list control in Explorer.
        /// </summary>
        /// <param name="lv">ListView or TreeView control to fix.</param>
        public static void FixWindowTheme(Control ctl)
        {
            SetWindowTheme(ctl.Handle, "Explorer", null);
        }

        public static void LockWindowUpdate(Control control)
        {
            LockWindowUpdate(control.Handle);
        }

        public static void UnlockWindowUpdate()
        {
            LockWindowUpdate(IntPtr.Zero);
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

        [DllImport("user32.dll")]
        private static extern bool LockWindowUpdate(IntPtr hWndLock);

    }
}
