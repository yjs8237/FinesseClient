using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using EVENTOBJ;
using CTIFnClient;
using VO;
using CONST;

namespace XML
{
    public class XMLParser
    {
        private ArrayList list;
        private XmlDocument xmlDoucment;
        private XmlNodeList nodeList;
        private LogWrite logwrite;

        private Event evt;

        private Agent agent;


        public XMLParser(LogWrite logwrite , Agent agent)
        {
            list = new ArrayList();
            xmlDoucment = new XmlDocument();
            this.logwrite = logwrite;
            this.agent = agent;
        }

        public XMLParser(LogWrite logwrite)
        {
            this.logwrite = logwrite;
            xmlDoucment = new XmlDocument();
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

                evt = null;

                // Agent State Event
                nodeList = xmlDoucment.GetElementsByTagName("user");
                if (nodeList.Count > 0)
                {
                    evt = getUserEvent(nodeList);
                    evt.setEvtMsg(xml);
                    return evt;
                }
                // Call Event
                nodeList = xmlDoucment.GetElementsByTagName("Dialog");
                if (nodeList.Count > 0)
                {
                    evt = getdialogEvent(nodeList);
                    evt.setEvtMsg(xml);
                    return evt;
                }

                // Call Event
                nodeList = xmlDoucment.GetElementsByTagName("dialog");
                if (nodeList.Count > 0)
                {
                    evt = getdialogEvent(nodeList);
                    evt.setEvtMsg(xml);
                    return evt;
                }

                // Error Event
                nodeList = xmlDoucment.GetElementsByTagName("apiErrors");
                if (nodeList.Count > 0)
                {
                    evt = getErrorEvent(nodeList);
                    evt.setEvtMsg(xml);
                    return evt;
                }


            }
            catch (Exception e)
            {
                logwrite.write("parseXML", e.ToString());
            }

            return evt;
        }

        private Event getErrorEvent(XmlNodeList nodeList)
        {
            // Error 이벤트
            ErrorEvent evt = new ErrorEvent();
            evt.setEvtCode(EVENT_TYPE.ERROR);

             XmlNode nodeone = nodeList.Item(0);
             foreach (XmlNode node1 in nodeone.ChildNodes)
             {
                 if (node1.Name.Equals("apiError"))
                 {
                     foreach (XmlNode node2 in node1.ChildNodes)
                     {
                         if (node2.Name.Equals("errorMessage"))
                         {
                             evt.setErrorMessage(node2.InnerText.ToString());
                             //logwrite.write("### EVENT CHECK ###", "ErrorMessage : " + node2.InnerText.ToString());
                         }
                         if (node2.Name.Equals("errorType"))
                         {
                             evt.setErrorType(node2.InnerText.ToString());
                             //logwrite.write("### EVENT CHECK ###", "ErrorType : " + node2.InnerText.ToString());
                         }
                     }
                 }
             }

             return evt;
        }

