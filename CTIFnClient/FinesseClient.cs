using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTIFnClient;
using System.Collections;

namespace TCPSOCKET
{
    class FinesseClient : ClientSocket
    {

        private ArrayList ipArrList;

        public FinesseClient(LogWrite logwrite) : base(logwrite)
        {
               
        }

        public override int startClient()
        {
            
            // 서버의 IP ArrayList 를 가져온다. serverInfo 객체는 부모 클래스에 존재
            ipArrList = serverInfo.getIPList();

            Random ran = new Random();
            // Finesse 서버 접속은 랜덤으로 접속을 시도한다.
            int randomServer = ran.Next(0, ipArrList.Count);

            Boolean isConnected = false;

            // IP 리스트 중에서 Finesse 랜덤으로 접속한다.
            for (int i = 0; i < ipArrList.Count; i++)
            {
                String serverIP = (String)ipArrList[randomServer];

                logwrite.write("startClient", "Try Connection [" + serverIP + "][" + serverInfo.getPort() + "]");
                if (connect(serverIP, serverInfo.getPort()) == ERRORCODE.SUCCESS)
                {
                    logwrite.write("startClient", "Connection SUCCESS!! [" + serverIP + "][" + serverInfo.getPort() + "]");
                    isConnected = true;
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

            return isConnected ? ERRORCODE.SUCCESS : ERRORCODE.SOCKET_CONNECTION_FAIL;
            
        }
    }
}
