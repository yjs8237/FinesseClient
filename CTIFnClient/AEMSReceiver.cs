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
    class AEMSReceiver : ISocketReceiver
    {
        private StreamReader reader;
        private LogWrite logwrite;
        private TcpClient sock = null;
        private Finesse finesseObj;
        private NetworkStream writeStream;

        public AEMSReceiver(StreamReader reader, Finesse finesseObj)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;
        }

        public AEMSReceiver(TcpClient sock, Finesse finesseObj)
        {
            this.sock = sock;
            this.finesseObj = finesseObj;
            this.logwrite = LogWrite.getInstance();
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            this.reader = new StreamReader(writeStream, encode);
        }

        public void runThread()
        {
            try
            {
                /*
                String readLine = "";

                while (true)
                {
                    readLine = reader.ReadLine();
                    logwrite.write("AEMSReceiver runThread", readLine);
                }
                 * */

                int BUFFERSIZE = sock.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int bytelen = 0;
                while ((bytelen = writeStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytelen);
                    logwrite.write("recv", message);
                }

            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                logwrite.write("AEMSReceiver runThread", e.StackTrace);
            }
        }
    }
}
