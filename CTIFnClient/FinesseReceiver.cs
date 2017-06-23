using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Web;
using CTIFnClient;
using XML;
using EVENTOBJ;
using VO;
using System.Collections;

namespace ThreadGroup
{
    class FinesseReceiver : ISocketReceiver
    {

        private TcpClient sock = null;
        private NetworkStream writeStream;

        private StreamReader reader;
        private LogWrite logwrite;
        private Finesse finesseObj;

        private XMLParser xmlParser;

        private Agent agent;

        public FinesseReceiver(StreamReader reader , Finesse finesseObj)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
            this.xmlParser = new XMLParser(logwrite , null);
        }

        public FinesseReceiver(TcpClient sock, Finesse finesseObj, Agent agent)
        {
            this.sock = sock;
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            this.reader = new StreamReader(writeStream, encode);
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
            this.xmlParser = new XMLParser(logwrite , agent);
            this.agent = agent;
        }

        public void runThread()
        {
            try
            {

                logwrite.write("FinesseReceiver runThread", " Finesse Recv Thread Start !!");


                Event evt = null;

                if (writeStream == null)
                {
                    logwrite.write("FinesseReceiver runThread", "writeStream null");
                }
                
                /*
                string line;
                while ((line = reader.ReadToEnd()) != null)
                {
                    // do something with line
                    line = line.Replace("&lt;", "<");
                    line = line.Replace("&gt;", ">");
                    logwrite.write("FinesseReceiver runThread", line);
                }
                */
                
                int BUFFERSIZE = sock.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int bytelen = 0;

                StringBuilder sb = new StringBuilder();


                while ((bytelen = writeStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytelen);
                    message = message.Replace("&lt;", "<");
                    message = message.Replace("&gt;", ">");

                    message = message.Replace("\n", "");

                    logwrite.write("FinesseReceiver runThread", message.Replace("\n" , ""));

                    sb.Append(message); 

                    // XML 이 끊겨서 두번의 패킷으로 들어오는 현상 방지
                    if (!message.EndsWith("</message>"))
                    {
                        continue;   
                    }

                    string rootString = "";

                    int index = 0;

                    message = sb.ToString();
                    /*
                     * XML Root 가 한번에 두개씩 리턴되는 경우가 있어 XML 파싱이 제대로 되지 않는 현상을 방지하기 위해
                     * 
                     * */
                    logwrite.write("FinesseReceiver runThread", "## 1 ## [" + message);

                    while (true)
                    {
                        index = message.IndexOf("</message>");  // Root XML
                        int messageLen = "</message>".Length;

                        if (index < 0) { sb = new StringBuilder(); break; }

                        rootString = message.Substring(0, index + "</message>".Length);

                        logwrite.write("FinesseReceiver runThread", "## 2 ## [" + rootString);

                        // 서버로부터 받은 이벤트 XML 을 파싱한다.
                        evt = xmlParser.parseXML(rootString);
                        finesseObj.raiseEvent(evt);


                        if (rootString.Length == message.Length)
                        {
                            sb = new StringBuilder();
                            break;
                        }
                        else
                        {
                            message = message.Substring(rootString.Length, message.Length - rootString.Length);
                        }

                    }
                    
                }
        

            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (writeStream != null)
                {
                    writeStream.Close();
                    writeStream = null;
                }
                logwrite.write("FinesseReceiver runThread", e.ToString());
                logwrite.write("FinesseReceiver runThread", e.StackTrace);
            }
        }

        private string getRootDoc(string xml)
        {
            string returnStr = null;

            int index = xml.IndexOf("</message>");

            if(index > 0) {

                string tempStr = xml.Substring(0, index + "</message>".Length);

                if (tempStr.Length > 0)
                {
                    return tempStr;
                }

            }
            return returnStr;
        }


    }
}
