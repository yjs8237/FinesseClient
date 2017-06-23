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
        private LogWrite logwrite;
        public Form1()
        {
            InitializeComponent();
            useDll = new UseDll();
            logwrite = LogWrite.getInstance();
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
            
            int ret = useDll.fnConnect(finesse_A, finesse_B, finesseDomain, AEMS_A, AEMS_B, AEMS_Port, ISPS_A, ISPS_B, ISPS_Port, loglevel);
            logwrite.write("", "RETURN DATA : " + ret);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            logwrite.write("", "RETURN DATA : " +  useDll.fnDisconnect());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 로그인버튼 클릭
            string agentID = textBox9.Text;    // agentID
            string agentPwd = textBox10.Text;   // agentPwd
            string extension = textBox11.Text;   // extension
           
            logwrite.write("", "RETURN DATA : " +  useDll.fnLogin(agentID, agentPwd, extension, "5000"));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnLogout());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string dialNumber = textBox14.Text;    // dialNumber
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnMakeCall(dialNumber));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAnswer());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAgentState("READY"));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string reasoncode = textBox15.Text;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAgentState("NOT_READY", reasoncode));
        }

        private void button9_Click(object sender, EventArgs e)
        {
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnRelease());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAgentState("NOT_READY"));
        }

    }
}
