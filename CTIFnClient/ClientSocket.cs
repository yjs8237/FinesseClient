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
using VO;

namespace TCPSOCKET
{
    class ClientSocket
    {

        protected TcpClient sock = null;
        //protected Socket sock = null;

        protected NetworkStream writeStream;
        protected StreamReader reader;
        protected StreamWriter writer;

        protected ServerInfo serverInfo;    // 서버 정보를 담고 있는 객체
        protected LogWrite logwrite = null;

        private bool isDisconnectReq;

        private bool isSocketConnected;

        protected Hashtable currentServer;

        protected ClientSocket(LogWrite logwrite)
        {
            this.logwrite = logwrite;
            this.isDisconnectReq = false;
            this.currentServer = new Hashtable();
        }

        public string getCurrentServerIP()
        {
            return (string)currentServer["IP"];
        }
        public int getCurrentServerPort()
        {
            return (int)currentServer["PORT"];
        }

        public void setDisconnectReq(bool isDisconnectReq)
        {
            this.isDisconnectReq = isDisconnectReq;
        }
        public bool getDisconnectReq()
        {
            return this.isDisconnectReq;
        }

        public void setServerInfo(ServerInfo serverInfo)
        {
            this.serverInfo = serverInfo;
        }

        protected int connect(String ip , int port)
        {
            try
            {
                isSocketConnected = false;
                
                sock = new TcpClient();

                IAsyncResult result = sock.BeginConnect(ip, port, null, null);

                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(CONNECTION.CONNECTION_TIMEOUT), true);

                if (success)
                {
                    writeStream = sock.GetStream();

                    //writeStream.ReadTimeout = 3000;

                    writer = new StreamWriter(writeStream);

                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    reader = new StreamReader(writeStream, encode);

                    if (currentServer.ContainsKey("IP"))
                    {
                        currentServer.Remove("IP");
                    }
                    if (currentServer.ContainsKey("PORT"))
                    {
                        currentServer.Remove("PORT");
                    }

                    currentServer.Add("IP", ip);
                    currentServer.Add("PORT", port);

                }
                else
                {
                    return ERRORCODE.SOCKET_CONNECTION_FAIL;
                }
                
                /*
                IAsyncResult result = sock.BeginConnect(ip, port, null, null);

                // TCP Socket Connect Timeout 구현
                int time = 0;
                while (!isSocketConnected && time < CONNECTION.CONNECTION_TIMEOUT)
                {
                    Thread.Sleep(100);
                    time += 100;
                }

                if (!isSocketConnected)
                {
                    logwrite.write("connect", "[" + ip + "][" + port + "] Connection Timeout " + CONNECTION.CONNECTION_TIMEOUT + " Fail !!");
                    return ERRORCODE.SOCKET_CONNECTION_FAIL;
                }
                else
                {
                    writeStream = sock.GetStream();

                    //writeStream.ReadTimeout = 3000;

                    writer = new StreamWriter(writeStream);

                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    reader = new StreamReader(writeStream, encode);
                }
                */

            }
            catch (Exception e)
            {
                logwrite.write("connect", e.ToString());

                if (sock != null)
                {
                    sock = null;
                }

                return ERRORCODE.SOCKET_CONNECTION_FAIL;
            }



            return ERRORCODE.SUCCESS;
        }


        public int disconnect()
        {
            if (sock != null)
            {
                isDisconnectReq = true;
                sock.Close();
                sock = null;
            }
            return ERRORCODE.SUCCESS;
        }

        public int sessionClose()
        {
            logwrite.write("sessionClose", "TCP Session Closed!!!");
            if (sock != null)
            {
                sock.Close();
                sock = null;
            }
            return ERRORCODE.SUCCESS;
        }

        public bool isConnected()
        {
            if (sock != null && sock.Connected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ServerInfo getServerInfo()
        {
            return this.serverInfo;
        }

    }
}
