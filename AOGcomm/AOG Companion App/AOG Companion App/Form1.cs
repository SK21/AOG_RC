using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOG_Companion_App
{
    public partial class Form1 : Form
    {
        private UDPcomm UDPobj;

        byte[] Data = new byte[10];

        public Form1()
        {
            InitializeComponent();
            UDPobj = new UDPcomm(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbReceivePort.Text = "8888";
            tbSendPort.Text = "2388";
        }

        private void tbReceive_TextChanged(object sender, EventArgs e)
        {
            tbReceive.SelectionStart = tbReceive.Text.Length;
            tbReceive.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data[0] = 121;
            Data[1] = 224;
            UDPobj.SendMessage(Data);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Data[0] = 125;
            Data[1] = 200;
            UDPobj.SendMessage(Data);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Data[0] = 130;
            Data[1] = 220;
            UDPobj.SendMessage(Data);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UDPobj.Start(tbReceivePort.Text, tbSendPort.Text);
        }
    }
}
