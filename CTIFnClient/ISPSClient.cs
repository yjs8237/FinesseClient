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
using EVENTOBJ;

namespace TCPSOCKET
{
    class ISPSClient : ClientSocket
    {
         private ArrayList ipArrList;
         private Finesse finesseObj;

         public ISPSClient(LogWrite logwrite ,  Finesse finesseObj) : base(logwrite)
        {
            this.finesseObj = finesseObj;
        }

         public int reConnect()
         {
             if (sock != null && sock.Connected)
             {
                 logwrite.write("reConnect", "ISPS Already Connected !!");
                 return ERRORCODE.FAIL;
             }
             return ispsConnect();
         }
         public  int ispsConnect()
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
                     logwrite.write("startClient", "ISPS Connection SUCCESS!! [" + serverIP + "][" + serverInfo.getPort() + "]");

                     bisConnected = true;
                     finesseObj.setISPSConnected(true); // 접속 여부 flag 재접속 할때 Flag 참조한다

                     writeStream = sock.GetStream();
                     Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                     reader = new StreamReader(writeStream, encode);

                     /*
                     // 소켓이 연결되면 서버로 부터 패킷을 받는 스레드 시작
                     ISocketReceiver ispsRecv = new ISPSReceiver(sock, finesseObj, this);
                     ThreadStart ts = new ThreadStart(ispsRecv.runThread);
                     Thread thread = new Thread(ts);
                     thread.Start();
                     */

                     callConnectionEvent();


                     logwrite.write("startClient", "ISPS Thread Start!!");

                     break;
                 }
                 else
                 {
                     finesseObj.setISPSConnected(false);  // 접속 여부 flag 재접속 할때 Flag 참조한다
                     bisConnected = false;
                 }

             }
             return bisConnected ? ERRORCODE.SUCCESS : ERRORCODE.SOCKET_CONNECTION_FAIL;
         }

         public void callConnectionEvent()
         {
             ErrorEvent evt = new ErrorEvent();
             evt.setServerType("03");   // ISPS Server Code : 03
             evt.setEvtCode(EVENT_TYPE.ON_CONNECTION);
             evt.setCurIspsIP((string)currentServer["IP"]);
             evt.setEvtMsg("ISPS Connection Success!!");
             finesseObj.raiseEvent(evt);
         }

    }
}
