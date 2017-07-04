using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace EVENTOBJ
{
    class CallEvent : Event
    {
        private string fromAddress;
        private string toAddress;

        private string dialogID;
        private string callType;

        private Hashtable callVarTable;

        private string callState;

        public void setFromAddress(string fromAddress)
        {
            this.fromAddress = fromAddress;
        }
        public string getFromAddress()
        {
            return this.fromAddress;
        }

        public void setToAddress(string toAddress)
        {
            this.toAddress = toAddress;
        }
        public string getToAddress()
        {
            return this.toAddress;
        }

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


    }
}
