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
        private TcpClient sock = null;
        private StreamReader reader;
        private LogWrite logwrite;
        private NetworkStream writeStream;
        private Finesse finesseObj;

        public ISPSReceiver(StreamReader reader)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
        }

        public ISPSReceiver(TcpClient sock, Finesse finesseObj)
        {
            this.sock = sock;
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            this.reader = new StreamReader(writeStream, encode);
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
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
