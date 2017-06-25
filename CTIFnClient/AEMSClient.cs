using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CTIFnClient;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using ThreadGroup;
using CONST;
using VO;
namespace TCPSOCKET
{
    class AEMSClient : ClientSocket
    {

        private ArrayList ipArrList;
        private Finesse finesseObj;

        public AEMSClient(LogWrite logwrite ,  Finesse finesseObj)
            : base(logwrite)
        {
            this.finesseObj = finesseObj;
        }

        public int reConnect()
        {
            if (sock != null && sock.Connected)
            {
                logwrite.write("reConnect", "AEMS Already Connected !!");
                return ERRORCODE.FAIL;
            }
            return aemsConnect();
        }

        public  int aemsConnect()
        {

            // 이미 소켓이 연결되어 있는지 체크
            if (isConnected())
            {
                logwrite.write("startClient", "AEMS Already Connected !!");
                return ERRORCODE.SUCCESS;
            }

            Boolean bisConnected = false;

            // 서버의 IP ArrayList 를 가져온다. serverInfo 객체는 부모 클래스에 존재
            ipArrList = serverInfo.getIPList();


            // IP 리스트 중에서 A Side를 먼저 바라본다.
            for (int i = 0; i < ipArrList.Count; i++)
            {

                String serverIP = (String)ipArrList[i];

                logwrite.write("startClient", "AEMS Try Connection [" + serverIP + "][" + serverInfo.getPort() + "]");
                if (connect(serverIP, serverInfo.getPort()) == ERRORCODE.SUCCESS)
                {
                    logwrite.write("startClient", "AEMS Connection SUCCESS!! [" + serverIP + "][" + serverInfo.getPort() + "]");


                    bisConnected = true;
                    finesseObj.setAEMSConnected(true); // 접속 여부 flag 재접속 할때 Flag 참조한다

                    writeStream = sock.GetStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    reader = new StreamReader(writeStream, encode);

                    // 소켓이 연결되면 서버로 부터 패킷을 받는 스레드 시작
                    ISocketReceiver aemsRecv = new AEMSReceiver(sock , finesseObj , this);
                    ThreadStart ts = new ThreadStart(aemsRecv.runThread);
                    Thread thread = new Thread(ts);
                    thread.Start();

                    logwrite.write("startClient", "AEMS Thread Start!!");

                    break;
                }
                else
                {
                    finesseObj.setAEMSConnected(false); // 접속 여부 flag 재접속 할때 Flag 참조한다
                    bisConnected = false;
                }

            }

            return bisConnected ? ERRORCODE.SUCCESS : ERRORCODE.SOCKET_CONNECTION_FAIL;
            
        }

        public  int login(Agent agent)
        {
            throw new NotImplementedException();
        }

        public  int logout()
        {
            throw new NotImplementedException();
        }

    }
}
