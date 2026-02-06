using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RateController
{
    public class VerticalProgressBar : ProgressBar
    {
        private const int PBS_VERTICAL = 0x04;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= PBS_VERTICAL;
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetWindowTheme(this.Handle, "", "");
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(
            IntPtr hWnd,
            string pszSubAppName,
            string pszSubIdList);
    }
}
