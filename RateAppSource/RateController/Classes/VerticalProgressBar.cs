using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RateController
{
    // Minimal-style vertical tank level indicator.
    // ForeColor  = liquid colour (green by default, settable in designer properties)
    // BackColor  = empty-area colour (set by form to MainBackColour)
    // BorderColor = outer border colour (set by form to DisplayForeColour — matches themed buttons)
    // Value      = 0-100 percent full
    public class VerticalProgressBar : Control
    {
        private int   _value       = 0;
        private Color _borderColor = Color.Gray;
        private const int Radius   = 8;

        public int Value
        {
            get => _value;
            set { _value = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        // Outer border — set by form to match themed button border colour
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

            int w     = Width;
            int h     = Height;
            var full  = new Rectangle(0, 0, w, h);
            var inner = new Rectangle(1, 1, w - 2, h - 2);

            int liquidH = (int)(h * _value / 100.0);
            int liquidY = h - liquidH;

            // Content area sits inside the inner black border (3px inset each side)
            var contentRect = new Rectangle(4, 4, w - 8, h - 8);

            // ── Background (MainBackColour via BackColor) ─────────────────────
            using (var bg = new SolidBrush(BackColor))
                g.FillRectangle(bg, full);

            // ── Liquid — clipped to content rect ─────────────────────────────
            if (liquidH > 0)
            {
                using (var clip = RoundedRect(contentRect, Math.Max(1, Radius - 4)))
                {
                    g.SetClip(clip);

                    var liqRect = new Rectangle(0, liquidY, w, liquidH);
                    using (var liqBrush = new LinearGradientBrush(
                        liquidH > 1 ? liqRect : new Rectangle(0, liquidY, w, 1),
                        Lighten(ForeColor, 55), ForeColor, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(liqBrush, liqRect);
                    }

                    // Glass highlight — white fade on left edge
                    int hlW    = Math.Max(5, w / 3);
                    var hlRect = new Rectangle(4, liquidY, hlW, liquidH);
                    if (hlRect.Width > 0 && hlRect.Height > 0)
                    {
                        using (var hlBrush = new LinearGradientBrush(
                            hlRect,
                            Color.FromArgb(190, 255, 255, 255),
                            Color.FromArgb(0,   255, 255, 255),
                            LinearGradientMode.Horizontal))
                        {
                            g.FillRectangle(hlBrush, hlRect);
                        }
                    }

                    g.ResetClip();
                }
            }

            // ── Inner border — 1px black ──────────────────────────────────────
            using (var path = RoundedRect(new Rectangle(3, 3, w - 6, h - 6), Math.Max(1, Radius - 3)))
            using (var pen  = new Pen(Color.Black, 1f))
                g.DrawPath(pen, path);

            // ── Outer border — 1px themed colour (matches button border) ──────
            using (var path = RoundedRect(new Rectangle(1, 1, w - 2, h - 2), Radius - 1))
            using (var pen  = new Pen(_borderColor, 1f))
                g.DrawPath(pen, path);

            // ── Tick marks — black, 25 / 50 / 75% ────────────────────────────
            using (var tickPen = new Pen(Color.Black, 1.5f))
            {
                DrawTick(g, tickPen, inner, 25, major: false);
                DrawTick(g, tickPen, inner, 50, major: true);
                DrawTick(g, tickPen, inner, 75, major: false);
            }

            // ── % label — black, 12pt, between 50% and 75% ticks ─────────────
            string pctText = _value + "%";
            using (var f  = new Font("Arial", 12f, FontStyle.Bold))
            using (var sf = (StringFormat)StringFormat.GenericTypographic.Clone())
            {
                sf.Alignment     = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;

                float y75  = inner.Bottom - inner.Height * 75f / 100f;
                float y50  = inner.Bottom - inner.Height * 50f / 100f;
                float midY = (y75 + y50) / 2f;

                SizeF  sz = g.MeasureString(pctText, f, PointF.Empty, sf);
                float  tx = Math.Max(4f, (w - sz.Width) / 2f);
                float  ty = midY - sz.Height / 2f;

                using (var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                    g.DrawString(pctText, f, shadow, tx + 1, ty + 1, sf);
                using (var tb = new SolidBrush(Color.Black))
                    g.DrawString(pctText, f, tb, tx, ty, sf);
            }
        }

        private static void DrawTick(Graphics g, Pen pen, Rectangle r, int pct, bool major)
        {
            int   len = major ? 7 : 4;
            float y   = r.Bottom - r.Height * pct / 100f;
            y = Math.Max(r.Top, Math.Min(r.Bottom, y));
            g.DrawLine(pen, r.Left,  y, r.Left  + len, y);
            g.DrawLine(pen, r.Right, y, r.Right - len,  y);
        }

        private static GraphicsPath RoundedRect(Rectangle b, int radius)
        {
            int d = Math.Min(radius * 2, Math.Min(b.Width, b.Height));
            var path = new GraphicsPath();
            path.AddArc(b.Left,      b.Top,        d, d, 180, 90);
            path.AddArc(b.Right - d, b.Top,        d, d, 270, 90);
            path.AddArc(b.Right - d, b.Bottom - d, d, d,   0, 90);
            path.AddArc(b.Left,      b.Bottom - d, d, d,  90, 90);
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
