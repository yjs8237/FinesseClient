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
using System.Collections;

namespace CTIFnClientTest
{
    public partial class Form1 : Form
    {
        private UseDll useDll;
        private LogWrite logwrite;

        private Hashtable buttonTable;


        public bool isTransfer;
        public bool isConference;

        public Form1()
        {
            InitializeComponent();
            useDll = new UseDll(this);
            logwrite = LogWrite.getInstance();
            buttonTable = new Hashtable();
            initialButtonMask();
            CheckForIllegalCrossThreadCalls = false;
        }


        private void setConsultCallSetting()
        {
            isTransfer = false;
            isConference = false;
        }

        private void initialButtonMask()
        {
            buttonTable.Add(BTNMASK.CONNECTION, button1);
            buttonTable.Add(BTNMASK.DISCONNECT, button2);
            buttonTable.Add(BTNMASK.LOGIN, button3);
            buttonTable.Add(BTNMASK.LOGOUT, button4);
            buttonTable.Add(BTNMASK.READY, button7);
            buttonTable.Add(BTNMASK.NOT_READY, button10);
            buttonTable.Add(BTNMASK.REASON, button8);
            buttonTable.Add(BTNMASK.MAKE_CALL, button5);
            buttonTable.Add(BTNMASK.CCTRANSFER, button11);
            buttonTable.Add(BTNMASK.ANSWER, button6);
            buttonTable.Add(BTNMASK.DROP, button9);
            buttonTable.Add(BTNMASK.HOLD, button13);
            buttonTable.Add(BTNMASK.RETRIEVE, button14);
            buttonTable.Add(BTNMASK.TRANSFER, button16);
            buttonTable.Add(BTNMASK.CCCONFERENCE, button17);
            buttonTable.Add(BTNMASK.CONFERENCE, button18);
            buttonTable.Add(BTNMASK.RECONNECT, button19);
            buttonTable.Add(BTNMASK.TRANSFER_SST, button24);
            
            setInitialButton();

        }

        public void setInitialButton()
        {
            /*
            foreach (DictionaryEntry item in buttonTable)
            {
                Button button = (Button)item.Value;
                if (item.Key.Equals(BTNMASK.CONNECTION))
                {
                    button.Enabled = true;
                }
                else
                {
                    button.Enabled = false ;
                }

            }
             * */
        }

        public void setButtonMask(string []buttonMask)
        {
            
            foreach (DictionaryEntry item in buttonTable)
            {
                string buttonKey = (string)item.Key;
                Button button = (Button)item.Value;
                if (buttonMask.Contains(buttonKey))
                {
                    button.Enabled = true;
                }
                else
                {
                    button.Enabled = false;
                }
            }
            
        }

        public void setServerInfo(string finesseip, string aemsip, string ispsip)
        {
            if (finesseip != null && finesseip.Length > 0)
            {
                label14.Text = finesseip;
            }
            if (aemsip != null && aemsip.Length > 0)
            {
                label15.Text = aemsip;
            }
            if (ispsip != null && ispsip.Length > 0)
            {
                label17.Text = ispsip;
            }
        }
        public void setDisconnectServerInfo(string finesseip, string aemsip, string ispsip)
        {
            if (finesseip != null && finesseip.Length > 0)
            {
                label14.Text = "0.0.0.0";
            }
            if (aemsip != null && aemsip.Length > 0)
            {
                label15.Text = "0.0.0.0";
            }
            if (ispsip != null && ispsip.Length > 0)
            {
                label17.Text = "0.0.0.0";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();

            String finesse_A = textBox1.Text;
            String finesse_B = textBox2.Text;
            
            String AEMS_A = textBox3.Text;
            String AEMS_B = textBox4.Text;
            int AEMS_Port = Int32.Parse(textBox5.Text);
            String ISPS_A = textBox8.Text;
            String ISPS_B = textBox7.Text;
            int ISPS_Port = Int32.Parse(textBox6.Text);
            int loglevel = Int32.Parse(textBox12.Text);
            
            int ret = useDll.fnConnect(finesse_A, finesse_B, AEMS_A, AEMS_B, AEMS_Port, ISPS_A, ISPS_B, ISPS_Port, loglevel);
            logwrite.write("", "RETURN DATA : " + ret);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "RETURN DATA : " +  useDll.fnDisconnect());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();

            // 로그인버튼 클릭
            string agentID = textBox9.Text;    // agentID
            string agentPwd = textBox10.Text;   // agentPwd
            string extension = textBox11.Text;   // extension
           
            logwrite.write("", "RETURN DATA : " +  useDll.fnLogin(agentID, agentPwd, extension, "5000"));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnLogout());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string dialNumber = textBox14.Text;    // dialNumber
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnMakeCall(dialNumber));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAnswer());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAgentState("READY"));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string reasoncode = textBox15.Text;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAgentState("NOT_READY", reasoncode));
        }

        private void button9_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnRelease());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnAgentState("NOT_READY"));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string dialNumber = textBox14.Text;    // dialNumber
            isTransfer = true;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnCCTransfer(dialNumber));
        }

        private void button12_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string varname = textBox16.Text;    // 변수명
            string varvalue = textBox17.Text;    // 데이터
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnSetCallData(varname, varvalue));
        }

        private void button13_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnHold());
        }

        private void button14_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnRetrieve());
        }

        private void button15_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnGetReasonCodeList());
        }

        private void button16_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnTransfer());
        }

        private void button17_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string dialNumber = textBox14.Text;    // dialNumber
            isConference = true;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnCCConference(dialNumber));
        }

        private void button18_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnConference());
        }

        private void button19_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnReconnect());
        }

        private void button20_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string account = textBox19.Text; // 계좌번호
            string dialNum = textBox18.Text;    // 폰패드 번호
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnPhonePad(dialNum, "1" , account));
        }

        private void button22_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string dialNum = textBox20.Text;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnArsTransfer(dialNum));
        }

        private void button23_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string ispsData = textBox21.Text;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnSendISPS(ispsData));
        }

        private void button24_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string dialNum = textBox14.Text;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnArsTransfer(dialNum));
        }

        private void button25_Click(object sender, EventArgs e)
        {
            setConsultCallSetting();
            string data = textBox13.Text;
            logwrite.write("", "<------- RETURN DATA -------> : " + useDll.fnSendAEMS(data));
        }

    }
}
