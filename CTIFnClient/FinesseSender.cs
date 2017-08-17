using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using CTIFnClient;
using TCPSOCKET;
using System.Collections;
using CONST;
using VO;

namespace ThreadGroup 
{
    class FinesseSender : ISocketSender
    {

        private LogWrite logwrite;
        private TcpClient sock = null;
        private FinesseClient finesseClient;

        private NetworkStream writeStream;
        private StreamReader reader;
        private StreamWriter writer;

        private Agent agent;

        public FinesseSender(TcpClient sock , Agent agent , FinesseClient finesseClient)
        {
            this.sock = sock;
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            writer = new StreamWriter(writeStream);
            this.finesseClient = finesseClient;
            this.logwrite = LogWrite.getInstance();
            this.agent = agent;
        }


        public FinesseSender(LogWrite logwrite, FinesseClient finesseClient)
        {
            this.finesseClient = finesseClient;
            this.logwrite = LogWrite.getInstance();
        }



        public void pingCheck()
        {

        }

        public void runThread()
        {
            
            ArrayList ipList = new ArrayList();

            ServerInfo serverInfo = finesseClient.getServerInfo();

            ipList = serverInfo.getIPList();
            int port = serverInfo.getPort();

            bool connectSuccess = false;

            while (!connectSuccess)
            {
                for (int i = 0; i < ipList.Count; i++)
                {
                    string ip = (string)ipList[i];

                    sock = new TcpClient();

                    var result = sock.BeginConnect(ip, port, null, null);

                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(CONNECTION.CONNECTION_TIMEOUT));
                    
                        if (sock != null && sock.Connected)
                        {
                            connectSuccess = true;
                            logwrite.write("Finesse Sender", "Connection SUCCESS IP [" + ip + "] PORT [" + port + "]");
                            break;
                        }
                        else
                        {
                            logwrite.write("Finesse Sender", "Connection Fail IP [" + ip + "] PORT [" + port + "]");
                        }
                }
            }

            if (sock != null)
            {
                sock.Close();
            }

            finesseClient.finesseReConnect();
            
        }


    }
}
