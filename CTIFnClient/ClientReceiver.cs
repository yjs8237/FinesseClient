using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using CTIFnClient;

namespace ThreadGroup
{
    class ClientReceiver
    {

        private StreamReader reader;
        private LogWrite logwrite;

        public ClientReceiver(StreamReader reader)
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
                    logwrite.write("ClientReceiver runThread", readLine);
                }
            }
            catch (Exception e)
            {
                logwrite.write("ClientReceiver runThread", e.StackTrace);
            }

        }

    }
}
