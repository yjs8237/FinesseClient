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
        private Agent agent;
        private HttpHandler httpHandler;

        private bool isAlreadyAuth;     // 이벤트를 받기위해 XMPP 인증 절차 여부, XMPP 세션이 끊어지지 않으면, XMPP 인증은 한번만 받아야 한다.

        public FinesseClient(LogWrite logwrite , Finesse finesseObj) : base(logwrite)
        {
            this.finesseObj = finesseObj;
            this.isAlreadyAuth = false;
        }

        public void setXMPPAuth(bool isAlreadyAuth)
        {
            this.isAlreadyAuth = isAlreadyAuth;
        }


        public int finesseReConnect()
        {
            if (sock != null && sock.Connected)
            {
                logwrite.write("finesseReConnect", "Finesse Already Connected !!");
                return ERRORCODE.FAIL;
            }
            if (finesseConnect() == ERRORCODE.SUCCESS)
            {
                return connectXMPPAuth();
            }
            else
            {
                logwrite.write("finesseReConnect", "Finesse ReConnection FAIL !! ");
                return ERRORCODE.FAIL;
            }
        }

        public int startClient()
        {

            // 이미 소켓이 연결되어 있는지 체크
            if (isConnected())
            {
                logwrite.write("startClient", "Finesse Already Connected !!");
                return ERRORCODE.SUCCESS;
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

            string currentServerIP = (string)currentServer["IP"];
            if (currentServerIP == null)
            {
                currentServerIP = "";
            }

            Boolean bisConnected = false;

            // IP 리스트 중에서 Finesse 랜덤으로 접속한다.
            for (int i = 0; i < ipArrList.Count; i++)
            {
                String serverIP = (String)ipArrList[randomServer];

                if (randomServer >= ipArrList.Count - 1)
                {
                    randomServer = 0;
                }
                else
                {
                    randomServer++;
                }

                if (currentServerIP.Equals(serverIP))
                {
                    // 재접속일 경우 접속되어있던 서버는 건너 뛴다.
                    continue;
                }

                logwrite.write("startClient", "Finesse Try Connection [" + serverIP + "][" + serverInfo.getPort() + "]");

                // 서버 접속 성공할 경우 서버로부터 패킷받는 스레드 구동 시작
                if (connect(serverIP, serverInfo.getPort()) == ERRORCODE.SUCCESS)
                {
                    logwrite.write("startClient", "Finesse Connection SUCCESS!! [" + serverIP + "][" + serverInfo.getPort() + "]");

                    bisConnected = true;
                    finesseObj.setFinesseConnected(true);   // 접속 여부 flag 재접속 할때 Flag 참조한다

                    break;
                }
                else
                {
                    finesseObj.setFinesseConnected(false); // 접속 여부 flag 재접속 할때 Flag 참조한다
                    bisConnected = false;
                }

            }

            return bisConnected ? ERRORCODE.SUCCESS : ERRORCODE.SOCKET_CONNECTION_FAIL;
        }

        public  int logout()
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.logoutRequest((string)currentServer["IP"], agent); 
        }

        public int login()
        {

            // XMPP 인증 시도
            if (connectXMPPAuth() != ERRORCODE.SUCCESS)
            {
                return ERRORCODE.FAIL;
            }

            checkAgentState();  // 이전 상담원 상태체크

            return httpHandler.loginRequest((string)currentServer["IP"], agent);

        }

        public int connectXMPPAuth()
        {
            this.agent = Agent.getInstance();

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
            else
            {
                logwrite.write("connectXMPPAuth", "## Finesse Authentication is Already Success ##");
            }

            logwrite.write("connectXMPPAuth", "## Finesse Authentication Success ##");

            isAlreadyAuth = true; // XMPP 인증 완료 여부 flag

            // Finesse XMPP 이벤트 받는 스레드 시작
            finesseRecv = new FinesseReceiver(sock, finesseObj, agent, this);
            ThreadStart recvts = new ThreadStart(finesseRecv.runThread);
            Thread recvThread = new Thread(recvts);
            recvThread.Start();


            callConnectionEvent();  // Connection 성공 이벤트 전달

            return ERRORCODE.SUCCESS;
        }

        public int checkAgentState()
        {
            // 로그인하기전 서버에 상담원 상태를 먼저 체크한다.

            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            string agentState = "";
            string agentReasonCode = "";
            // 로그인 하기전에 상담원 상태 체크를 먼저한다.
            string agentStateXml = httpHandler.checkAgentState((string)currentServer["IP"], agent);
            if (agentStateXml != null)
            {
                XMLParser xmlParser = new XMLParser(logwrite, agent);

                agentStateXml = agentStateXml.Replace("\n", "");

                agentState = xmlParser.getData(agentStateXml, "state");
                agentReasonCode = xmlParser.getData(agentStateXml, "code");

                logwrite.write("checkAgentState", "CURRENT AGENT STATE : " + agentState + " , REASON CODE : " + agentReasonCode);

                // 서버에 이미 남아 있는 상담원 상태가 로그아웃이 아닐경우에 이벤트를 발생한다.
                if (!agentState.Equals(AGENTSTATE.LOGOUT))
                {
                    AgentEvent evt = new AgentEvent();
                    evt.setEvtMsg(agentStateXml);
                    evt.setAgentState(agentState);
                    evt.setReasonCode(agentReasonCode);
                    evt.setEvtCode(EVENT_TYPE.ON_AGENTSTATE_CHANGE);
                    finesseObj.raiseEvent(evt);
                }

                return ERRORCODE.SUCCESS;
            }
            else
            {
                return ERRORCODE.FAIL;
            }
        }



        private int startPreProcess()
        {
            try
            {
                int tempindex = 0;

                FinesseDomain domain = FinesseDomain.getInstance();

                UTIL util = new UTIL();
                string strID = "insungUCDev";
                Random random = new Random();
                int ranNum = random.Next(1, 10);

                string strMsg = @"<?xml version='1.0' ?><stream:stream to='" + (string)currentServer["IP"] + "' xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }

                strMsg = @"<auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='PLAIN' xmlns:ga='http://www.google.com/talk/protocol/auth' ga:client-uses-full-bind-result='true'>" + util.AuthBase64_IDAndPw(agent.getAgentID(), agent.getAgentPwd()) + "</auth>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }

                strMsg = @"<stream:stream to='" + (string)currentServer["IP"] + "' xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' version='1.0'>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }

                strMsg = @"<iq type='set' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'><resource>isi</resource></bind></iq>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                ranNum++;

                strMsg = @"<iq type='set' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><session xmlns='urn:ietf:params:xml:ns:xmpp-session'/></iq>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                ranNum++;

                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='" + domain.getFinesseDomain() + "'><query xmlns='http://jabber.org/protocol/disco#items'/></iq>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                ranNum++;


                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='" + domain.getFinesseDomain() + "'><query xmlns='http://jabber.org/protocol/disco#info'/></iq>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                ranNum++;


                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><vCard xmlns='vcard-temp'/></iq>";
                strMsg += @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><query xmlns='jabber:iq:roster'/></iq>";
                strMsg += @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='" + domain.getFinesseDomain() + "'><query xmlns='http://jabber.org/protocol/disco#items' node='http://jabber.org/protocol/commands'/></iq>";
                strMsg += @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='proxy.eu.jabber.org'><query xmlns='http://jabber.org/protocol/bytestreams'/></iq>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                ranNum++;

                strMsg = @"<iq type='get' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "' to='proxy." + domain.getFinesseDomain() + "'><query xmlns='http://jabber.org/protocol/bytestreams'/></iq>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
                ranNum++;

                strMsg = @"<presence><priority>1</priority><c xmlns='http://jabber.org/protocol/caps' node='http://pidgin.im/' hash='sha-1' ver='I22W7CegORwdbnu0ZiQwGpxr0Go='/><x xmlns='vcard-temp:x:update'><photo/></x></presence>";
                strMsg += @"<iq type='set' id='" + strID + util.lpad(Convert.ToString(ranNum), "a", 3) + "'><pubsub xmlns='http://jabber.org/protocol/pubsub'><publish node='http://jabber.org/protocol/tune'><item><tune xmlns='http://jabber.org/protocol/tune'/></item></publish></pubsub></iq>";
                send(strMsg);
                if (recv(tempindex++) != ERRORCODE.SUCCESS) { return ERRORCODE.FAIL; }
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
                if (finesseReConnect() == ERRORCODE.SUCCESS)
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

        private int recv(int tempIndex)
        {
            
            //int BUFFERSIZE = sock.ReceiveBufferSize;
            byte[] buffer = new byte[32768];
            //int bytes = writeStream.Read(buffer, 0, buffer.Length);

            writeStream.ReadTimeout = 3000;

            int read;

            try
            {

                read = writeStream.Read(buffer, 0, buffer.Length);
                if (read > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, read);
                    logwrite.write("recv", message);

                    /*
                     *  Finesse 서버의 도메인을 가져오기 위한 로직
                     * */
                    if (message.Contains("stream:stream") && tempIndex == 0)
                    {
                        int startIndex = message.IndexOf("<stream:stream");
                        int messageLen = message.Length;
                        string tempStr = message.Substring(startIndex, messageLen - startIndex);

                        startIndex = tempStr.IndexOf("from=");
                        tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);

                        startIndex = 0;
                        int endIndex = 0;
                        int tempInt = 0;
                        for (int i = 0; i < tempStr.Length; i++)
                        {
                            string str = tempStr.Substring(i, 1);
                            if (str.Equals("\""))
                            {
                                tempInt++;
                                if (tempInt == 1)
                                {
                                    startIndex = i + 1;
                                }
                                else if (tempInt == 2)
                                {
                                    endIndex = i;
                                    break;
                                }

                            }
                        }

                        tempStr = tempStr.Substring(startIndex, endIndex - startIndex);
                        logwrite.write("recv", " ** Finesse Domain ** : [" + tempStr + "]");

                        FinesseDomain domain = FinesseDomain.getInstance();
                        domain.setFinesseDomain(tempStr);

                    }
                }
                else
                {
                    logwrite.write("recv", "return bytes size -> " + read);
                }
            }
            catch (Exception e)
            {
                if (sock != null)
                {
                    sessionClose();
                }
                return ERRORCODE.FAIL;
            }
            return ERRORCODE.SUCCESS;
        }

        public void callConnectionEvent()
        {
            ErrorEvent evt = new ErrorEvent();
            evt.setServerType("01");   // FINESSE Server Code : 03
            evt.setEvtCode(EVENT_TYPE.ON_CONNECTION);
            evt.setCurFinesseIP((string)currentServer["IP"]);
            evt.setEvtMsg("Finesse Connection Success!!");
            finesseObj.raiseEvent(evt);
        }

        public int ccTransfer(string dialNumber , string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            return httpHandler.ccTransferRequest((string)currentServer["IP"], agent, dialNumber, dialogID);
        }

        public int transfer(string dialNumber , string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            return httpHandler.transferRequest((string)currentServer["IP"], agent, dialNumber, dialogID);
        }



        public int ccConference(string dialNumber, string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            return httpHandler.ccConferenceRequest((string)currentServer["IP"], agent, dialNumber, dialogID);
        }

        public int conference(string dialNumber, string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            return httpHandler.conferenceRequest((string)currentServer["IP"], agent, dialNumber, dialogID);
        }

        public int makeCall(string dialNumber)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.makeCallRequest((string)currentServer["IP"], agent, dialNumber);
        }



        public int answer(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.answerRequest((string)currentServer["IP"], agent, dialogID);
        }

        public int hold(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.holdRequest((string)currentServer["IP"], agent, dialogID);
        }

        public int arsTransfer(string dialNumber , string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.arsTransferRequest((string)currentServer["IP"], agent, dialNumber, dialogID);
        }


        public int retrieve(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.retrieveRequest((string)currentServer["IP"], agent, dialogID);
        }

        public int reconnect(string dialogID, string dialogID_second)
        {
            release(dialogID_second);
            return retrieve(dialogID);
        }

        public int release(string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.releaseRequest((string)currentServer["IP"], agent, dialogID);
        }


        public string getReasonCodeList()
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }
            return httpHandler.reasonCodeRequest((string)currentServer["IP"], agent);
        }

        public int setCallData(string varName, string varValue, string dialogID)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.setCalldataRequest((string)currentServer["IP"], agent, varName, varValue, dialogID);
        }
        public int agentState(string state)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.agentStateChangeRequest((string)currentServer["IP"], agent, state);
        }

        public int agentState(string state, string reasonCode)
        {
            if (httpHandler == null)
            {
                httpHandler = new HttpHandler(logwrite);
            }

            return httpHandler.agentStateChangeRequest((string)currentServer["IP"], agent, state, reasonCode);
        }

      
    }
}
