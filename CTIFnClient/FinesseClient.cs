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
using HTTP;
using XML;
using EVENTOBJ;


namespace TCPSOCKET
{
    class FinesseClient : ClientSocket
    {

        private ArrayList ipArrList;
        private Finesse finesseObj;
        private ISocketReceiver finesseRecv;
        //private ISocketSender finesseSend;
        private Agent agent;
        private HttpHandler httpHandler;
        private Hashtable finesseCurrent;  // 현재 접속되어 있는 Finesse 서버 정보를 관리 하기 위함

        private bool isAlreadyAuth;     // 이벤트를 받기위해 XMPP 인증 절차 여부, XMPP 세션이 끊어지지 않으면, XMPP 인증은 한번만 받아야 한다.

        public FinesseClient(LogWrite logwrite , Finesse finesseObj) : base(logwrite)
        {
            this.finesseObj = finesseObj;
            this.finesseCurrent = new Hashtable();
            this.isAlreadyAuth = false;
        }

        public int reConnect()
        {
            if (sock != null && sock.Connected)
            {
                logwrite.write("reConnect", "Finesse Already Connected !!");
                return ERRORCODE.FAIL;
            }
            return finesseConnect();
        }

        public int finesseConnect()
        {

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
                    finesseObj.setFinesseConnected(true);   // 접속 여부 flag 재접속 할때 Flag 참조한다

                    finesseCurrent.Add("IP", serverIP);
                    finesseCurrent.Add("PORT", serverInfo.getPort());

                    break;
                }
                else
                {
                    finesseObj.setFinesseConnected(false); // 접속 여부 flag 재접속 할때 Flag 참조한다
                    bisConnected = false;
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

        public  int startClient()
        {

            // 이미 소켓이 연결되어 있는지 체크
            if (isConnected())
            {
                logwrite.write("startClient", "Finesse Already Connected !!");
                return ERRORCODE.SUCCESS;
            }

            return finesseConnect();
            
        }

        public  int logout()
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.logoutRequest((string)finesseCurrent["IP"], agent); 
        }

        public  int login(Agent agent)
        {
            this.agent = agent;

            // 소켓이 연결되고 사전 인증절차가 끝나면 스레드를 시작한다.
            // 소켓이 연결되면 서버로 부터 패킷을 받는 스레드 시작 (Call Event Handle)

            // 로그인 시도전 XMPP 사전 인증 절차를 거친다.

            if (!isAlreadyAuth)
            {
                if (startPreProcess() != ERRORCODE.SUCCESS)
                {
                    return ERRORCODE.LOGIN_FAIL;
                }
            }

            isAlreadyAuth = true; // XMPP 인증 완료 여부 flag

            finesseRecv = new FinesseReceiver(sock, finesseObj , agent, this);
            ThreadStart recvts = new ThreadStart(finesseRecv.runThread);
            Thread recvThread = new Thread(recvts);
            recvThread.Start();

            
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            Event evt = null;
            string agentState = "";
            string agentReasonCode = "";
            // 로그인 하기전에 상담원 상태 체크를 먼저한다.
            string agentStateXml = httpHandler.checkAgentState((string)finesseCurrent["IP"], agent);
            if (agentStateXml != null)
            {
                XMLParser xmlParser = new XMLParser(logwrite , agent);

                agentStateXml = agentStateXml.Replace("\n", "");

                agentState = xmlParser.getData(agentStateXml, "state");
                agentReasonCode = xmlParser.getData(agentStateXml, "code");

                logwrite.write("login", "CURRENT AGENT STATE : " + agentState + " , REASON CODE : " + agentReasonCode);

                /*
                int stateStartIndex = agentState.IndexOf("<state>");
                int stateEndIndex = agentState.IndexOf("</state>");
                if (stateStartIndex > 0 && stateEndIndex > 0)
                {
                    tempStr = agentState.Substring(0, stateEndIndex);
                    int stateLen = tempStr.Length - stateStartIndex - "<state>".Length;
                    int tempInt = stateStartIndex + 7;
                    tempStr = tempStr.Substring(tempInt, stateLen);
                    logwrite.write("login", "CURRENT AGENT STATE : " + tempStr);
                   
                }
                 * */
            }


            int returnCode = httpHandler.loginRequest((string)finesseCurrent["IP"], agent);
            if (returnCode == ERRORCODE.SUCCESS)
            {
                if (!agentState.Equals(AGENTSTATE.LOGOUT))
                {
                    evt = new Event();
                    evt.setEvtMsg(agentStateXml);
                    evt.setAgentState(agentState);
                    evt.setReasonCode(agentReasonCode);
                    evt.setEvtCode(EVENT_TYPE.ON_AGENTSTATE_CHANGE);
                    finesseObj.raiseEvent(evt);
                }
                return returnCode;
            }
            else
            {
                return returnCode;
            }

            
            /*
            // 소켓이 연결되면 서버로 패킷을 보내는 스레드 시작
            finesseSend = new FinesseSender(writer, finesseObj);
            ThreadStart sendts = new ThreadStart(finesseSend.runThread);
            Thread sendThread = new Thread(sendts);
            sendThread.Start();

            logwrite.write("login", "Finesse Thread Start!!");
            */
        }

