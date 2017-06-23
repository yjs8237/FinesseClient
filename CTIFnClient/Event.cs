using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVENTOBJ
{
    public interface IEvent
    {

        void setEvtCode(int evtCode);
        int getEvtCode();
        void setEvtType(string evtType);
        string getEvtType();
        void setEvtMsg(string evtMsg);
        string getEvtMsg();
        void setAgentState(string agentState);
        string getAgentState();
        void setReasonCode(string reasonCode);
        string getReasonCode();
        void setDialogID(string dialogID);
        string getDialogID();
        void setCallType(string callType);
        string getCallType();
        void setCallState(string callState);
        string getCallState();


    }
}
