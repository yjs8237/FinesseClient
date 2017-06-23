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

        public Finesse()
        {
            logwrite = LogWrite.getInstance();
        }

        public int fnConnect(String fn_A_IP , String fn_B_IP ,String finesseDomain, String AEMS_A_IP , String AEMS_B_IP , int AEMS_Port , String ISPS_A_IP , String ISPS_B_IP , int ISPS_Port , int loglevel )
        {
            logwrite.write("fnConnect", "call fnConnect() \n");
            
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
                if (AEMSClient.startClient() != ERRORCODE.SUCCESS)
                {
                    logwrite.write("fnConnect", "AEMS Cannot Connect");
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
                if (ISPSClient.startClient() != ERRORCODE.SUCCESS)
                {
                    logwrite.write("fnConnect", "ISPS Cannot Connect");
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
            logwrite.write("fnConnect", "call fnDisconnect() \n");

            FinesseClient.disconnect();
            AEMSClient.disconnect();
            ISPSClient.disconnect();

            isFinesseConnected = false;
            isAEMSConnected = false;
            isISPSConnected = false;

            return ERRORCODE.SUCCESS;

        }


        public int fnLogin(String agentID , String agentPwd , String extension , String peripheralID)
        {
            logwrite.write("fnConnect", "call fnLogin() ID [" + agentID + "] Password [" + agentPwd + "] extension [" + extension + "] \n");
            Agent agent = new Agent(agentID , agentPwd, extension , peripheralID);
            return FinesseClient.login(agent);
        }

        public int fnLogout()
        {
            logwrite.write("fnConnect", "call fnLogout() \n");
            return FinesseClient.logout();
        }

        public int fnMakeCall(string dialNumber)
        {
            logwrite.write("fnConnect", "call fnMakeCall() \n");
            return FinesseClient.makeCall(dialNumber);
        }


        public int fnAnswer()
        {
            logwrite.write("fnAnswer", "call fnAnswer() \n");
            return FinesseClient.answer(dialogID);
        }

        public int fnRelease()
        {
            logwrite.write("fnRelease", "call fnRelease() \n");
            return FinesseClient.release(dialogID);
        }

        public int fnAgentState(string state)
        {
            logwrite.write("fnAgentState", "call fnAgentState("+state+") \n");
            return FinesseClient.agentState(state);
        }

        public int fnAgentState(string state, string reasonCode)
        {
            logwrite.write("fnAgentState", "call fnAgentState(" + state + " , " +reasonCode+ ") \n");
            return FinesseClient.agentState(state, reasonCode);
        }

        public void raiseEvent(Event evt)
        {
            if (evt == null)
            {
                logwrite.write("raiseEvent", ":::::::::::::::::::::::: evt NULL ::::::::::::::::::::::::");
                return;
            }


            Event evtObj = evt;
           

            int evtCode = evtObj.getEvtCode();
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

                case EVENT_TYPE.ON_CALL:
                    logwrite.write("raiseEvent", ":::::::::::::::::::::::: GetEventOnCallEvent ::::::::::::::::::::::::");
                    logwrite.write("raiseEvent", evtMessage);
                    logwrite.write("raiseEvent", "CALLTYPE : " + evt.getCallType() + " , STATE : " + evt.getCallState());
                    logwrite.write("raiseEvent", "::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    dialogID = evt.getDialogID();

                    checkTable(evt.getCallVariable());

                    GetEventOnAgentStateChange(evtMessage);
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
