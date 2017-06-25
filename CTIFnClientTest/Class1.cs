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

        public override void GetEventOnConnection(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnConnectionClosed(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallBegin(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallDelivered(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallEstablished(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallHeld(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallRetrieved(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallConnectionCleared(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnLoginFail(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnPasswordChecked(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnConnectionFail(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnError(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnAgentStateChange(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallAlerting(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallWrapup(String evt)
        {
            Console.WriteLine(evt);
        }

        public override void GetEventOnCallActive(String evt)
        {
            Console.WriteLine(evt);
        }
    }
}
