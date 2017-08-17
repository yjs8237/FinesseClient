using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CTIFnClient;
using System.Collections;

namespace CTIFnClientTest
{
    class UseDll : Finesse
    {
        private Form1 form;
        private LogWrite logwrite;

        public UseDll(Form1 form)
        {
            this.form = form;
            this.logwrite = LogWrite.getInstance();
        }

        public override void GetEventOnConnection(string finesseip , string aemsip , string ispsip , String evt)
        {
            if (finesseip != null && finesseip.Length > 0)
            {
                string[] arr = { BTNMASK.LOGIN, BTNMASK.DISCONNECT };
                form.setButtonMask(arr);
            }
            form.setServerInfo(finesseip, aemsip, ispsip);
           // Console.WriteLine(evt);
        }

        public override void GetEventOnAgentStateChange(string state, string reasonCode, string evtMessage)
        {
            if (state.Equals(BTNMASK.NOT_READY))
            {
                string[] arr = { BTNMASK.LOGOUT, BTNMASK.READY, BTNMASK.REASON, BTNMASK.MAKE_CALL };
                form.setButtonMask(arr);
            }
            else if (state.Equals(BTNMASK.READY))
            {
                string[] arr = { BTNMASK.REASON };
                form.setButtonMask(arr);
            }
            else if (state.Equals(BTNMASK.LOGOUT))
            {
                string[] arr = { BTNMASK.DISCONNECT, BTNMASK.LOGIN };
                form.setButtonMask(arr);
            }
            else if (state.Equals(BTNMASK.WORK_READY) || state.Equals(BTNMASK.WORK))
            {
                string[] arr = { BTNMASK.REASON, BTNMASK.READY };
                form.setButtonMask(arr);
            }
            else if (state.Equals(BTNMASK.HOLD))
            {
                string[] arr = { BTNMASK.RETRIEVE};
                form.setButtonMask(arr);
            }
            else if (state.Equals(BTNMASK.TALKING))
            {
                string[] arr = { BTNMASK.DROP };
                form.setButtonMask(arr);
            }
        }

        public override void GetEventOnCallAlerting(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
            if (actionList != null && actionList.Length != 0)
            {
                char[] delimiterChars = { '^' };
                string[] arr = actionList.Split(delimiterChars);
                form.setButtonMask(arr);
            }
            else
            {
                string[] arr = { BTNMASK.ANSWER };
                form.setButtonMask(arr);
            }

            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);


            
        }

        public override void GetEventOnDisConnection(string finesseIP , string aemsIP , string ispsIP , String evt)
        {
            string[] arr = { BTNMASK.CONNECTION };
            form.setButtonMask(arr);

            //Console.WriteLine("1. finesseIP : " + finesseIP + " , aemsIP : " + aemsIP + " , ispsIP : " + ispsIP);

            if (finesseIP != null && finesseIP.Length > 0)
            {
                finesseIP = "0.0.0.0";
            }
            if (aemsIP != null && aemsIP.Length > 0)
            {
                aemsIP = "0.0.0.0";
            }
            if (ispsIP != null && ispsIP.Length > 0)
            {
                ispsIP = "0.0.0.0";
            }

            //Console.WriteLine("2. finesseIP : " + finesseIP + " , aemsIP : " + aemsIP + " , ispsIP : " + ispsIP);

            form.setDisconnectServerInfo(finesseIP, aemsIP, ispsIP);

            //Console.WriteLine(evt);
        }

