using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CTIFnClient;


namespace CTIFnClientTest
{
    class UseDll : Finesse
    {
        private Form1 form;
        public UseDll(Form1 form)
        {
            this.form = form;
        }

        public override void GetEventOnConnection(string finesseip , string aemsip , string ispsip , String evt)
        {
            string[] arr = { BTNMASK.LOGIN, BTNMASK.DISCONNECT };
            form.setButtonMask(arr);
            form.setServerInfo(finesseip, aemsip, ispsip);

           // Console.WriteLine(evt);
        }


        public override void GetEventOnAgentStateChange(string state, string reasonCode, string evtMessage)
        {
            if (state.Equals(BTNMASK.NOT_READY))
            {
                string[] arr = { BTNMASK.LOGOUT, BTNMASK.READY, BTNMASK.REASON, BTNMASK.MAKECALL };
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
        }
      

        public override void GetEventOnCallAlerting(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            string[] arr = { BTNMASK.ANSWER };
            form.setButtonMask(arr);
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

            form.setServerInfo(finesseIP, aemsIP, ispsIP);

            //Console.WriteLine(evt);
        }


        public override void GetEventOnCallEstablished(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            /*
            string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
            form.setButtonMask(arr);
             * */
        }


        public override void GetEventOnCallWrapUp(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            /*
           string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
           form.setButtonMask(arr);
            * */
        }

        public override void GetEventOnCallError(string errorMessage)
        {
            
        }

        public override void GetEventOnCallDropped(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            /*
         string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
         form.setButtonMask(arr);
          * */
        }

        public override void GetEventOnCallHeld(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            /*
     string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
     form.setButtonMask(arr);
      * */
        }

        public override void GetEventOnCallInitiating(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            /*
     string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
     form.setButtonMask(arr);
      * */
        }

        public override void GetEventOnCallInitiated(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            /*
     string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
     form.setButtonMask(arr);
      * */
        }

        public override void GetEventOnCallFailed(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03)
        {
            /*
    string[] arr = { BTNMASK.RELEASE, BTNMASK.TRANSFER, BTNMASK.HOLD };
    form.setButtonMask(arr);
     * */
        }
    }
}
