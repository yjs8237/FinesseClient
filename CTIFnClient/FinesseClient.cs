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
using VO;
using CONST;

namespace TCPSOCKET
{
    class FinesseClient : ClientSocket
    {

        private ArrayList ipArrList;
        private Finesse finesseObj;
        private ISocketReceiver finesseRecv;
        private ISocketSender finesseSend;
        private Agent agent;


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

                    writer = new StreamWriter(writeStream);

                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    reader = new StreamReader(writeStream, encode);
                    
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

        public override int login(Agent agent)
        {
            this.agent = agent;

            // 로그인 시도전 XMPP 사전 인증 절차를 거친다.
            if (startPreProcess() != ERRORCODE.SUCCESS)
            {
                return ERRORCODE.LOGIN_FAIL;
            }

            // 소켓이 연결되고 사전 인증절차가 끝나면 스레드를 시작한다.
            // 소켓이 연결되면 서버로 부터 패킷을 받는 스레드 시작 (Call Event Handle)
            finesseRecv = new FinesseReceiver(reader, finesseObj);
            ThreadStart recvts = new ThreadStart(finesseRecv.runThread);
            Thread recvThread = new Thread(recvts);
            recvThread.Start();


            // 소켓이 연결되면 서버로 패킷을 보내는 스레드 시작
            finesseSend = new FinesseSender(writer, finesseObj);
            ThreadStart sendts = new ThreadStart(finesseSend.runThread);
            Thread sendThread = new Thread(sendts);
            sendThread.Start();


            return ERRORCODE.SUCCESS;
        }

        private int startPreProcess()
        {
            String returnMsg;

            logwrite.write("startPreProcess", "1. HELLO MESSAGE SEND ");
            /*
            CString strMsg = "<?xml version='1.0' ?>"\
			"<stream:stream to=\'insunginfo.co.kr\' xmlns=\'jabber:client\' "\
			"xmlns:stream=\'http://etherx.jabber.org/streams\' version=\'1.0\'>";
            */
            String strMsg = @"<?xml version='1.0' ?> <stream:stream to='insunginfo.co.kr' xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>";
            logwrite.write("startPreProcess", strMsg);
            writer.WriteLine(strMsg);
            writer.Flush();

            returnMsg = reader.ReadLine();
            logwrite.write("startPreProcess", "RETURN [" + returnMsg + "]");


            logwrite.write("startPreProcess", "2. AUTH MESSAGE SEND ");
            /*
            strMsg.Format("<auth xmlns=\'urn:ietf:params:xml:ns:xmpp-sasl\' "\
		    " mechanism='PLAIN' xmlns:ga=\'http://www.google.com/talk/protocol/auth\' "\
		    " ga:client-uses-full-bind-result=\'true\'>"\
		    " %s</auth>", INSUNG::Base64::AuthBase64_IDAndPw(szID, szPW));
             * */

            UTIL util = new UTIL();
            strMsg = @"<auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='PLAIN' xmlns:ga='http://www.google.com/talk/protocol/auth' ga:client-uses-full-bind-result='true'>";
            strMsg += util.AuthBase64_IDAndPw(agent.getAgentID(), agent.getAgentPwd()) + "</auth>";

            logwrite.write("startPreProcess", strMsg);
            writer.WriteLine(strMsg);
            writer.Flush();

            returnMsg = reader.ReadLine();
            logwrite.write("startPreProcess", "RETURN [" + returnMsg + "]");

            return ERRORCODE.SUCCESS;
        }

      
    }
}
