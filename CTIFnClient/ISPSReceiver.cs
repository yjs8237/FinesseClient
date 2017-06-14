using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using CTIFnClient;


namespace ThreadGroup
{
    class ISPSReceiver : ISocketReceiver
    {

        private StreamReader reader;
        private LogWrite logwrite;

        public ISPSReceiver(StreamReader reader)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
        }


        public void runThread()
        {
            try
            {
                String readLine = "";

                while (true)
                {
                    readLine = reader.ReadLine();
                    logwrite.write("ISPSReceiver runThread", readLine);
                }

            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                logwrite.write("ISPSReceiver runThread", e.StackTrace);
            }
        }
    }
}