        public override void GetEventOnCallWrapUp(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
            string[] arr = { BTNMASK.REASON, BTNMASK.READY };
            form.setButtonMask(arr);
            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);
        }

        public override void GetEventOnCallError(string errorMessage)
        {
            
        }

        public override void GetEventOnCallDropped(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
         /*
         string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
         form.setButtonMask(arr);
         */
            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);
        }

        public override void GetEventOnCallHeld(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
            /*
            if (actionList != null && actionList.Length != 0)
            {
                actionList = actionList.Replace("CONSULT_CALL", "CCTRANSFER^CCCONFERENCE");
                char[] delimiterChars = { '^' };
                string[] arr = actionList.Split(delimiterChars);
                form.setButtonMask(arr);
            }
            */
            /*
            string[] arr = { BTNMASK.RETRIEVE };
            form.setButtonMask(arr);
           */
            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);
        }

        public override void GetEventOnCallInitiating(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
            /*
     string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
     form.setButtonMask(arr);
      * */
            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);
        }

        public override void GetEventOnCallInitiated(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
            if (actionList != null && actionList.Length != 0)
            {
                char[] delimiterChars = { '^' };
                string[] arr = actionList.Split(delimiterChars);
                form.setButtonMask(arr);
            }
            else
            {
                string[] arr = { BTNMASK.DROP };
                form.setButtonMask(arr);
            }
            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);
        }

        public override void GetEventOnCallFailed(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
            /*
    string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
    form.setButtonMask(arr);
     * */
            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);
        }

        public override void GetEventOnPassCheck(string ret, string data)
        {
            /*
   string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
   form.setButtonMask(arr);
    * */
        }

        public override void GetEventOnCallActive(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList)
        {
            /*
            if (actionList != null && actionList.Length != 0)
            {
                actionList = actionList.Replace("CONSULT_CALL", "CCTRANSFER^CCCONFERENCE");
                char[] delimiterChars = { '^' };
                string[] arr = actionList.Split(delimiterChars);
                form.setButtonMask(arr);
            }
         */

            if (form.isTransfer)
            {
                // 호전환 시도하여 컨설트콜이 Active 상태가 되었을경우
                string[] arr = { BTNMASK.RECONNECT, BTNMASK.TRANSFER };
                form.setButtonMask(arr);
            }
            else if (form.isConference)
            {
                // 3자통화 시도하여 컨설트콜이 Active 상태가 되었을경우
                string[] arr = { BTNMASK.RECONNECT, BTNMASK.CONFERENCE };
                form.setButtonMask(arr);
            }
            else
            {
                // 일반 아웃바운드 콜 (내선말고)
                string[] arr = { BTNMASK.DROP, BTNMASK.CCTRANSFER, BTNMASK.HOLD, BTNMASK.CCCONFERENCE, BTNMASK.TRANSFER_SST };
                form.setButtonMask(arr);
            }
            logwrite.write("USEDLL", "callState : " + callState);
            logwrite.write("USEDLL", "actionList : " + actionList);

            /*
            if (callType.Equals("OUT"))
            {
                if (form.isTransfer)
                {
                    // 호전환 시도하여 컨설트콜이 Active 상태가 되었을경우
                    string[] arr = { BTNMASK.RECONNECT, BTNMASK.TRANSFER };
                    form.setButtonMask(arr);
                }
                else if (form.isConference)
                {
                    // 3자통화 시도하여 컨설트콜이 Active 상태가 되었을경우
                    string[] arr = { BTNMASK.RECONNECT, BTNMASK.CONFERENCE };
                    form.setButtonMask(arr);
                }
                else
                {
                    // 일반 아웃바운드 콜 (내선말고)
                    string[] arr = { BTNMASK.DROP, BTNMASK.CCTRANSFER, BTNMASK.HOLD, BTNMASK.CCCONFERENCE , BTNMASK.TRANSFER_SST };
                    form.setButtonMask(arr);
                }
            }
            else if (callType.Equals("CONSULT") && form.isConference)
            {
                // 3자통화 완료하였을 경우
                string[] arr = { BTNMASK.RECONNECT, BTNMASK.TRANSFER };
                form.setButtonMask(arr);
            }
            else
            {
                string[] arr = { BTNMASK.DROP, BTNMASK.CCTRANSFER, BTNMASK.HOLD, BTNMASK.CCCONFERENCE, BTNMASK.TRANSFER_SST };
                form.setButtonMask(arr);
            } 
             * */
            
        }

        public override void GetEventOnAgentLoggedOn(string state, string reasonCode, string evtMessage)
        {
                string[] arr = { BTNMASK.LOGOUT, BTNMASK.READY, BTNMASK.REASON, BTNMASK.MAKE_CALL };
                form.setButtonMask(arr);
        }
    }
}