        public int ccTransfer(string dialNumber , string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            return httpHandler.ccTransferRequest((string)finesseCurrent["IP"], agent, dialNumber, dialogID);
        }

        public int makeCall(string dialNumber)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.makeCallRequest((string)finesseCurrent["IP"], agent, dialNumber);
        }

        public int answer(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.answerRequest((string)finesseCurrent["IP"], agent, dialogID);
        }

        public int hold(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.holdRequest((string)finesseCurrent["IP"], agent, dialogID);
        }

        public int retrieve(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.retrieveRequest((string)finesseCurrent["IP"], agent, dialogID);
        }



        public int release(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.releaseRequest((string)finesseCurrent["IP"], agent, dialogID);
        }


        public string getReasonCodeList()
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            return httpHandler.reasonCodeRequest((string)finesseCurrent["IP"], agent);
        }

        public int setCallData(string varName, string varValue, string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.setCalldataRequest((string)finesseCurrent["IP"], agent, varName, varValue, dialogID);
        }
        public int agentState(string state)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.agentStateChangeRequest((string)finesseCurrent["IP"], agent, state);
        }

        public int agentState(string state, string reasonCode)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.agentStateChangeRequest((string)finesseCurrent["IP"], agent, state, reasonCode);
        }


        private int startPreProcess()
        {

            try
            {

                UTIL util = new UTIL();
                string strID = "insungUCDev";
                Random random = new Random();
                int ranNum = random.Next(1, 10);

                string strMsg = @"<?xml version='1.0' ?><stream:stream to='" + (string)finesseCurrent["IP"] + "' xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>";
                send(strMsg);
                recv();
                recv();

                strMsg = @"<auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='PLAIN' xmlns:ga='http://www.google.com/talk/protocol/auth' ga:client-uses-full-bind-result='true'>" + util.AuthBase64_IDAndPw(agent.getAgentID(), agent.getAgentPwd()) + "</auth>";
                send(strMsg);
                recv();

                strMsg = @"<stream:stream to='" + (string)finesseCurrent["IP"] + "' xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>";
                send(strMsg);
                recv();

                strMsg = @"<iq type='set' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'><resource>isi</resource></bind></iq>";
                send(strMsg);
                recv();
                ranNum++;

                strMsg = @"<iq type='set' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><session xmlns='urn:ietf:params:xml:ns:xmpp-session'/></iq>";
                send(strMsg);
                recv();
                ranNum++;

                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='" + serverInfo.getDomain() + "'><query xmlns='http://jabber.org/protocol/disco#items'/></iq>";
                send(strMsg);
                recv();
                ranNum++;


                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='" + serverInfo.getDomain() + "'><query xmlns='http://jabber.org/protocol/disco#info'/></iq>";
                send(strMsg);
                recv();
                ranNum++;


                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><vCard xmlns='vcard-temp'/></iq>";
                strMsg += @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><query xmlns='jabber:iq:roster'/></iq>";
                strMsg += @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='" + serverInfo.getDomain() + "'><query xmlns='http://jabber.org/protocol/disco#items' node='http://jabber.org/protocol/commands'/></iq>";
                strMsg += @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='proxy.eu.jabber.org'><query xmlns='http://jabber.org/protocol/bytestreams'/></iq>";
                send(strMsg);
                recv();
                recv();
                ranNum++;

                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='proxy." + serverInfo.getDomain() + "'><query xmlns='http://jabber.org/protocol/bytestreams'/></iq>";
                send(strMsg);
                recv();
                ranNum++;

                strMsg = @"<presence><priority>1</priority><c xmlns='http://jabber.org/protocol/caps' node='http://pidgin.im/' hash='sha-1' ver='I22W7CegORwdbnu0ZiQwGpxr0Go='/><x xmlns='vcard-temp:x:update'><photo/></x></presence>";
                strMsg += @"<iq type='set' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><pubsub xmlns='http://jabber.org/protocol/pubsub'><publish node='http://jabber.org/protocol/tune'><item><tune xmlns='http://jabber.org/protocol/tune'/></item></publish></pubsub></iq>";
                send(strMsg);
                recv();
                ranNum++;
            }
            catch (Exception e)
            {
                logwrite.write("startPreProcess", e.ToString());
                return ERRORCODE.FAIL;
            }

            return ERRORCODE.SUCCESS;
        }


        private void send(String msg)
        {

            if (sock == null || !sock.Connected)
            {
                if (reConnect() == ERRORCODE.SUCCESS)
                {
                    logwrite.write("send", msg);
                    writer.WriteLine(msg);
                    writer.Flush();
                }
            }
            else
            {
                logwrite.write("send", msg);
                writer.WriteLine(msg);
                writer.Flush();
            }
        }

        private void recv()
        {

            //int BUFFERSIZE = sock.ReceiveBufferSize;
            byte[] buffer = new byte[32768];
            //int bytes = writeStream.Read(buffer, 0, buffer.Length);

            int read;


            read = writeStream.Read(buffer, 0, buffer.Length);
            if (read > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, read);
                logwrite.write("recv", message);
            }
            else 
            {
                logwrite.write("recv", "return bytes size -> " + read);    
            }
            
            /*
            if (bytes > 0)
            {
              
            }
            else
            {
                logwrite.write("recv", "return bytes size -> " + bytes);    
            }
             * */
            
        }
      
    }
}