        private Event getUserEvent(XmlNodeList nodeList)
        {
            // AgentState 이벤트
            AgentEvent evt = new AgentEvent();
            evt.setEvtCode(EVENT_TYPE.ON_AGENTSTATE_CHANGE);

            XmlNode nodeone = nodeList.Item(0);
            foreach (XmlNode node1 in nodeone.ChildNodes)
            {
                if (node1.Name.Equals("state"))
                {
                    // 상담원 상태 데이터
                    evt.setAgentState(node1.InnerText.ToString());
                   // logwrite.write("### EVENT CHECK ###", "Agentstate : " + node1.InnerText.ToString());
                }
                if (node1.Name.Equals("reasonCode"))
                {
                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        // 상세 이석사유코드 데이터
                        if (node2.Name.Equals("code"))
                        {
                            evt.setReasonCode(node2.InnerText.ToString());
                           // logwrite.write("### EVENT CHECK ###", "setReasonCode : " + node2.InnerText.ToString());
                        }
                    }
                }
            }
            return evt;
        }

        private Event getdialogEvent(XmlNodeList nodeList) 
        {
            CallEvent evt = null;

            if (nodeList.Count > 0)
            {
                // Call 이벤트
                evt = new CallEvent();

                //evt.setEvtCode(EVENT_TYPE.ON_CALL);

                XmlNode nodeone = nodeList.Item(0);
                foreach (XmlNode node1 in nodeone.ChildNodes)
                {
                    if (node1.Name.Equals("fromAddress"))
                    {
                        evt.setFromAddress(node1.InnerText.ToString());
                      //  logwrite.write("### EVENT CHECK ###", "setFromAddress : " + node1.InnerText.ToString());
                    }

                    if (node1.Name.Equals("toAddress"))
                    {
                        evt.setToAddress(node1.InnerText.ToString());
                       // logwrite.write("### EVENT CHECK ###", "setToAddress : " + node1.InnerText.ToString());
                    }

                    if (node1.Name.Equals("id"))
                    {
                        // Dialog ID 데이터 (콜을 Control 하기 위해서 이 ID 가 필요하다)
                        evt.setDialogID(node1.InnerText.ToString());
                       // logwrite.write("### EVENT CHECK ###", "setDialogID : " + node1.InnerText.ToString());
                    }
                    if (node1.Name.Equals("mediaProperties"))
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name.Equals("callType"))
                            {
                                evt.setCallType(node2.InnerText.ToString());
                                //logwrite.write("### EVENT CHECK ###", "setCallType : " + node2.InnerText.ToString());
                            }
                            if (node2.Name.Equals("callvariables"))
                            {
                                evt.setCallVariable(getCallVarValues(node2));
                                //logwrite.write("### EVENT CHECK ###", "setCallVariable : " + getCallVarValues(node2).ToString());
                            }
                        }
                    }
                    if (node1.Name.Equals("participants"))
                    {

                        ArrayList list = getCallState(node1);
                        for (int i = 0; i < list.Count; i++)
                        {
                            Hashtable table = (Hashtable)list[i];
                            string extension = (string)table["mediaAddress"];
                            string state = (string)table["state"];
                            evt.setCallState(extension, state);
                            /*
                            if (extension.Equals(agent.getExtension()))
                            {
                                // 상담원 내선과 이벤트 내선이 같을 경우 이벤트 데이터에 포함시킨다.
                                evt.setCallState((string)table["state"]);
                                evt.setEvtCode((string)table["state"]);
                                //logwrite.write("### EVENT CHECK ###", "setCallState : " + (string)table["state"]);
                            }
                             * */
                        }

                        Hashtable callStateList = evt.getCallStateTable();
                        int tempInt = 0;
                        foreach (DictionaryEntry item in callStateList)
                        {
                            if (item.Value.Equals("DROPPED"))
                            {
                                tempInt++;
                            }
                        }

                        if(tempInt == 0) 
                        {
                            evt.setEvtCode(EVENT_TYPE.ESTABLISHED);
                        } else if (tempInt < callStateList.Count){
                            evt.setEvtCode(EVENT_TYPE.CONNECTIONCLEARED);
                        } else {
                            evt.setEvtCode(EVENT_TYPE.CALLEND);
                        }

                    }

                }
            }

            return evt;
        }

        private Event getDialogEvent(XmlNodeList nodeList)
        {
            CallEvent evt = null;

            if (nodeList.Count > 0)
            {
                // Call 이벤트
                evt = new CallEvent();
                
                //evt.setEvtCode(EVENT_TYPE.ON_CALL);

                XmlNode nodeone = nodeList.Item(0);
                foreach (XmlNode node1 in nodeone.ChildNodes)
                {
                    if (node1.Name.Equals("fromAddress"))
                    {
                        evt.setFromAddress(node1.InnerText.ToString());
                       // logwrite.write("### EVENT CHECK ###", "setFromAddress : " + node1.InnerText.ToString());
                    }

                    if (node1.Name.Equals("toAddress"))
                    {
                        evt.setToAddress(node1.InnerText.ToString());
                      //  logwrite.write("### EVENT CHECK ###", "setToAddress : " + node1.InnerText.ToString());
                    }

                    if (node1.Name.Equals("id"))
                    {
                        // Dialog ID 데이터 (콜을 Control 하기 위해서 이 ID 가 필요하다)
                        evt.setDialogID(node1.InnerText.ToString());
                       // logwrite.write("### EVENT CHECK ###", "setDialogID : " + node1.InnerText.ToString());
                    }
                    if (node1.Name.Equals("mediaProperties"))
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name.Equals("callType"))
                            {
                                evt.setCallType(node2.InnerText.ToString());
                              //  logwrite.write("### EVENT CHECK ###", "setCallType : " + node2.InnerText.ToString());
                            }
                            if (node2.Name.Equals("callvariables"))
                            {
                                evt.setCallVariable(getCallVarValues(node2));
                              //  logwrite.write("### EVENT CHECK ###", "setCallVariable : " + getCallVarValues(node2).ToString());
                            }
                        }
                    }
                    if (node1.Name.Equals("participants"))
                    {
                        ArrayList list = getCallState(node1);
                        for (int i = 0; i < list.Count; i++)
                        {
                            Hashtable table = (Hashtable)list[i];
                            string extension = (string)table["mediaAddress"];
                            if (extension.Equals(agent.getExtension()))
                            {
                                // 상담원 내선과 이벤트 내선이 같을 경우 이벤트 데이터에 포함시킨다.
                                //evt.setCallState((string)table["state"]);
                                evt.setEvtCode((string)table["state"]);
                              //  logwrite.write("### EVENT CHECK ###", "setCallState : " + (string)table["state"]);
                            }
                        }
                    }

                }
            }

            return evt;
        }
        public string getAttributeData(string xml, string tagName , string attributeName)
        {
            try
            {
                if (xml == null)
                {
                    return null;
                }
                logwrite.write("### getAttributeData CHECK ###", xml);

                // xml 로드
                xmlDoucment.LoadXml(xml);

                nodeList = xmlDoucment.GetElementsByTagName(tagName);

                if (nodeList.Count <= 0)
                {
                    return null;
                }

                XmlNode node = nodeList.Item(0);

                return node.Attributes[attributeName].Value;

            }
            catch (Exception e)
            {
                logwrite.write("getAttributeData", e.ToString());
                return null;
            }
        }

        public string getData(string xml, string tagName)
        {
            try
            {
                if (xml == null)
                {
                    return null;
                }
                    
                logwrite.write("### getData CHECK ###", xml);

                // xml 로드
                xmlDoucment.LoadXml(xml);

                nodeList = xmlDoucment.GetElementsByTagName(tagName);

                if (nodeList.Count <= 0)
                {
                    return null;
                }

                XmlNode node = nodeList.Item(0);

                return node.InnerText.ToString();

            }
            catch (Exception e)
            {
                logwrite.write("getData", e.ToString());
                return null;
            }
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

    }
}
