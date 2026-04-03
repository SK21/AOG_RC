using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RateController.Classes
{
    public class RoundedButton : Button
    {
        public int CornerRadius { get; set; } = 8;

        private bool _pressed = false;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // clear corners with parent background to avoid artifacts on theme change
            e.Graphics.Clear(Parent?.BackColor ?? BackColor);

            // inset by 1px so the border isn't clipped at the control edge
            Rectangle bounds = Rectangle.Inflate(ClientRectangle, -1, -1);

            Color fillTop = _pressed ? Darken(BackColor, 30) : Lighten(BackColor, 60);
            Color fillBottom = _pressed ? Darken(BackColor, 50) : Darken(BackColor, 30);

            using (GraphicsPath path = RoundedRect(bounds, CornerRadius))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    bounds, fillTop, fillBottom, LinearGradientMode.Vertical))
                    e.Graphics.FillPath(brush, path);

                using (Pen pen = new Pen(ForeColor, 1))
                    e.Graphics.DrawPath(pen, path);
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            _pressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            _pressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnForeColorChanged(System.EventArgs e)
        {
            base.OnForeColorChanged(e);
            Invalidate();
        }

        protected override void OnBackColorChanged(System.EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }

        protected override void OnParentBackColorChanged(System.EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            Invalidate();
        }

        private static Color Lighten(Color c, int amount)
        {
            return Color.FromArgb(
                Math.Min(255, c.R + amount),
                Math.Min(255, c.G + amount),
                Math.Min(255, c.B + amount));
        }

        private static Color Darken(Color c, int amount)
        {
            return Color.FromArgb(
                Math.Max(0, c.R - amount),
                Math.Max(0, c.G - amount),
                Math.Max(0, c.B - amount));
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
