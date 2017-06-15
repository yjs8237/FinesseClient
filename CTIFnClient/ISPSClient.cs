using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using ThreadGroup;
using CTIFnClient;
using CONST;
using VO;

namespace TCPSOCKET
{
    class ISPSClient : ClientSocket
    {
         private ArrayList ipArrList;

         public ISPSClient(LogWrite logwrite) : base(logwrite)
        {
               
        }
         public override int startClient()
         {

             // 이미 소켓이 연결되어 있는지 체크
             if (isConnected())
             {
                 logwrite.write("startClient", "ISPS Already Connected !!");
                 return ERRORCODE.SUCCESS;
             }

             Boolean bisConnected = false;

             // 서버의 IP ArrayList 를 가져온다. serverInfo 객체는 부모 클래스에 존재
             ipArrList = serverInfo.getIPList();

             // IP 리스트 중에서 A Side를 먼저 바라본다.
             for (int i = 0; i < ipArrList.Count; i++)
             {

                 String serverIP = (String)ipArrList[i];

                 logwrite.write("startClient", "ISPS Try Connection [" + serverIP + "][" + serverInfo.getPort() + "]");
                 if (connect(serverIP, serverInfo.getPort()) == ERRORCODE.SUCCESS)
                 {
                     bisConnected = true;
                     logwrite.write("startClient", "ISPS Connection SUCCESS!! [" + serverIP + "][" + serverInfo.getPort() + "]");

                     writeStream = sock.GetStream();
                     Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                     reader = new StreamReader(writeStream, encode);

                     // 소켓이 연결되면 서버로 부터 패킷을 받는 스레드 시작
                     ISocketReceiver aemsRecv = new AEMSReceiver(reader);
                     ThreadStart ts = new ThreadStart(aemsRecv.runThread);
                     Thread thread = new Thread(ts);
                     thread.Start();

                     logwrite.write("startClient", "ISPS Thread Start!!");

                     break;
                 }
             }
             return bisConnected ? ERRORCODE.SUCCESS : ERRORCODE.SOCKET_CONNECTION_FAIL;
         }

         public override int login(Agent agent)
         {
             throw new NotImplementedException();
         }
    }
}
