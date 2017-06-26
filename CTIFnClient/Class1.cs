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

        private Hashtable reasonCodeTable;

        public Finesse()
        {
            logwrite = LogWrite.getInstance();
        }
     

        public int fnConnect(String fn_A_IP , String fn_B_IP ,String finesseDomain, String AEMS_A_IP , String AEMS_B_IP , int AEMS_Port , String ISPS_A_IP , String ISPS_B_IP , int ISPS_Port , int loglevel )
        {
            logwrite.write("fnConnect", "\n call fnConnect() \n");
            
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
            ServerInfo finesseInfo = new ServerInfo(fn_A_IP, fn_B_IP, finesseport, finesseDomain);
            ServerInfo aemsInfo = new ServerInfo(AEMS_A_IP, AEMS_B_IP, AEMS_Port );
            ServerInfo ispsInfo = new ServerInfo(ISPS_A_IP, ISPS_B_IP, ISPS_Port );


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

            if (isFinesseConnected && isAEMSConnected && isISPSConnected)
            {
                Event evt = new Event();
                evt.setEvtCode(EVENT_TYPE.ON_CONNECTION);
                evt.setEvtMsg("CONNECTED SUCCESS");
                raiseEvent(evt);
            }

            return ERRORCODE.SUCCESS;
        }

        public int fnDisconnect()
        {
            logwrite.write("fnConnect", "\n call fnDisconnect() \n");
            if (FinesseClient != null)
            {
                FinesseClient.disconnect();
            }
            if (AEMSClient != null)
            {
                AEMSClient.disconnect();
            }
            if (ISPSClient != null)
            {
                ISPSClient.disconnect();
            }
            isFinesseConnected = false;
            isAEMSConnected = false;
            isISPSConnected = false;

            return ERRORCODE.SUCCESS;

        }


        public int fnLogin(String agentID , String agentPwd , String extension , String peripheralID)
        {
            logwrite.write("fnConnect", "\n call fnLogin() ID [" + agentID + "] Password [" + agentPwd + "] extension [" + extension + "] \n");

            reasonCodeTable = new Hashtable(); // 이석사유코드 정보를 최초 로그인시 메모리에 관리한다.
            //Agent agent = new Agent(agentID , agentPwd, extension , peripheralID);
            Agent agent = Agent.getInstance();
            agent.setAgentID(agentID);
            agent.setAgentPwd(agentPwd);
            agent.setExtension(extension);


            if (FinesseClient.login(agent) == ERRORCODE.SUCCESS)
            {
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
            logwrite.write("fnConnect", "\n call fnLogout() \n");
            return FinesseClient.logout();
        }

        public string fnGetReasonCodeList()
        {
            logwrite.write("fnGetReasonCodeList", "\n call fnGetReasonCodeList() \n");
            return FinesseClient.getReasonCodeList();
        }

        public int fnMakeCall(string dialNumber)
        {
            logwrite.write("fnConnect", "\n call fnMakeCall() \n");
            return FinesseClient.makeCall(dialNumber);
        }

        public int fnHold()
        {
            logwrite.write("fnHold", "\n call fnHold() \n");
            return FinesseClient.hold(dialogID);
        }

        public int fnRetrieve()
        {
            logwrite.write("fnRetrieve", "\n call fnRetrieve() \n");
            return FinesseClient.retrieve(dialogID);
        }

        public int fnAnswer()
        {
            logwrite.write("fnAnswer", "\n call fnAnswer() \n");
            return FinesseClient.answer(dialogID);
        }

        public int fnRelease()
        {
            logwrite.write("fnRelease", "\n call fnRelease() \n");
            return FinesseClient.release(dialogID);
        }

        public int fnSetCallData(string varName, string varValue)
        {
            logwrite.write("fnSetCallData", "\n call fnSetCallData() \n");
            return FinesseClient.setCallData(varName, varValue, dialogID);
        }

        public int fnCCTransfer(string dialNumber)
        {
            logwrite.write("fnCCTransfer", "\n call fnCCTransfer() \n");
            return FinesseClient.ccTransfer(dialNumber, dialogID);
        }


        public int fnAgentState(string state)
        {
            logwrite.write("fnAgentState", "\n call fnAgentState(" + state + ") \n");
            return FinesseClient.agentState(state);
        }

        public int fnAgentState(string state, string reasonCode)
        {
            logwrite.write("fnAgentState", "\n call fnAgentState(" + state + " , " + reasonCode + ") \n");
            string reasonCodeID = (string) reasonCodeTable[reasonCode];
            return FinesseClient.agentState(state, reasonCodeID);
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

            Event evtObj = evt;

            string evtCode = evtObj.getEvtCode();
            string evtMessage = evtObj.getEvtMsg();

            evtMessage = evtMessage.Replace("\n", "");

            switch (evtCode)
            {
                case EVENT_TYPE.ON_CONNECTION:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::: GetEventOnConnection ::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnConnection(evtMessage);
                    break;

                case EVENT_TYPE.ON_AGENTSTATE_CHANGE:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::: GetEventOnAgentStateChange ::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "STATE : " + evt.getAgentState() + " , REASONCODE : " + evt.getReasonCode());
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    GetEventOnAgentStateChange(evtMessage);
                    break;

                case EVENT_TYPE.ALERTING:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::: GetEventOnCallAlerting ::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "CALLTYPE : " + evt.getCallType() + " , STATE : " + evt.getCallState());
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    dialogID = evt.getDialogID();

                    checkTable(evt.getCallVariable());

                    GetEventOnCallAlerting(evtMessage);
                    break;
                case EVENT_TYPE.ACTIVE:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::: GetEventOnCallActive ::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "CALLTYPE : " + evt.getCallType() + " , STATE : " + evt.getCallState());
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");

                    checkTable(evt.getCallVariable());
                    GetEventOnCallActive(evtMessage);
                    break;

                case EVENT_TYPE.WRAP_UP:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::: GetEventOnCallWrapup ::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "CALLTYPE : " + evt.getCallType() + " , STATE : " + evt.getCallState());
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");

                    checkTable(evt.getCallVariable());
                    GetEventOnCallWrapup(evtMessage);
                    break;

                default :

                    break;
            }

        }

        private void checkTable(Hashtable table) {

            foreach (DictionaryEntry item in table)
            {
                logwrite.write("checkTable", "key : " + item.Key + " , " + item.Value);

            }

        }

        public abstract void GetEventOnCallWrapup(String evt);
        public abstract void GetEventOnCallActive(String evt);
        public abstract void GetEventOnCallAlerting(String evt);
        public abstract void GetEventOnAgentStateChange(String evt);
        public abstract void GetEventOnError(String evt);
        public abstract void GetEventOnConnection(String evt);
        public abstract void GetEventOnConnectionClosed(String evt);
        public abstract void GetEventOnCallBegin(String evt);
        public abstract void GetEventOnCallDelivered(String evt);
        public abstract void GetEventOnCallEstablished(String evt);
        public abstract void GetEventOnCallHeld(String evt);
        public abstract void GetEventOnCallRetrieved(String evt);
        public abstract void GetEventOnCallConnectionCleared(String evt);
        public abstract void GetEventOnLoginFail(String evt);
        public abstract void GetEventOnPasswordChecked(String evt);
        public abstract void GetEventOnConnectionFail(String evt);
        
    }



}
