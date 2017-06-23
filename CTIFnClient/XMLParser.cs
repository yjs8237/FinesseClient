﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using EVENTOBJ;
using CTIFnClient;
using VO;

namespace XML
{
    public class XMLParser
    {
        private ArrayList list;
        private XmlDocument xmlDoucment;
        private XmlNodeList nodeList;
        private XmlNodeList nodeList_1;
        private LogWrite logwrite;

        private Event evt;

        private Agent agent;


        /* 이벤트 구분을 위한.. */
        private bool isUpdateTag;
        private bool isDataTag;
        private bool isUserTag;
        private bool isStateTag;

        private bool isDialogTag;
        private bool isCallTypeTag;
        private bool isDialogIDTag;
        private bool isCallStateTag;

        private string agentstate;
        private string reasonCode;
        private string callType;
        private string dialogID;
        private string callState;

        public XMLParser(LogWrite logwrite , Agent agent)
        {
            list = new ArrayList();
            xmlDoucment = new XmlDocument();
            this.logwrite = logwrite;
            this.agent = agent;
        }


        public void findAllNodes(XmlNode node)
        {
            
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name.Equals("Update"))
                {
                    isUpdateTag = true;
                }
                if (n.Name.Equals("data"))
                {
                    isDataTag = true;
                }
                if (n.Name.Equals("user"))
                {
                    isUserTag = true;
                }
                if (n.Name.Equals("state"))
                {
                    agentstate = n.InnerText;
                    callState = n.InnerText;    // User 이벤트가 올때는 agentState 변수만 참조하고, Dialog 이벤트가 올때는 callState 변수만 참조한다.
                    isStateTag = true;
                }
                
                if (n.Name.Equals("state") && n.ParentNode.Name.Equals("Participant"))
                {
                    isCallStateTag = true;
                }
                
                if (n.Name.Equals("code")){ reasonCode = n.InnerText; }
                if (n.Name.Equals("Dialog")) { isDialogTag = true; }
                if (n.Name.Equals("callType")) { isCallTypeTag = true; callType = n.InnerText; }
                if (n.Name.Equals("id")) { isDialogIDTag = true; dialogID = n.InnerText; }


