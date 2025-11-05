using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    public class frmTextInput : Form
    {
        private readonly TextBox tb;
        private readonly Button btnOk;
        private readonly Button btnCancel;
        public string EnteredText { get; private set; }

        public frmTextInput(string prompt, string title = "Input")
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new Size(420, 140);

            var lb = new Label
            {
                AutoSize = false,
                Text = prompt ?? "",
                Left = 12,
                Top = 12,
                Width = 396,
                Height = 32
            };

            tb = new TextBox
            {
                Left = 12,
                Top = 50,
                Width = 396
            };

            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 228,
                Top = 92,
                Width = 84
            };
            btnOk.Click += (s, e) => { EnteredText = tb.Text; };

            btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 324,
                Top = 92,
                Width = 84
            };

            this.Controls.Add(lb);
            this.Controls.Add(tb);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            tb.Focus();
            tb.SelectAll();
        }
    }
}
