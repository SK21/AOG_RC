using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Controls
{
    public partial class CKbox : UserControl
    {
        private Color Fcolor;

        public CKbox()
        {
            InitializeComponent();
            checkBox1.Dock = DockStyle.Fill;
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            Fcolor = checkBox1.ForeColor;
        }

        public Appearance Appearance
        {
            get { return checkBox1.Appearance; }
            set { checkBox1.Appearance = value; }
        }

        public ContentAlignment CheckAlign
        {
            get { return checkBox1.CheckAlign; }
            set { checkBox1.CheckAlign = value; }
        }

        public CheckBox CheckBox
        { get { return checkBox1; } }

        public bool Checked
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }
        }

        public ContentAlignment TextAlign
        {
            get { return checkBox1.TextAlign; }
            set { checkBox1.TextAlign = value; }
        }

        public string Txt
        {
            get { return checkBox1.Text; }
            set { checkBox1.Text = value; }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Fcolor = checkBox1.ForeColor;
                checkBox1.ForeColor = checkBox1.BackColor;
            }
            else
            {
                checkBox1.ForeColor = Fcolor;
            }
        }
    }
}