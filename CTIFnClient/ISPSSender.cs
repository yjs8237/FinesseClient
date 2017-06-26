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
namespace ThreadGroup
{
    class ISPSSender : ISocketSender
    {
        
        private LogWrite logwrite;
        private TcpClient sock = null;
        private ISPSClient ispsClient;

        public ISPSSender(LogWrite logwrite, ISPSClient ispsClient)
        {
            this.logwrite = logwrite;
            this.ispsClient = ispsClient;
        }


        public void runThread()
        {
            ArrayList ipList = new ArrayList();

            ServerInfo serverInfo = ispsClient.getServerInfo();

            ipList = serverInfo.getIPList();
            int port = serverInfo.getPort();

            bool connectSuccess = false;

            while (!connectSuccess)
            {
                for (int i = 0; i < ipList.Count; i++)
                {
                    string ip = (string)ipList[0];

                    sock = new TcpClient();

                    var result = sock.BeginConnect(ip, port, null, null);

                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(CONNECTION.CONNECTION_TIMEOUT));
                    
                        if (sock != null && sock.Connected)
                        {
                            connectSuccess = true;
                            logwrite.write("ISPS Sender", "Connection SUCCESS IP [" + ip + "] PORT [" + port + "]");
                            break;
                        }
                        else
                        {
                            logwrite.write("ISPS Sender", "Connection Fail IP [" + ip + "] PORT [" + port + "]");
                        }

                }

            }

            if (sock != null)
            {
                sock.Close();
            }

            ispsClient.reConnect();
           
        }

    }
}
