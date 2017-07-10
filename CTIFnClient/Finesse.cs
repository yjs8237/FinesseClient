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
        private string phonePadSysNum;


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
                    return ERRORCODE.FAIL;
                }
                else
                {
                    isFinesseConnected = true;
                }
            }

            AEMSClient = new AEMSClient(logwrite, this);
            AEMSClient.setServerInfo(aemsInfo);

            ISPSClient = new ISPSClient(logwrite, this);
            ISPSClient.setServerInfo(ispsInfo);

            /*
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
             * */

            return ERRORCODE.SUCCESS;
        }

        public int fnDisconnect()
        {
            logwrite.write("fnConnect", "\t ** call fnDisconnect() ** ");

            Event evt = new Event();
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


            if (FinesseClient.login() == ERRORCODE.SUCCESS)
            {
                // 로그인이 성공하면 Finesse 에 등록된 이석사유코드 리스트를 가져와 메모리에 올린다.
                string reasonCodeXML = fnGetReasonCodeList();
                setReasonCodeList(reasonCodeXML);
                return ERRORCODE.SUCCESS;
            }
            else
            {
                return ERRORCODE.FAIL;
            }
        }

        public int fnLogout()
        {
            logwrite.write("fnConnect", "\t ** call fnLogout() ** ");
            return FinesseClient.logout();
        }

        public string fnGetReasonCodeList()
        {
            logwrite.write("fnGetReasonCodeList", "\t ** call fnGetReasonCodeList() **");
            return FinesseClient.getReasonCodeList();
        }

        public int fnMakeCall(string dialNumber)
        {
            logwrite.write("fnMakeCall", "\t ** call fnMakeCall() **");
            this.dialNumber = dialNumber;
            return FinesseClient.makeCall(dialNumber);
        }

        public int fnHold()
        {
            logwrite.write("fnHold", "\t ** call fnHold() **");
            return FinesseClient.hold(activeDialogID);
        }

        public int fnRetrieve()
        {
            logwrite.write("fnRetrieve", "\t ** call fnRetrieve() **");
            return FinesseClient.retrieve(activeDialogID);
        }

        public int fnReconnect()
        {
            logwrite.write("fnReconnect", "\t ** call fnReconnect() **");
            return FinesseClient.reconnect(dialogID, dialogID_second);
        }

        public int fnAnswer()
        {
            logwrite.write("fnAnswer", "\t ** call fnAnswer() **");
            return FinesseClient.answer(activeDialogID);
        }

        public int fnRelease()
        {
            logwrite.write("fnRelease", "\t ** call fnRelease() **");
            return FinesseClient.release(activeDialogID);
        }

        public int fnSetCallData(string varName, string varValue)
        {
            logwrite.write("fnSetCallData", "\t ** call fnSetCallData() **");
            return FinesseClient.setCallData(varName, varValue, activeDialogID);
        }

        public int fnCCTransfer(string dialNumber)
        {
            logwrite.write("fnCCTransfer", "\t ** call fnCCTransfer() **");
            this.dialNumber = dialNumber;
            return FinesseClient.ccTransfer(dialNumber, activeDialogID);
        }

        public int fnTransfer()
        {
            logwrite.write("fnTransfer", "\t ** call fnTransfer() **");
            return FinesseClient.transfer(dialNumber, dialogID);
        }
        public int fnCCConference(string dialNumber)
        {
            logwrite.write("fnCCConference", "\t ** call fnCCConference() **");
            this.dialNumber = dialNumber;
            // Conference 는 첫번째 콜 DialogID 로 요청해야한다.
            return FinesseClient.ccConference(dialNumber, activeDialogID);
        }

        public int fnConference()
        {
            logwrite.write("fnConference", "\t ** call fnConference() **");
            return FinesseClient.conference(dialNumber, dialogID);
        }


        public int fnAgentState(string state)
        {
            logwrite.write("fnAgentState", "\t ** call fnAgentState(" + state + ") **");
            return FinesseClient.agentState(state);
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
            return FinesseClient.agentState(state, reasonCodeID);
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

            if (!phonePadVO.getRet().Equals("0"))
            {
                return ERRORCODE.FAIL;
            }


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

            string number01 = "";
            string state01 = "";
            string number02 = "";
            string state02 = "";
            string number03 = "";
            string state03 = "";

            evtMessage = evtMessage.Replace("\n", "");

            int index = 0;
            foreach (DictionaryEntry item in evt.getCallStateTable())
            {
                if (index == 0)
                {
                    number01 = (string)item.Key;
                    state01 = (string)item.Value;
                }
                else if (index == 1)
                {
                    number02 = (string)item.Key;
                    state02 = (string)item.Value;
                }
                else if (index == 2)
                {
                    number03 = (string)item.Key;
                    state03 = (string)item.Value;
                }
                index++;
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
                    writeCallEventLog("GetEventOnCallAlerting", evt, number01, state01, number02, state02, number03, state03);
                    setActiveDialogID(evt);
                    GetEventOnCallAlerting(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    break;

                case EVENT_TYPE.FAILED:
                    writeCallEventLog("GetEventOnCallFailed", evt, number01, state01, number02, state02, number03, state03);
                    setActiveDialogID(evt);
                    GetEventOnCallFailed(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    break;

                case EVENT_TYPE.ESTABLISHED:
                    writeCallEventLog("GetEventOnCallEstablished", evt, number01, state01, number02, state02, number03, state03);
                    setActiveDialogID(evt);
                    GetEventOnCallEstablished(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    if (evt.getToAddress().Equals(phonePadNum))
                    {
                        // 폰패드 컨퍼런스 
                        logwrite.write("raiseEvent", "PhonePad Conference Start");
                        if (state02.Length > 0) { phonePadSysNum = state02; }
                        else if (state03.Length > 0) { phonePadSysNum = state03;  }
                        fnConference();
                    }
                    break;

                case EVENT_TYPE.HELD:
                    writeCallEventLog("GetEventOnCallHeld", evt, number01, state01, number02, state02, number03, state03);
                    //setActiveDialogID(evt);
                    GetEventOnCallHeld(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    break;

                case EVENT_TYPE.INITIATING:
                    writeCallEventLog("GetEventOnCallInitiating", evt, number01, state01, number02, state02, number03, state03);
                    setActiveDialogID(evt);
                    GetEventOnCallInitiating(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    break;

                case EVENT_TYPE.INITIATED:
                    writeCallEventLog("GetEventOnCallInitiated", evt, number01, state01, number02, state02, number03, state03);
                    setActiveDialogID(evt);
                    GetEventOnCallInitiated(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    break;


            case EVENT_TYPE.WRAP_UP:
                    writeCallEventLog("GetEventOnCallWrapUp", evt, number01, state01, number02, state02, number03, state03);
                    // checkTable(callEvent.getCallVariable());
                    removeDialogID(evt);
                    GetEventOnCallWrapUp(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    break;

            case EVENT_TYPE.DROPPED:
                    writeCallEventLog("GetEventOnCallDropped", evt, number01, state01, number02, state02, number03, state03);
                    // checkTable(callEvent.getCallVariable());
                    removeDialogID(evt);
                    GetEventOnCallDropped(evt.getDialogID(), evt.getCallType(), evt.getFromAddress(), evt.getToAddress(), number01, state01, number02, state02, number03, state03);
                    if (evt.getCallType().Equals(CALL.CONFERENCE))
                    {
                        if (number02.Equals(phonePadSysNum) && state02.Equals(EVENT_TYPE.DROPPED))
                        {
                            getPhonePadInfo();
                        }
                        else if (number03.Equals(phonePadSysNum) && state03.Equals(EVENT_TYPE.DROPPED))
                        {
                            getPhonePadInfo();
                        }
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

        private void writeCallEventLog(string eventName , CallEvent evt , string number01 ,string state01 , string number02 , string state02 , string number03 , string state03)
        {

            logwrite.write("raiseEvent", ":::::::::::::::::::::::::::::::::::: "+eventName+" ::::::::::::::::::::::::::::::::::::");
            logwrite.write("raiseEvent", evt.getEvtMsg());
            logwrite.write("raiseEvent", "ID : " + evt.getDialogID());
            logwrite.write("raiseEvent", "CALLTYPE : " + evt.getCallType());
            logwrite.write("raiseEvent", "FromAddress : " + evt.getFromAddress());
            logwrite.write("raiseEvent", "ToAddress : " + evt.getToAddress());
            logwrite.write("raiseEvent", "STATE : [" + number01 + " -> " + state01 + "][" + number02 + " -> " + state02 + "][" + number03 + " -> " + state03 + "]");
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
                dialogID = evt.getDialogID();
                activeDialogID = dialogID;
            }
            logwrite.write("setDialogID", "Active DialogID : " + activeDialogID + " , first ID : " + dialogID + " , seconde ID : " + dialogID_second);
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
        public abstract void GetEventOnCallAlerting(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);
        public abstract void GetEventOnCallEstablished(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);

        public abstract void GetEventOnCallDropped(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);
        public abstract void GetEventOnCallWrapUp(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);
        public abstract void GetEventOnCallHeld(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);
        public abstract void GetEventOnCallInitiating(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);
        public abstract void GetEventOnCallInitiated(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);
        public abstract void GetEventOnCallFailed(string dialogID, string callType, string fromAddress, string toAddress, string num01, string state01, string number02, string state02, string number03, string state03);
        
        

        /*
         *  Agent State 이벤트
         * */
        public abstract void GetEventOnAgentStateChange(string state , string reasonCode , string evtMessage);

        public abstract void GetEventOnCallError(string errorMessage);

        

   
        
    }



}
