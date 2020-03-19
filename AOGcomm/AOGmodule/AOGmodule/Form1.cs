using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOGmodule
{
    public partial class Form1 : Form
    {
        private UDPcomm UDPobj;

        byte[] Data = new byte[10];

        public Form1()
        {
            InitializeComponent();
            tbServerIP.Text = "192.168.2.255";
            UDPobj = new UDPcomm(this, tbServerIP.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UDPobj.Start();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Data[0] = 121;
            Data[1] = 124;
            UDPobj.SendMessage(Data);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Data[0] = 125;
            Data[1] = 100;
            UDPobj.SendMessage(Data);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Data[0] = 128;
            Data[1] = 232;
            UDPobj.SendMessage(Data);
        }

        private void tbReceive_TextChanged_1(object sender, EventArgs e)
        {
            tbReceive.SelectionStart = tbReceive.Text.Length;
            tbReceive.ScrollToCaret();
        }
    }
}
