using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Printing
{
    public class RichTextBoxPrinterRTF
    {
        private readonly RichTextBox _rtb;
        private int _charFrom;

        public RichTextBoxPrinterRTF(RichTextBox rtb)
        {
            _rtb = rtb ?? throw new ArgumentNullException(nameof(rtb));
        }

        public void Print()
        {
            PrintDocument pd = new PrintDocument();
            pd.BeginPrint += (s, e) => _charFrom = 0;
            pd.PrintPage += OnPrintPage;

            PrintDialog dlg = new PrintDialog { Document = pd };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            _charFrom = FormatRange(e, _charFrom, _rtb.TextLength);

            // More pages?
            e.HasMorePages = (_charFrom < _rtb.TextLength);

            if (!e.HasMorePages)
            {
                FormatRangeDone(_rtb);
            }
        }

        #region Native Interop

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FORMATRANGE
        {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public RECT rc;
            public RECT rcPage;
            public CHARRANGE chrg;
        }

        private const int WM_USER = 0x0400;
        private const int EM_FORMATRANGE = WM_USER + 57;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private int FormatRange(PrintPageEventArgs e, int charFrom, int charTo)
        {
            // Convert margins to twips (1/1440 inch)
            RECT rcPage = new RECT
            {
                Top = HundredthInchToTwips(e.PageBounds.Top),
                Bottom = HundredthInchToTwips(e.PageBounds.Bottom),
                Left = HundredthInchToTwips(e.PageBounds.Left),
                Right = HundredthInchToTwips(e.PageBounds.Right)
            };

            RECT rc = new RECT
            {
                Top = HundredthInchToTwips(e.MarginBounds.Top),
                Bottom = HundredthInchToTwips(e.MarginBounds.Bottom),
                Left = HundredthInchToTwips(e.MarginBounds.Left),
                Right = HundredthInchToTwips(e.MarginBounds.Right)
            };

            CHARRANGE cr = new CHARRANGE { cpMin = charFrom, cpMax = charTo };

            // Get HDC once
            IntPtr hdc = e.Graphics.GetHdc();

            FORMATRANGE fr = new FORMATRANGE
            {
                hdc = hdc,
                hdcTarget = hdc,
                rc = rc,
                rcPage = rcPage,
                chrg = cr
            };

            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
            Marshal.StructureToPtr(fr, lParam, false);

            IntPtr res = SendMessage(_rtb.Handle, EM_FORMATRANGE, (IntPtr)1, lParam);

            Marshal.FreeCoTaskMem(lParam);

            // Release HDC once
            e.Graphics.ReleaseHdc(hdc);

            return res.ToInt32();
        }

        private void FormatRangeDone(RichTextBox rtb)
        {
            SendMessage(rtb.Handle, EM_FORMATRANGE, (IntPtr)0, IntPtr.Zero);
        }

        private int HundredthInchToTwips(int n)
        {
            return (int)(n * 14.4); // 1 hundredth inch = 14.4 twips
        }

        #endregion
    }
}
