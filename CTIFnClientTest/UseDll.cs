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

            Console.WriteLine(evt);
        }

        public override void GetEventOnError(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnAgentStateChange(string state , string reasonCode , String evt)
        {
            if (state.Equals(BTNMASK.NOT_READY))
            {
                string[] arr = { BTNMASK.LOGOUT, BTNMASK.READY , BTNMASK.REASON , BTNMASK.MAKECALL  };
                form.setButtonMask(arr);
            }
            else if (state.Equals(BTNMASK.READY))
            {
                string[] arr = { BTNMASK.REASON };
                form.setButtonMask(arr);
            }
            else if (state.Equals(BTNMASK.LOGOUT))
            {
                string[] arr = { BTNMASK.DISCONNECT , BTNMASK.LOGIN };
                form.setButtonMask(arr);
            }

            Console.WriteLine(evt);
        }

        public override void GetEventOnCallAlerting(String evt)
        {
            string[] arr = { BTNMASK.ANSWER};
            form.setButtonMask(arr);
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallWrapup(String evt)
        {
            string[] arr = { BTNMASK.READY , BTNMASK.REASON };
            form.setButtonMask(arr);
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallActive(String evt)
        {
            string[] arr = { BTNMASK.RELEASE };
            form.setButtonMask(arr);
            Console.WriteLine(evt);
        }

        public override void GetEventOnDisConnection(string finesseIP , string aemsIP , string ispsIP , String evt)
        {
            string[] arr = { BTNMASK.CONNECTION };
            form.setButtonMask(arr);

            Console.WriteLine("1. finesseIP : " + finesseIP + " , aemsIP : " + aemsIP + " , ispsIP : " + ispsIP);

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

            Console.WriteLine("2. finesseIP : " + finesseIP + " , aemsIP : " + aemsIP + " , ispsIP : " + ispsIP);

            form.setServerInfo(finesseIP, aemsIP, ispsIP);

            Console.WriteLine(evt);
        }
    }
}
