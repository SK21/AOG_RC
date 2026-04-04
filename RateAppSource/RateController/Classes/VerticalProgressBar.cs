using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RateController
{
    // Minimal-style vertical tank level indicator (Option 9).
    // BackColor  = theme background
    // ForeColor  = liquid color (default LimeGreen)
    // BorderColor = tick/border color — set by form to match theme (default Gray)
    // Value      = 0-100 percent full
    public class VerticalProgressBar : Control
    {
        private int _value = 0;
        private Color _borderColor = Color.Gray;
        private const int Radius = 8;

        public int Value
        {
            get => _value;
            set { _value = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        public VerticalProgressBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRegion();
        }

        private void ApplyRegion()
        {
            if (Width > 0 && Height > 0)
            {
                using (var path = RoundedRect(new Rectangle(0, 0, Width, Height), Radius))
                    Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = Width;
            int h = Height;
            var full = new Rectangle(0, 0, w, h);
            var inner = new Rectangle(1, 1, w - 2, h - 2);

            int liquidH = (int)(h * _value / 100.0);
            int liquidY = h - liquidH;

            // ── Background ──────────────────────────────────────────────────
            using (var bg = new SolidBrush(BackColor))
                g.FillRectangle(bg, full);

            // ── Liquid ───────────────────────────────────────────────────────
            if (liquidH > 0)
            {
                var liqRect = new Rectangle(0, liquidY, w, liquidH);
                using (var liqBrush = new LinearGradientBrush(
                    liquidH > 1 ? liqRect : new Rectangle(0, liquidY, w, 1),
                    Lighten(ForeColor, 55), ForeColor, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(liqBrush, liqRect);
                }

                // ── Glass highlight — full-height strip on left ───────────────
                int hlW = Math.Max(5, w / 3);
                var hlRect = new Rectangle(2, liquidY, hlW, liquidH);
                if (hlRect.Width > 0 && hlRect.Height > 0)
                {
                    using (var hlBrush = new LinearGradientBrush(
                        hlRect,
                        Color.FromArgb(190, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255),
                        LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(hlBrush, hlRect);
                    }
                }
            }

            // ── Inner BackColor gap — drawn first so main border overlaps it ──
            var gapRect = new Rectangle(2, 2, w - 4, h - 4);
            using (var gapPath = RoundedRect(gapRect, Math.Max(1, Radius - 2)))
            using (var gapPen  = new Pen(BackColor, 4f))
                g.DrawPath(gapPen, gapPath);

            // ── Main border — drawn on top, covers outer half of gap ──────────
            var borderRect = new Rectangle(2, 2, w - 4, h - 4);
            using (var borderPath = RoundedRect(borderRect, Math.Max(1, Radius - 2)))
            using (var borderPen  = new Pen(_borderColor, 2f))
                g.DrawPath(borderPen, borderPath);

            // ── Tick marks (25%, 50%, 75%) — fully opaque BorderColor ─────────
            using (var tickPen = new Pen(Color.Black, 1.5f))
            {
                DrawTick(g, tickPen, inner, 25, major: false);
                DrawTick(g, tickPen, inner, 50, major: true);
                DrawTick(g, tickPen, inner, 75, major: false);
            }

            // ── % label — midpoint between 50% and 75% ticks ─────────────────
            string pctText = _value + "%";
            using (var f = new Font("Arial", 11f, FontStyle.Bold))
            using (var sf = (StringFormat)StringFormat.GenericTypographic.Clone())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;

                float y75 = inner.Bottom - inner.Height * 75f / 100f;
                float y50 = inner.Bottom - inner.Height * 50f / 100f;
                float midY = (y75 + y50) / 2f;

                SizeF sz = g.MeasureString(pctText, f, PointF.Empty, sf);
                float tx = (w - sz.Width) / 2f + 2f;   // +2 nudge right
                float ty = midY - sz.Height / 2f;

                using (var shadow = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                    g.DrawString(pctText, f, shadow, tx + 1, ty + 1, sf);
                using (var tb = new SolidBrush(Color.Black))
                    g.DrawString(pctText, f, tb, tx, ty, sf);
            }
        }

        private static void DrawTick(Graphics g, Pen pen, Rectangle r, int pct, bool major)
        {
            int len = major ? 7 : 4;
            float y = r.Bottom - r.Height * pct / 100f;
            y = Math.Max(r.Top, Math.Min(r.Bottom, y));
            g.DrawLine(pen, r.Left, y, r.Left + len, y);
            g.DrawLine(pen, r.Right, y, r.Right - len, y);
        }

        private static GraphicsPath RoundedRect(Rectangle b, int radius)
        {
            int d = Math.Min(radius * 2, Math.Min(b.Width, b.Height));
            var path = new GraphicsPath();
            path.AddArc(b.Left, b.Top, d, d, 180, 90);
            path.AddArc(b.Right - d, b.Top, d, d, 270, 90);
            path.AddArc(b.Right - d, b.Bottom - d, d, d, 0, 90);
            path.AddArc(b.Left, b.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color Lighten(Color c, int amount) =>
            Color.FromArgb(c.A,
                Math.Min(255, c.R + amount),
                Math.Min(255, c.G + amount),
                Math.Min(255, c.B + amount));
    }
}
