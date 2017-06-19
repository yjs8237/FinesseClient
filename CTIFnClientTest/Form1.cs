using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CTIFnClient;


namespace CTIFnClientTest
{
    public partial class Form1 : Form
    {
        private UseDll useDll;
        public Form1()
        {
            InitializeComponent();
            useDll = new UseDll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String finesse_A = textBox1.Text;
            String finesse_B = textBox2.Text;
            String finesseDomain = textBox13.Text;
            String AEMS_A = textBox3.Text;
            String AEMS_B = textBox4.Text;
            int AEMS_Port = Int32.Parse(textBox5.Text);
            String ISPS_A = textBox8.Text;
            String ISPS_B = textBox7.Text;
            int ISPS_Port = Int32.Parse(textBox6.Text);
            int loglevel = Int32.Parse(textBox12.Text);
            
            useDll.fnConnect(finesse_A, finesse_B, finesseDomain, AEMS_A, AEMS_B, AEMS_Port, ISPS_A, ISPS_B, ISPS_Port, loglevel);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            useDll.fnDisconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 로그인버튼 클릭
            string agentID = textBox9.Text;    // agentID
            string agentPwd = textBox10.Text;   // agentPwd
            string extension = textBox11.Text;   // extension

            useDll.fnLogin(agentID, agentPwd, extension, "5000");
        }

    }
}
