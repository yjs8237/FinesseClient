using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TCPSOCKET;
using ThreadGroup;
using CONST;
using VO;
using EVENTOBJ;
using System.Collections;
using System.Xml;
using JSON;

namespace CTIFnClient
{
    public abstract class Finesse
    {

        private FinesseClient FinesseClient;
        private AEMSClient AEMSClient;
        private ISPSClient ISPSClient;

        private LogWrite logwrite;

        private bool isFinesseConnected = false;
        private bool isAEMSConnected = false;
        private bool isISPSConnected = false;

        private string dialogID;
        private string dialogID_second;
        private string activeDialogID;

        private string dialNumber;
        private string phonePadNum;
        private string phonePadCallID;

        private Hashtable reasonCodeTable;

        private ServerInfo finesseInfo;
         private ServerInfo aemsInfo;
         private ServerInfo ispsInfo;

         private PhonePad phonePadVO;

        public Finesse()
        {
            logwrite = LogWrite.getInstance();
            dialogID = "";  dialogID_second="";  activeDialogID = "";
        }

        public int fnConnect(String fn_A_IP , String fn_B_IP ,String finesseDomain, String AEMS_A_IP , String AEMS_B_IP , int AEMS_Port , String ISPS_A_IP , String ISPS_B_IP , int ISPS_Port , int loglevel )
        {
            logwrite.write("fnConnect", "\t ** call fnConnect() **");
            
            StringBuilder sb = new StringBuilder();
            logwrite.write("fnConnect", "Finesse A \t [" + fn_A_IP + "]");
            logwrite.write("fnConnect", "Finesse B \t [" + fn_B_IP + "]");
            logwrite.write("fnConnect", "AEMS A \t [" + AEMS_A_IP + "]");
            logwrite.write("fnConnect", "AEMS B \t [" + AEMS_B_IP + "]");
            logwrite.write("fnConnect", "AEMS Port \t [" + AEMS_Port + "]");
            logwrite.write("fnConnect", "ISPS A \t [" + ISPS_A_IP + "]");
            logwrite.write("fnConnect", "ISPS B \t [" + ISPS_B_IP + "]");
            logwrite.write("fnConnect", "ISPS Port \t [" + ISPS_Port + "]");
            logwrite.write("fnConnect", "Loglevel \t [" + loglevel + "]");

            int finesseport = SERVERINFO.Finesse_PORT;

            // 각 서버정보 객체화
            finesseInfo = new ServerInfo(fn_A_IP, fn_B_IP, finesseport, finesseDomain);
            aemsInfo = new ServerInfo(AEMS_A_IP, AEMS_B_IP, AEMS_Port );
            ispsInfo = new ServerInfo(ISPS_A_IP, ISPS_B_IP, ISPS_Port );

            /*
             *  finesse 세션 연결
             * */
            
            if (isFinesseConnected)
            {
                logwrite.write("fnConnect", "Finesse is Already Connected!!");
            }
            else
            {
                FinesseClient = new FinesseClient(logwrite , this);
                FinesseClient.setServerInfo(finesseInfo);
                if (FinesseClient.startClient() != ERRORCODE.SUCCESS)
                {
                    logwrite.write("fnConnect", "Finesse Cannot Connect");
                     isFinesseConnected = false;
                     logwrite.write("fnConnect", "\t Return Data : " + ERRORCODE.FAIL);
                    return ERRORCODE.FAIL;
                }
                else
                {
                    isFinesseConnected = true;
                }
            }

            /*
            AEMSClient = new AEMSClient(logwrite, this);
            AEMSClient.setServerInfo(aemsInfo);

            ISPSClient = new ISPSClient(logwrite, this);
            ISPSClient.setServerInfo(ispsInfo);
            */
            
            if (isAEMSConnected)
            {
                logwrite.write("fnConnect", "AEMS is Already Connected!!");
            }
            else
            {
                AEMSClient = new AEMSClient(logwrite , this);
                AEMSClient.setServerInfo(aemsInfo);
                if (AEMSClient.aemsConnect() != ERRORCODE.SUCCESS)
                {
                    logwrite.write("fnConnect", "AEMS Cannot Connect");
                    isAEMSConnected = false;
                    return ERRORCODE.FAIL;
                }
                else
                {
                    isAEMSConnected = true;
                }
            }

            if (isISPSConnected)
            {
                logwrite.write("fnConnect", "ISPS is Already Connected!!");
            }
            else
            {
                ISPSClient = new ISPSClient(logwrite , this);
                ISPSClient.setServerInfo(ispsInfo);
                if (ISPSClient.ispsConnect() != ERRORCODE.SUCCESS)
                {
                    logwrite.write("fnConnect", "ISPS Cannot Connect");
                    isISPSConnected = false;
                    return ERRORCODE.FAIL;
                }
                else
                {
                    isISPSConnected = true;
                }
            }
        
            logwrite.write("fnConnect", "\t Return Data : " + ERRORCODE.SUCCESS);

          
            return ERRORCODE.SUCCESS;
        }

