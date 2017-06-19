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
    abstract class ClientSocket
    {

        protected TcpClient sock = null;
        //protected Socket sock = null;


        protected NetworkStream writeStream;
        protected StreamReader reader;
        protected StreamWriter writer;

        protected ServerInfo serverInfo;    // 서버 정보를 담고 있는 객체
        protected LogWrite logwrite = null;

        private static bool isSocketConnected = false;


        protected string connSvrIP;

        protected ClientSocket(LogWrite logwrite)
        {
            this.logwrite = logwrite;
        }

        public void setServerInfo(ServerInfo serverInfo)
        {
            this.serverInfo = serverInfo;
        }

        protected int connect(String ip , int port)
        {
            try
            {
                /*
                sock = new TcpClient();
                sock.Connect(ip, port);

                var result = sock.BeginConnect(ip, port, null, null);

                // 3초의 Connection Timeout 설정
                bool success = result.AsyncWaitHandle.WaitOne(CONNECTION.CONNECTION_TIMEOUT, true);

                if (success)
                {
                    // TCP/IP Connection 성공
                }
                else
                {
                    logwrite.write("connect", "[" + ip + "][" + port + "] Connection Timeout " + CONNECTION.CONNECTION_TIMEOUT + " Fail !!");
                    return ERRORCODE.SOCKET_CONNECTION_FAIL;
                }
                */

                /*
                IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse(ip) , port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                IAsyncResult result = socket.BeginConnect(ipEndpoint, null, null);

                if (result.AsyncWaitHandle.WaitOne(CONNECTION.CONNECTION_TIMEOUT, true))
                {

                }
                else
                {
                    logwrite.write("connect", "[" + ip + "][" + port + "] Connection Timeout " + CONNECTION.CONNECTION_TIMEOUT + " Fail !!");
                    return ERRORCODE.SOCKET_CONNECTION_FAIL;
                }
                */
                
                sock = new TcpClient();

                var result =  sock.BeginConnect(ip, port, null, null);

                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(CONNECTION.CONNECTION_TIMEOUT));

                if (success)
                {

                }
                else
                {
                    return ERRORCODE.SOCKET_CONNECTION_FAIL;
                }
                
                /*
                IAsyncResult result = sock.BeginConnect(ip, port, connect_callback, sock);

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
                */

            }
            catch (Exception e)
            {
                logwrite.write("connect", e.StackTrace);

                if (sock != null)
                {
                    sock = null;
                }

                return ERRORCODE.SOCKET_CONNECTION_FAIL;
            }

            return ERRORCODE.SUCCESS;
        }

        // TCP Connection Async Callback 함수
        private static void connect_callback(IAsyncResult result)
        {
            try
            {
                Socket socket = (Socket) result.AsyncState;
                isSocketConnected = socket.Connected;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                isSocketConnected = false;
            }
        }


        public int disconnect()
        {
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

        // Finesse , AEMS , ISPS 접속 방식을 자식 클래스에게 위임
        public abstract int startClient();
        public abstract int login(Agent agent);

    }
}
