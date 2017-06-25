using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace EVENTOBJ
{
    public class Event 
    {
        private string evtCode;
        private string evtType;
        private string evtMsg;

        private string dialogID;
        private string callType;
        private string agentState;
        private string reasonCode;

        private string callState;

        private Hashtable callVarTable;


        public void setCallVariable(Hashtable table)
        {
            this.callVarTable = table;
        }
        public Hashtable getCallVariable()
        {
            return this.callVarTable;
        }

        public void setCallState(string callState)
        {
            this.callState = callState;
        }
        public string getCallState()
        {
            return this.callState;
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

        public void setDialogID(string dialogID)
        {
            this.dialogID = dialogID;
        }
        public string getDialogID()
        {
            return dialogID;
        }
        public void setCallType(string callType)
        {
            this.callType = callType;
        }
        public string getCallType()
        {
            return this.callType;
        }


        public void setEvtCode(string evtCode)
        {
            this.evtCode = evtCode;
        }

        public string getEvtCode()
        {
            return this.evtCode;
        }

        public void setEvtType(string evtType)
        {
            this.evtType = evtType;
        }

        public string getEvtType()
        {
            return evtType;
        }

        public void setEvtMsg(string evtMsg)
        {
            this.evtMsg = evtMsg;
        }

        public string getEvtMsg()
        {
            return this.evtMsg;
        }
    }
}
