using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using EVENTOBJ;
using CTIFnClient;

namespace XML
{
    class XMLParser
    {
        private ArrayList list;
        private XmlDocument xmlDoucment;
        private XmlNodeList nodeList;
        private LogWrite logwrite;

        private IEvent evt;

        public XMLParser(LogWrite logwrite)
        {
            list = new ArrayList();
            xmlDoucment = new XmlDocument();
            this.logwrite = logwrite;
        }

        public IEvent parseXML(string xml)
        {
            
            try
            {
                if (xml == null)
                {
                    return null;
                }
                // xml 로드
                xmlDoucment.LoadXml(xml);

                nodeList = xmlDoucment.GetElementsByTagName("Dialog");
                if (nodeList.Count > 0)
                {
                    return parseDialogXML(xml);
                }
                

                // 에러 
                nodeList = xmlDoucment.GetElementsByTagName("ApiErrors");
                if (nodeList.Count > 0)
                {
                    return parseErrorXML(xml);
                }

                // Agent State Change
                nodeList = xmlDoucment.GetElementsByTagName("state");
                if (nodeList.Count > 0)
                {
                    return parseStateChangeXML(xml);
                }
            }
            catch (Exception e)
            {
                logwrite.write("parseXML", e.StackTrace);
                logwrite.write("parseXML", e.ToString());
            }

            return evt;
        }

        private IEvent parseStateChangeXML(string xml)
        {
            evt = new Evt();
            evt.setEventMsg(xml);
            evt.setEventCode(EVENT.OnAgentStateChange);
            nodeList = xmlDoucment.GetElementsByTagName("state");
            XmlNode node = nodeList.Item(0);
            evt.setAgentState(node.InnerText.ToString());
            return evt;
        }
        private IEvent parseErrorXML(string xml)
        {
            evt = new Evt();
            evt.setEventMsg(xml);
            evt.setEventCode(EVENT.OnError);

            return evt;
        }
       
        private IEvent parseDialogXML(string xml)
        {
             evt = new Evt();
            evt.setEventMsg(xml);
            evt.setEventCode(EVENT.OnCallBegin);

            nodeList = xmlDoucment.GetElementsByTagName("id");

            foreach (XmlNode node in nodeList)
            {
                if (node.Name.Equals("id"))
                {
                    evt.setDialogID(node.InnerText.ToString());
                }
            }
            return evt;
        }


    }
}
