using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVENTOBJ
{
    interface IEvent
    {
        void setEventCode(int evtCode);
        int getEventCode();
         void setEventType(string evtType);
         string getEventType();
         void setEventMsg(string evtMsg);
         string getEventMsg();
    }

    class EVENT
    {
        public const int OnError = -100;

        public const int OnConnection = 100;
        public const int OnConnectionClosed = 200;
        public const int OnCallBegin = 300;
        public const int OnCallDlivered = 400;
        public const int OnCallEstablished = 500;
        public const int OnCallHeld = 600;
        public const int OnCallRetrieved = 700;
        public const int OnCallConnectionCleared = 800;
        public const int OnLoginFail = 900;
        public const int OnPasswordCheked = 1000;
        public const int OnConnectionFail = 1100;

        public const int OnAgentStateChange = 1200;



    }
}
