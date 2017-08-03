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

        //private string callState;

        private Hashtable callStateTable;   // 각 콜 주체의 콜 state 체크
        private ArrayList actionList;       // Action List

        public CallEvent()
        {
            callStateTable = new Hashtable();
            actionList = new ArrayList();
        }

        public void setCallState(string number , string state)
        {
            if (callStateTable.ContainsKey(number))
            {
                callStateTable.Remove(number);
            }
            callStateTable.Add(number, state);
        }
        public Hashtable getCallStateTable()
        {
            return this.callStateTable;
        }

        public void setActionList (ArrayList actionList)
        {
            this.actionList = actionList;
        }
        public ArrayList getActionList()
        {
            return actionList;
        }


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
