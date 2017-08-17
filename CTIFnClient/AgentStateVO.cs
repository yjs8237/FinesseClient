using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VO
{
    class AgentStateVO
    {
        private string state;
        private string reasonCode;
        private string xmppMsg;


        public void setState(string state)
        {
            this.state = state;
        }
        public string getState()
        {
            return this.state;
        }

        public void setReasonCode(string reasonCode)
        {
            this.reasonCode = reasonCode;
        }
        public string getReasonCode()
        {
            return this.reasonCode;
        }

        public void setXmppMsg(string msg)
        {
            this.xmppMsg = msg;
        }
        public string getXmppMsg()
        {
            return this.xmppMsg;
        }

    }
}