        public int fnDisconnect()
        {
            logwrite.write("fnConnect", "\t ** call fnDisconnect() ** ");

            ErrorEvent evt = new ErrorEvent();
            evt.setEvtCode(EVENT_TYPE.ON_DISCONNECTION);

            if (FinesseClient != null)
            {
                FinesseClient.disconnect();
                evt.setEvtMsg("Finesse Session Disconnected");
                evt.setCurFinesseIP(FinesseClient.getCurrentServerIP());
                raiseEvent(evt);
            }
            if (AEMSClient != null)
            {
                AEMSClient.disconnect();
                evt.setEvtMsg("AEMS Session Disconnected");
                evt.setCurAemsIP(AEMSClient.getCurrentServerIP());
                raiseEvent(evt);
            }
            if (ISPSClient != null)
            {
                ISPSClient.disconnect();
                evt.setEvtMsg("ISPS Session Disconnected");
                evt.setCurIspsIP(ISPSClient.getCurrentServerIP());
                raiseEvent(evt);
            }

            isFinesseConnected = false;
            isAEMSConnected = false;
            isISPSConnected = false;
            logwrite.write("fnDisconnect", "\t Return Data : " + ERRORCODE.SUCCESS);
            return ERRORCODE.SUCCESS;

        }

        public int fnLogin(String agentID , String agentPwd , String extension , String peripheralID)
        {
            logwrite.write("fnConnect", "\t ** call fnLogin() ID [" + agentID + "] Password [" + agentPwd + "] extension [" + extension + "] ** ");

            reasonCodeTable = new Hashtable(); // 이석사유코드 정보를 최초 로그인시 메모리에 관리한다.
            //Agent agent = new Agent(agentID , agentPwd, extension , peripheralID);
            Agent agent = Agent.getInstance();
            agent.setAgentID(agentID);
            agent.setAgentPwd(agentPwd);
            agent.setExtension(extension);

            if (FinesseClient != null)
            {
                if (FinesseClient.login() == ERRORCODE.SUCCESS)
                {
                    // 로그인이 성공하면 Finesse 에 등록된 이석사유코드 리스트를 가져와 메모리에 올린다.
                    string reasonCodeXML = fnGetReasonCodeList();
                    setReasonCodeList(reasonCodeXML);
                    logwrite.write("fnLogin", "\t Return Data : " + ERRORCODE.SUCCESS);
                    return ERRORCODE.SUCCESS;
                }
                else
                {
                    logwrite.write("fnLogin", "\t Return Data : " + ERRORCODE.FAIL);
                    return ERRORCODE.FAIL;
                }
            }
            else
            {
                return ERRORCODE.FAIL;
            }
            
        }

        public int fnLogout()
        {
            logwrite.write("fnConnect", "\t ** call fnLogout() ** ");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.logout();
                logwrite.write("fnLogout", "\t Return Data : " + ret);
            }
             return ret;
        }

        public string fnGetReasonCodeList()
        {
            logwrite.write("fnGetReasonCodeList", "\t ** call fnGetReasonCodeList() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                return FinesseClient.getReasonCodeList();
            }
            else
            {
                return null;
            }
        }

