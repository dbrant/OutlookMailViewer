using System.Windows.Forms;

namespace OutlookMailViewer
{
    public class ListViewDblBuf : ListView
    {
        public ListViewDblBuf()
        {
            DoubleBuffered = true;
        }
    }
}
