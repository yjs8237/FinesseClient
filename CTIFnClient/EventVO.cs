using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVENTOBJ
{
    public class Evt : IEvent
    {

        private int evtCode;
        private string evtType;
        private string evtMsg;
        private string dialogID;
        private string agentState;


        public void setEventCode(int evtCode)
        {
            this.evtCode = evtCode;
        }
        public int getEventCode()
        {
            return this.evtCode;
        }

        public void setEventType(string evtType)
        {
            this.evtType = evtType;
        }

        public string getEventType()
        {
            return this.evtType;
        }

        public void setEventMsg(string evtMsg)
        {
            this.evtMsg = evtMsg;
        }

        public string getEventMsg()
        {
            return this.evtMsg;
        }

        public void setDialogID(string dialogID)
        {
            this.dialogID = dialogID;
        }

        public string getDialogID()
        {
            return this.dialogID;
        }

        public void setAgentState(string agentState)
        {
            this.agentState = agentState;
        }

        public string getAgentState()
        {
            return this.agentState;
        }
      
    }
}