                findAllNodes(n);
            }

        }


        public Event parseXML(string xml)
        {
            
            try
            {
                if (xml == null)
                {
                    return null;
                }

                logwrite.write("### CHECK ###", xml);

                // xml 로드
                xmlDoucment.LoadXml(xml);

                evt = new Event();

                nodeList = xmlDoucment.GetElementsByTagName("user");
                if (nodeList.Count > 0)
                {
                    // AgentState 이벤트
                    
                    evt.setEvtMsg(xml);
                    evt.setEvtCode(EVENT_TYPE.ON_AGENTSTATE_CHANGE);
                    
                    XmlNode nodeone = nodeList.Item(0);
                    foreach (XmlNode node1 in nodeone.ChildNodes)
                    {
                        if (node1.Name.Equals("state"))
                        {
                            // 상담원 상태 데이터
                            evt.setAgentState(node1.InnerText.ToString());
                            logwrite.write("### EVENT CHECK ###", "Agentstate : " + node1.InnerText.ToString());
                        }
                        if (node1.Name.Equals("reasonCode"))
                        {
                            foreach (XmlNode node2 in node1.ChildNodes)
                            {   
                                // 상세 이석사유코드 데이터
                                if (node2.Name.Equals("code"))
                                {
                                    evt.setReasonCode(node2.InnerText.ToString());
                                    logwrite.write("### EVENT CHECK ###", "setReasonCode : " + node2.InnerText.ToString());
                                }
                            }
                        }
                    }
                    return evt;
                }

                nodeList = xmlDoucment.GetElementsByTagName("Dialog");
                nodeList_1 = xmlDoucment.GetElementsByTagName("dialog");
                if (nodeList.Count > 0 || nodeList_1.Count > 0)
                {
                    // Call 이벤트
                    evt.setEvtMsg(xml);
                    evt.setEvtCode(EVENT_TYPE.ON_CALL);

                    XmlNode nodeone = nodeList.Item(0);
                    foreach (XmlNode node1 in nodeone.ChildNodes)
                    {
                        if (node1.Name.Equals("id"))
                        {
                            // Dialog ID 데이터 (콜을 Control 하기 위해서 이 ID 가 필요하다)
                            evt.setDialogID(node1.InnerText.ToString());
                            logwrite.write("### EVENT CHECK ###", "setDialogID : " + node1.InnerText.ToString());
                        }
                        if (node1.Name.Equals("mediaProperties"))
                        {
                            foreach (XmlNode node2 in node1.ChildNodes)
                            {
                                if (node2.Name.Equals("callType"))
                                {
                                    evt.setCallType(node2.InnerText.ToString());
                                    logwrite.write("### EVENT CHECK ###", "setCallType : " + node2.InnerText.ToString());
                                }
                                if (node2.Name.Equals("callvariables"))
                                {
                                    evt.setCallVariable(getCallVarValues(node2));
                                    logwrite.write("### EVENT CHECK ###", "setCallVariable : " + getCallVarValues(node2).ToString());
                                }
                            }
                        }
                        if (node1.Name.Equals("participants"))
                        {
                            ArrayList list = getCallState(node1);
                            for (int i = 0; i < list.Count; i++)
                            {
                                Hashtable table = (Hashtable) list[i];
                                string extension = (string) table["mediaAddress"];
                                if (extension.Equals(agent.getExtension()))
                                {
                                    // 상담원 내선과 이벤트 내선이 같을 경우 이벤트 데이터에 포함시킨다.
                                    evt.setCallState((string)table["state"]);
                                    logwrite.write("### EVENT CHECK ###", "setCallState : " + (string)table["state"]);
                                }
                            }
                        }

                    }
                }
                

            }
            catch (Exception e)
            {
                logwrite.write("parseXML", e.ToString());
            }

            return evt;
        }

        private ArrayList getCallState(XmlNode node)
        {
            ArrayList list = new ArrayList();
            
            foreach (XmlNode node1 in node.ChildNodes)
            {
                Hashtable table = new Hashtable();
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    table.Add(node2.Name, node2.InnerText.ToString());
                }
                list.Add(table);
            }

            return list;
        }


        private Hashtable getCallVarValues(XmlNode node)
        {
            // 콜변수 데이터 정보를 가져오기 위한 XML 파싱 함수
            Hashtable table = new Hashtable();
            string key = "";
            string value = "";
            foreach (XmlNode tempNode in node.ChildNodes)
            {
                foreach (XmlNode tempNode2 in tempNode.ChildNodes)
                {
                    if (tempNode2.Name.Equals("name"))  // 콜변수 및 ECC 변수 명
                    {
                        key = tempNode2.InnerText.ToString();
                    }
                    if (tempNode2.Name.Equals("value")) // 콜변수 및 ECC 데이터
                    {
                        value = tempNode2.InnerText.ToString();
                    }
                }
                table.Add(key, value);
            }
            return table;
        }


        /*
        private IEvent agentStateChange(string xml , string state , string reasonCode)
        {
            evt = new Event();
            evt.setEvtMsg(xml);
            evt.setEvtCode(EVENT_TYPE.ON_AGENTSTATE_CHANGE);
            evt.setAgentState(state);
            evt.setReasonCode(reasonCode);
            return evt;
        }

        private IEvent callEvent(string xml, string callType, string dialogID, string callState)
        {
            evt = new Event();
            evt.setEvtMsg(xml);
            evt.setEvtCode(EVENT_TYPE.ON_CALL);
            evt.setCallType(callType);
            evt.setDialogID(dialogID);
            evt.setCallState(callState);
            return evt;

        }
         * */
    }
}
