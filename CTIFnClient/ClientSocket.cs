using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using CTIFnClient;
using System.Collections;
using ThreadGroup;
using System.Threading;



namespace TCPSOCKET
{
    abstract class ClientSocket
    {

        private TcpClient sock = null;
        
        private NetworkStream writeStream;
        private StreamReader reader;

        protected ServerInfo serverInfo;
        protected LogWrite logwrite = null;


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
                sock = new TcpClient();
                sock.Connect(ip, port);

                writeStream = sock.GetStream();
                
                Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                reader = new StreamReader(writeStream, encode);

                // 소켓이 연결되면 서버로 부터 패킷을 받는 스레드 시작
                ClientReceiver receiver = new ClientReceiver(reader);
                ThreadStart ts = new ThreadStart(receiver.runThread);
                Thread thread = new Thread(ts);
                thread.Start();




            }
            catch (Exception e)
            {
                logwrite.write("connect", e.StackTrace);
                return ERRORCODE.SOCKET_CONNECTION_FAIL;
            }

            return ERRORCODE.SUCCESS;
        }


        // Finesse , AEMS , ISPS 접속 방식을 자식 클래스에게 위임
        public abstract int startClient();
       

    }
}
