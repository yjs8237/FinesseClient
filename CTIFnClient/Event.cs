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

        private string currentFinesseIP;
        private string currentAemsIP;
        private string currentIspsIP;
       

        public void setCurFinesseIP(string ip)
        {
            this.currentFinesseIP = ip;
        }
        public string getCurFinesseIP()
        {
            return this.currentFinesseIP;
        }
        public void setCurAemsIP(string ip)
        {
            this.currentAemsIP = ip;
        }
        public string getCurAemsIP()
        {
            return this.currentAemsIP;
        }
        public void setCurIspsIP(string ip)
        {
            this.currentIspsIP = ip;
        }
        public string getCurIspsIP()
        {
            return this.currentIspsIP;
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
