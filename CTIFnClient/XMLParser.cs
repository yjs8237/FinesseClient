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

        public XMLParser(LogWrite logwrite)
        {
            list = new ArrayList();
            xmlDoucment = new XmlDocument();
            this.logwrite = logwrite;
        }

        public IEvent parseXML(string xml)
        {
            IEvent evt = null;

            try
            {
                if (xml == null)
                {
                    return null;
                }
                // xml 로드
                xmlDoucment.LoadXml(xml);

                evt = new Evt();
                evt.setEventMsg(xml);

                // 에러 
                nodeList = xmlDoucment.GetElementsByTagName("ApiErrors");
                if (nodeList.Count > 0)
                {
                    evt.setEventCode(EVENT.OnError);
                    return evt;
                }

                // Agent State Change
                nodeList = xmlDoucment.GetElementsByTagName("Update");
                if (nodeList.Count > 0)
                {
                    evt.setEventCode(EVENT.OnAgentStateChange);
                    return evt;
                }
            }
            catch (Exception e)
            {
                logwrite.write("parseXML", e.StackTrace);
                logwrite.write("parseXML", e.ToString());
            }

            return evt;
        }



    }
}
