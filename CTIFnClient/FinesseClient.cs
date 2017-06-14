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

namespace TCPSOCKET
{
    class FinesseClient : ClientSocket
    {

        private ArrayList ipArrList;
        private Finesse finesseObj;


        public FinesseClient(LogWrite logwrite , Finesse finesseObj) : base(logwrite)
        {
            this.finesseObj = finesseObj;
        }

        public override int startClient()
        {

            // 이미 소켓이 연결되어 있는지 체크
            if (isConnected())
            {
                logwrite.write("startClient", "Finesse Already Connected !!");
                return ERRORCODE.SUCCESS;
            }


            // 서버의 IP ArrayList 를 가져온다. serverInfo 객체는 부모 클래스에 존재
            ipArrList = serverInfo.getIPList();

            Random ran = new Random();
            // Finesse 서버 접속은 랜덤으로 접속을 시도한다.
            int randomServer = ran.Next(0, ipArrList.Count);

            Boolean bisConnected = false;

            // IP 리스트 중에서 Finesse 랜덤으로 접속한다.
            for (int i = 0; i < ipArrList.Count; i++)
            {
                String serverIP = (String)ipArrList[randomServer];

                logwrite.write("startClient", "Finesse Try Connection [" + serverIP + "][" + serverInfo.getPort() + "]");

                // 서버 접속 성공할 경우 서버로부터 패킷받는 스레드 구동 시작
                if (connect(serverIP, serverInfo.getPort()) == ERRORCODE.SUCCESS)
                {
                    logwrite.write("startClient", "Finesse Connection SUCCESS!! [" + serverIP + "][" + serverInfo.getPort() + "]");
                    bisConnected = true;

                    writeStream = sock.GetStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    reader = new StreamReader(writeStream, encode);

                    // 소켓이 연결되면 서버로 부터 패킷을 받는 스레드 시작 (Call Event Handle)
                    ISocketReceiver finesseRecv = new FinesseReceiver(reader , finesseObj);
                    ThreadStart ts = new ThreadStart(finesseRecv.runThread);
                    Thread thread = new Thread(ts);
                    thread.Start();

                    logwrite.write("startClient", "Finesse Thread Start!!");

                    break;
                }

                if (randomServer >= ipArrList.Count - 1)
                {
                    randomServer = 0;
                }
                else
                {
                    randomServer++;
                }

            }

            return bisConnected ? ERRORCODE.SUCCESS : ERRORCODE.SOCKET_CONNECTION_FAIL;
            
        }
    }
}
