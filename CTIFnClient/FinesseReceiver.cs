using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using CTIFnClient;
using XML;
using EVENTOBJ;


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

        public FinesseReceiver(StreamReader reader , Finesse finesseObj)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
            this.xmlParser = new XMLParser(logwrite);
        }

        public FinesseReceiver(TcpClient sock, Finesse finesseObj)
        {
            this.sock = sock;
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            this.reader = new StreamReader(writeStream, encode);
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
            this.xmlParser = new XMLParser(logwrite);
        }

        public void runThread()
        {
            try
            {

                logwrite.write("FinesseReceiver runThread", " Finesse Recv Thread Start !!");

                /*
                if (reader == null)
                {
                    logwrite.write("FinesseReceiver runThread", "reader null");
                }
                
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // do something with line
                    logwrite.write("FinesseReceiver runThread", line);
                }
                */

                IEvent evt = null;

                int BUFFERSIZE = sock.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int bytelen = 0;
                while ((bytelen = writeStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytelen);
                    logwrite.write("recv", message);

                    // 서버로부터 받은 이벤트 XML 을 파싱한다.
                    evt = xmlParser.parseXML(message);

                    callRaiseEvent(evt);
                }

            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                logwrite.write("FinesseReceiver runThread", e.ToString());
                logwrite.write("FinesseReceiver runThread", e.StackTrace);
            }
        }



        private void callRaiseEvent(IEvent evt)
        {
            // 이벤트 발생 로직 구현 

            int evtCode = evt.getEventCode();
            string evtMsg = evt.getEventMsg();

            switch (evtCode)
            {
                case EVENT.OnError:
                    finesseObj.GetEventOnError(evtMsg);
                    logwrite.write("callRaiseEvent", "GetEventOnError");
                    logwrite.write("callRaiseEvent", evtMsg);
                    break;
            }

            //finesseObj.GetEventOnCallBegin(msg);
        }
       

    }
}