        public int fnMakeCall(string dialNumber)
        {
            logwrite.write("fnMakeCall", "\t ** call fnMakeCall() **");
            this.dialNumber = dialNumber;
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
               ret =  FinesseClient.makeCall(dialNumber);
                logwrite.write("fnMakeCall", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnHold()
        {
            logwrite.write("fnHold", "\t ** call fnHold() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.hold(activeDialogID);
                logwrite.write("fnHold", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnRetrieve()
        {
            logwrite.write("fnRetrieve", "\t ** call fnRetrieve() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.retrieve(activeDialogID);
                logwrite.write("fnRetrieve", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnReconnect()
        {
            logwrite.write("fnReconnect", "\t ** call fnReconnect() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.reconnect(dialogID, dialogID_second);
                logwrite.write("fnReconnect", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnAnswer()
        {
            logwrite.write("fnAnswer", "\t ** call fnAnswer() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.answer(activeDialogID);
                logwrite.write("fnAnswer", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnRelease()
        {
            logwrite.write("fnRelease", "\t ** call fnRelease() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.release(activeDialogID);
                logwrite.write("fnRelease", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnSetCallData(string varName, string varValue)
        {
            logwrite.write("fnSetCallData", "\t ** call fnSetCallData() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.setCallData(varName, varValue, activeDialogID);
                logwrite.write("fnSetCallData", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnCCTransfer(string dialNumber)
        {
            logwrite.write("fnCCTransfer", "\t ** call fnCCTransfer() **");
            this.dialNumber = dialNumber;
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.ccTransfer(dialNumber, activeDialogID);
                logwrite.write("fnCCTransfer", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnTransfer()
        {
            logwrite.write("fnTransfer", "\t ** call fnTransfer() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                 ret = FinesseClient.transfer(dialNumber, dialogID);
                logwrite.write("fnTransfer", "\t Return Data : " + ret);
            }
            return ret;
        }
        public int fnCCConference(string dialNumber)
        {
            logwrite.write("fnCCConference", "\t ** call fnCCConference() **");
            this.dialNumber = dialNumber;
            // Conference 는 첫번째 콜 DialogID 로 요청해야한다.
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.ccConference(dialNumber, activeDialogID);
                logwrite.write("fnCCConference", "\t Return Data : " + ret);
            }
            
            return ret;
        }

        public int fnConference()
        {
            logwrite.write("fnConference", "\t ** call fnConference() **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.conference(dialNumber, dialogID);
                logwrite.write("fnConference", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnDropParticipant(string mediaAddress)
        {
            logwrite.write("fnDropParticipant", "\t ** call fnDropParticipant() **");
            return 0;
            //return FinesseClient.dropParticipant(mediaAddress);
        }


        public int fnAgentState(string state)
        {
            logwrite.write("fnAgentState", "\t ** call fnAgentState(" + state + ") **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.agentState(state);
                logwrite.write("fnAgentState", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnAgentState(string state, string reasonCode)
        {
            logwrite.write("fnAgentState", "\t ** call fnAgentState(" + state + " , " + reasonCode + ") **");

            if (reasonCodeTable == null || reasonCodeTable.Count == 0)
            {
                // 로그인이 성공하면 Finesse 에 등록된 이석사유코드 리스트를 가져와 메모리에 올린다.
                string reasonCodeXML = fnGetReasonCodeList();
                setReasonCodeList(reasonCodeXML);
            }
            string reasonCodeID = (string) reasonCodeTable[reasonCode];
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.agentState(state, reasonCodeID);
                logwrite.write("fnAgentState", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnArsTransfer(string dialNumber)
        {
            logwrite.write("fnArsTransfer", "\t ** call fnArsTransfer(" + dialNumber + ") **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.arsTransfer(dialNumber, activeDialogID);
                logwrite.write("fnArsTransfer", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnSSTransfer(string dialNumber)
        {
            logwrite.write("fnSSTransfer", "\t ** call fnSSTransfer(" + dialNumber + ") **");
            int ret = ERRORCODE.FAIL;
            if (FinesseClient != null)
            {
                ret = FinesseClient.ssTransfer(dialNumber, activeDialogID);
                logwrite.write("fnSSTransfer", "\t Return Data : " + ret);
            }
            return ret;
        }

        public int fnSendISPS(string ispsData)
        {
            logwrite.write("fnSendISPS", "\t ** call fnSendISPS(" + ispsData + ") **");
            if (ISPSClient.ispsConnect() != ERRORCODE.SUCCESS)
            {
                logwrite.write("fnSendISPS", "ISPS Cannot Connect");
                isAEMSConnected = false;
                return ERRORCODE.FAIL;
            }

            Agent agent = Agent.getInstance();

            string delimiter = Convert.ToString((char) 0x02);

            //"C302" & DELIMETER & "001" & DELIMETER & "" & DELIMETER & Trim$(tmpIspsSendAni) & DELIMETER & Trim$(tmpIspsSendData) & LF
            StringBuilder sb = new StringBuilder();
            sb.Append("C302").Append(delimiter).Append("001").Append(delimiter).Append("").Append(delimiter).Append(agent.getExtension()).Append(delimiter).Append(ispsData);

            if (ISPSClient.send(sb.ToString()) != ERRORCODE.SUCCESS)
            {
                logwrite.write("fnSendISPS", "ISPS SEND FAIL!!");
                return ERRORCODE.FAIL;
            }

            string retStr = ISPSClient.recv();

            ISPSClient.disconnect();

            if (retStr == null)
            {
                logwrite.write("fnSendISPS", "ISPS RECV FAIL!!");
                return ERRORCODE.FAIL;
            }

            return ERRORCODE.SUCCESS;
        }

        public int fnPhonePad(string phonePadNum, string type , string account)
        {

            logwrite.write("fnPhonePad", "\t ** call fnPhonePad(" + phonePadNum + " , " + account + ") **");
            this.phonePadNum = phonePadNum;

            if (setPhonePadData(type, account) != ERRORCODE.SUCCESS)
            {
                return ERRORCODE.FAIL;
            }
            else
            {
                // AEMS 서버로 부터 OK 받으면 컨퍼런스 실시
                fnCCConference(phonePadNum);
            }

            return ERRORCODE.SUCCESS;

        }

        private int getPhonePadInfo()
        {
            if (AEMSClient.aemsConnect() != ERRORCODE.SUCCESS)
            {
                logwrite.write("getPhonePadInfo", "AEMS Cannot Connect");
                isAEMSConnected = false;
                return ERRORCODE.FAIL;
            }

            Agent agent = Agent.getInstance();
            JsonHandler jsonhandler = new JsonHandler(agent.getExtension());

            jsonhandler.setType(phonePadVO.getType());
            jsonhandler.setCmd("get");            

            string jsonData = jsonhandler.getJsonData();

            jsonData = jsonData.Replace("classType", "@type");
            logwrite.write("getPhonePadInfo", "AEMS SEND MESSAGE [" + jsonData + "]");

            if (AEMSClient.send(jsonData) != ERRORCODE.SUCCESS)
            {
                logwrite.write("getPhonePadInfo", "AEMS SEND FAIL!!");
                return ERRORCODE.FAIL;
            }

            string retStr = AEMSClient.recv();
            logwrite.write("getPhonePadInfo", "AEMS RECV MESSAGE [" + retStr + "]");

            AEMSClient.disconnect();

            if (retStr == null || retStr.Length <= 0)
            {
                logwrite.write("getPhonePadInfo", "AEMS RECV MESSAGE IS NULL !!");
                return ERRORCODE.FAIL;
            }

            phonePadVO = jsonhandler.recvJson(retStr);

            string result = "";
            for (int i = 0; i < phonePadVO.getData().Count; i++)
			{
			    result = (string)phonePadVO.getData()[i];
			}

            GetEventOnPassCheck(phonePadVO.getRet(), result);

            logwrite.write("","");
            logwrite.write("getPhonePadInfo", "::::::::::::::::::::::: Raise Event GetEventOnPassCheck(" + phonePadVO.getRet() + " , " + result + ") ::::::::::::::::::::::: ");

            return ERRORCODE.SUCCESS;
        }

        private int setPhonePadData(string type , string account)
        {
            if (AEMSClient.aemsConnect() != ERRORCODE.SUCCESS)
            {
                logwrite.write("setPhonePadData", "AEMS Cannot Connect");
                isAEMSConnected = false;
                return ERRORCODE.FAIL;
            }

            Agent agent = Agent.getInstance();
            JsonHandler jsonhandler = new JsonHandler(agent.getExtension());

            jsonhandler.setType(type);
            jsonhandler.setCmd("set");
            jsonhandler.setAccount(account);
            
            string jsonData = jsonhandler.getJsonData();

            jsonData = jsonData.Replace("classType", "@type");
            jsonData = jsonData.Replace("null", "0");
            logwrite.write("setPhonePadData", "AEMS SEND MESSAGE [" + jsonData + "]");

            if (AEMSClient.send(jsonData) != ERRORCODE.SUCCESS)
            {
                logwrite.write("setPhonePadData", "AEMS SEND FAIL!!");
                return ERRORCODE.FAIL;
            }

            string retStr = AEMSClient.recv();
            logwrite.write("setPhonePadData", "AEMS RECV MESSAGE [" + retStr + "]");

            AEMSClient.disconnect();

            if (retStr == null || retStr.Length <= 0)
            {
                logwrite.write("setPhonePadData", "AEMS RECV MESSAGE IS NULL !!");
                return ERRORCODE.FAIL;
            }

            phonePadVO = jsonhandler.recvJson(retStr);

            if (!phonePadVO.getRet().Equals("0"))
            {
                return ERRORCODE.FAIL;
            }
           
            return ERRORCODE.SUCCESS;
        }


        private void setReasonCodeList(string xml)
        {
            XmlDocument xmlDoucment = new XmlDocument(); 
            XmlNodeList nodeList;
            try
            {
                xmlDoucment.LoadXml(xml);

                nodeList = xmlDoucment.GetElementsByTagName("ReasonCodes");
                XmlNode rootNode = nodeList.Item(0);

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    string key = ""; string value = "";
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        
                        if (node2.Name.Equals("uri"))
                        {
                            string tempStr = node2.InnerText.ToString();
                            char[] delimiter = {'/'};
                            string[] arr = tempStr.Split(delimiter);
                            value = arr[4];         // API 용 코드 값
                        }
                        if (node2.Name.Equals("code"))
                        {
                            key = node2.InnerText.ToString();   // reasoncde 값 실제 이용되는 코드 값
                        }
                    }
                    reasonCodeTable.Add(key, value);
                }

            }
            catch (Exception e)
            {
                logwrite.write("setReasonCodeList", e.ToString());
            }
        }

        public void setFinesseConnected(bool isConnected)
        {
            this.isFinesseConnected = isConnected;
        }
        public bool getFinesseConnected()
        {
            return this.isFinesseConnected;
        }
        public void setAEMSConnected(bool isConnected)
        {
            this.isAEMSConnected = isConnected;
        }
        public bool getAEMSconnected()
        {
            return this.isAEMSConnected;
        }
        public void setISPSConnected(bool isConnected)
        {
            this.isISPSConnected = isConnected;
        }
        public bool getISPSConnected()
        {
            return this.isISPSConnected;
        }



        public void raiseEvent(Event evt)
        {
            if (evt == null)
            {
                logwrite.write("raiseEvent", ":::::::::::::::::::::::: evt NULL ::::::::::::::::::::::::");
                return;
            }

            AgentEvent agentEvent = null;
            CallEvent callEvent = null;
            ErrorEvent errorEvent = null;

            if (evt is AgentEvent)
            {
                agentEvent = (AgentEvent)evt;
                raiseAgentEvent(agentEvent);
            }
            else if (evt is CallEvent)
            {
                callEvent = (CallEvent)evt;
                raiseCallEvent(callEvent);
            }
            else if (evt is ErrorEvent)
            {
                errorEvent = (ErrorEvent) evt;
                raiseErrorEvent(errorEvent);
            }


        }

        private void raiseAgentEvent(AgentEvent evt)
        {
            string evtCode = evt.getEvtCode();
            string evtMessage = evt.getEvtMsg();

            evtMessage = evtMessage.Replace("\n", "");

            switch (evtCode)
            {

                case EVENT_TYPE.ON_CONNECTION:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnConnection ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnConnection(evt.getCurFinesseIP(), evt.getCurAemsIP(), evt.getCurIspsIP(), evtMessage);
                    break;

                case EVENT_TYPE.ON_DISCONNECTION:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnDisConnection ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnDisConnection(evt.getCurFinesseIP(), evt.getCurAemsIP(), evt.getCurIspsIP(), evtMessage);
                    break;

                case EVENT_TYPE.ON_AGENTSTATE_CHANGE:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnAgentStateChange ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "STATE : " + evt.getAgentState() + " , REASONCODE : " + evt.getReasonCode());
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnAgentStateChange(evt.getAgentState(), evt.getReasonCode(), evtMessage);
                    break;

                default:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: UNKWON EVENT ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    break;
            }
        }
        private void raiseCallEvent(CallEvent evt)
        {
            string evtCode = evt.getEvtCode();
            string evtMessage = evt.getEvtMsg();

            evtMessage = evtMessage.Replace("\n", "");

            StringBuilder callState = new StringBuilder();

            foreach (DictionaryEntry item in evt.getCallStateTable())
            {
                callState.Append(item.Key).Append("^").Append(item.Value).Append("|");
            }
            if (callState.ToString().EndsWith("|"))
            {
                string tempStr = callState.ToString().Substring(0, callState.ToString().Length - 1);
                callState = new StringBuilder();
                callState.Append(tempStr);
            }

            StringBuilder callAction = new StringBuilder();
            foreach (string str in evt.getActionList())
            {
                callAction.Append(str).Append("^");
            }
            if (callAction.ToString().EndsWith("^"))
            {
                string tempStr = callAction.ToString().Substring(0, callAction.ToString().Length - 1);
                callAction = new StringBuilder();
                callAction.Append(tempStr);
            }
            

            switch (evtCode)
            {

                case EVENT_TYPE.ON_CONNECTION:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnConnection ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnConnection(evt.getCurFinesseIP(), evt.getCurAemsIP(), evt.getCurIspsIP(), evtMessage);
                    break;

                case EVENT_TYPE.ON_DISCONNECTION:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnDisConnection ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnDisConnection(evt.getCurFinesseIP(), evt.getCurAemsIP(), evt.getCurIspsIP(), evtMessage);
                    break;

                case EVENT_TYPE.ALERTING:
                    writeCallEventLog("GetEventOnCallAlerting", evt);
                    setActiveDialogID(evt);
                    GetEventOnCallAlerting(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString() , callAction.ToString());
                    break;

                case EVENT_TYPE.FAILED:
                    writeCallEventLog("GetEventOnCallFailed", evt);
                    setActiveDialogID(evt);
                    GetEventOnCallFailed(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString(), callAction.ToString());
                    break;

                case EVENT_TYPE.ACTIVE:
                    writeCallEventLog("GetEventOnCallActive", evt);
                    setActiveDialogID(evt);
                    GetEventOnCallActive(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString(), callAction.ToString());

                    if (evt.getToAddress()!=null && evt.getToAddress().Equals(phonePadNum))
                    {
                        // 폰패드 컨퍼런스 
                        logwrite.write("raiseEvent", "PhonePad Conference Start");
                        phonePadCallID = dialogID;  // phonePad 콜 구분을 위한 DialogID 세팅
                        fnConference();
                    }
                    break;

                case EVENT_TYPE.HELD:
                    writeCallEventLog("GetEventOnCallHeld", evt);
                    //setActiveDialogID(evt);
                    GetEventOnCallHeld(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString(), callAction.ToString());
                    break;

                case EVENT_TYPE.INITIATING:
                    writeCallEventLog("GetEventOnCallInitiating", evt);
                    setActiveDialogID(evt);
                    GetEventOnCallInitiating(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString(), callAction.ToString());
                    break;

                case EVENT_TYPE.INITIATED:
                    writeCallEventLog("GetEventOnCallInitiated", evt);
                    setActiveDialogID(evt);
                    GetEventOnCallInitiated(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString(), callAction.ToString());
                    break;


            case EVENT_TYPE.WRAP_UP:
                    writeCallEventLog("GetEventOnCallWrapUp", evt);
                    // checkTable(callEvent.getCallVariable());
                    removeDialogID(evt);
                    GetEventOnCallWrapUp(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString(), callAction.ToString());
                    break;

            case EVENT_TYPE.DROPPED:
                    writeCallEventLog("GetEventOnCallDropped", evt);
                    // checkTable(callEvent.getCallVariable());
                    removeDialogID(evt);
                    GetEventOnCallDropped(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), callState.ToString(), callAction.ToString());
                    if (evt.getCallType().Equals(CALL.CONFERENCE) && evt.getDialogID().Equals(phonePadCallID))
                    {
                        // 폰패드 이후 Dropped 이벤트일 경우 폰패드 결과를 요청한다.
                        getPhonePadInfo();
                    }
                    break;     

                default:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: UNKWON EVENT ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    setActiveDialogID(evt);
                    break;
            }
        }
        private void raiseErrorEvent(ErrorEvent evt)
        {

            string evtCode = evt.getEvtCode();
            string evtMessage = evt.getEvtMsg();

            evtMessage = evtMessage.Replace("\n", "");

            switch (evtCode)
            {

                case EVENT_TYPE.ON_CONNECTION:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnConnection ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnConnection(evt.getCurFinesseIP(), evt.getCurAemsIP(), evt.getCurIspsIP(), evtMessage);
                    break;

                case EVENT_TYPE.ON_DISCONNECTION:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnDisConnection ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnDisConnection(evt.getCurFinesseIP(), evt.getCurAemsIP(), evt.getCurIspsIP(), evtMessage);
                    break;

                case EVENT_TYPE.ERROR:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: GetEventOnError ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "ERROR TYPE : " + evt.getErrorType() + " , ERROR MESSAGE : " + evt.getErrorMessage());
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnCallError(evtMessage);
                    break;

                default:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: UNKWON EVENT ::::::::::::::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    break;
            }
        }

        private void writeCallEventLog(string eventName , CallEvent evt )
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("STATE : ");
            foreach (DictionaryEntry item in evt.getCallStateTable())
            {
                sb.Append("[" + item.Key + " -> " + item.Value + "]");
            }

            StringBuilder actionBuilder = new StringBuilder();
            actionBuilder.Append("ACTION : ");
            foreach (string str in evt.getActionList()) 
            {
                actionBuilder.Append("[" + str + "]");
            }
            logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: "+eventName+" ::::::::::::::::::::::::::::::::::::");
            logwrite.write("raiseEvent", evt.getEvtMsg());
            logwrite.write("raiseEvent", "ID : " + evt.getDialogID());
            logwrite.write("raiseEvent", "CALLTYPE : " + evt.getCallType());
            logwrite.write("raiseEvent", "FromAddress : " + evt.getFromAddress());
            logwrite.write("raiseEvent", "ToAddress : " + evt.getToAddress());
            logwrite.write("raiseEvent", sb.ToString());
            logwrite.write("raiseEvent", actionBuilder.ToString());
            logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
        }

        private void removeDialogID(CallEvent evt)
        {

            if (dialogID_second.Equals(evt.getDialogID()))
            {
                dialogID_second = "";
                activeDialogID = dialogID;
            }
            if (dialogID.Equals(evt.getDialogID()))
            {
                dialogID = "";
                activeDialogID = "";
            }

            logwrite.write("removeDialogID", "Active DialogID : " + activeDialogID + " , first ID : " + dialogID + " , seconde ID : " + dialogID_second);

        }

        private void setActiveDialogID(CallEvent evt)
        {
            if (evt.getCallType().Equals("CONSULT"))
            {
                dialogID_second = evt.getDialogID();
                activeDialogID = dialogID_second;
            }
            else
            {
                if (evt.getDialogID().Equals(dialogID_second))
                {
                    dialogID_second = evt.getDialogID();
                    activeDialogID = dialogID_second;
                }
                else
                {
                    dialogID = evt.getDialogID();
                    activeDialogID = dialogID;
                }
            }
            logwrite.write("setDialogID", "Active DialogID : " + activeDialogID + " , first ID : " + dialogID + " , seconde ID : " + dialogID_second);
        }
        public void setActiveDialogID(string dialogID)
        {
            activeDialogID = dialogID;
        }

        private void checkTable(Hashtable table) {

            foreach (DictionaryEntry item in table)
            {
                logwrite.write("checkTable", "key : " + item.Key + " , " + item.Value);
            }

        }


        /*
         *  Call 이벤트
         * */
        public abstract void GetEventOnConnection(string finesseIP, string aemsIP, string ispsIP, String evt);
        public abstract void GetEventOnDisConnection(string finesseIP, string aemsIP, string ispsIP, String evt);
        public abstract void GetEventOnCallAlerting(string dialogID, string callType, string fromAddress, string toAddress, string callState , string actionList);
        //public abstract void GetEventOnCallEstablished(string dialogID, string callType, string fromAddress, string toAddress, Hashtable table);
        public abstract void GetEventOnCallActive(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList);
        public abstract void GetEventOnCallDropped(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList);
        public abstract void GetEventOnCallWrapUp(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList);
        public abstract void GetEventOnCallHeld(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList);
        public abstract void GetEventOnCallInitiating(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList);
        public abstract void GetEventOnCallInitiated(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList);
        public abstract void GetEventOnCallFailed(string dialogID, string callType, string fromAddress, string toAddress, string callState, string actionList);

        public abstract void GetEventOnPassCheck(string ret, string data);

        /*
         *  Agent State 이벤트
         * */
        public abstract void GetEventOnAgentStateChange(string state , string reasonCode , string evtMessage);

        public abstract void GetEventOnCallError(string errorMessage);

        

   
        
    }



}
