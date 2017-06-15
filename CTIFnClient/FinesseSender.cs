using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using CTIFnClient;
using System.Collections;
using ThreadGroup;
using CONST;
using System.Net;


namespace ThreadGroup 
{
    class FinesseSender : ISocketSender
    {
        private StreamWriter writer;
        private LogWrite logwrite;
        private Finesse finesseObj;

        public FinesseSender(StreamWriter writer, Finesse finesseObj)
        {
            this.writer = writer;
            this.finesseObj = finesseObj;
            this.logwrite = LogWrite.getInstance();
        }

        public void runThread()
        {

            
           
        }

    }
}
