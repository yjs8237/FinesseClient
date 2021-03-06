﻿using System;
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
                
                sock = new TcpClient();

                IAsyncResult result = sock.BeginConnect(ip, port, null, null);

                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(CONNECTION.CONNECTION_TIMEOUT), true);

                if (success)
                {
                    writeStream = sock.GetStream();

                    //writeStream.ReadTimeout = 3000;

                    writer = new StreamWriter(writeStream);

                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");

                    //Encoding encode = System.Text.Encoding.Default;

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

        public int send(string sendMsg)
        {
            if (sendMsg == null)
            {
                return ERRORCODE.FAIL;
            }

            try
            {
                if (writer == null)
                {
                    return ERRORCODE.FAIL;
                }

                writer.WriteLine(sendMsg);
                writer.Flush();
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                }
                return ERRORCODE.FAIL;
            }
            finally
            {
            }
            return ERRORCODE.SUCCESS;
        }

        public string recv()
        {

            sock.ReceiveTimeout = 3000;

            string recvMsg = null;
            try
            {
                /*
                int BUFFERSIZE = sock.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int bytelen = 0;

                StringBuilder sb = new StringBuilder();

                bytelen = writeStream.Read(buffer, 0, buffer.Length);
                recvMsg = Encoding.UTF8.GetString(buffer, 0, bytelen);
                */
                recvMsg = reader.ReadLine();
               
            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (sock != null)
                {
                    sock.Close();
                }
            }
            return recvMsg;
        }

        public int disconnect()
        {
            if (sock != null)
            {
                setDisconnectReq(true);
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
