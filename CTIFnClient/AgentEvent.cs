using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVENTOBJ
{
    class AgentEvent : Event
    {
        private string agentState;
        private string reasonCode;
        private bool isFirstLogin;

        public void setIsFirstLogin(bool isFirstLogin)
        {
            this.isFirstLogin = isFirstLogin;
        }
        public bool getIsFirstLogin()
        {
            return isFirstLogin;
        }
        public void setAgentState(string agentState)
        {
            this.agentState = agentState;
        }
        public string getAgentState()
        {
            return this.agentState;
        }
        public void setReasonCode(string reasonCode)
        {
            this.reasonCode = reasonCode;
        }
        public string getReasonCode()
        {
            return this.reasonCode;
        }
    }
}
