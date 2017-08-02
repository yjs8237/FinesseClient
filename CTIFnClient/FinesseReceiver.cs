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
using TCPSOCKET;
using CONST;

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
        private FinesseClient finesseClient;

        public FinesseReceiver(StreamReader reader , Finesse finesseObj)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
            this.xmlParser = new XMLParser(logwrite , null);
        }

        public FinesseReceiver(TcpClient sock, Finesse finesseObj, Agent agent , FinesseClient finesseClient)
        {
            this.sock = sock;
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            this.reader = new StreamReader(writeStream, encode);
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
            this.xmlParser = new XMLParser(logwrite , agent);
            this.agent = agent;
            this.finesseClient = finesseClient;
        }

        public void runThread()
        {
            try
            {

                logwrite.write("FinesseReceiver runThread", " Finesse Recv Thread Start !!");

                writeStream.ReadTimeout = Timeout.Infinite;

                Event evt = null;

                if (writeStream == null)
                {
                    logwrite.write("FinesseReceiver runThread", "writeStream null");
                }
                
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
                    message = message.Trim();

                    //Console.WriteLine(message);
                    logwrite.write("FinesseReceiver runThread", message.Replace("\n", ""));

                    int endIndex = 0;
                    int subLength = 0;

                    while (message.Length > 0)
                    {
                        //Console.WriteLine("message Len : " + message.Length);
                        if (message.StartsWith("<message"))
                        {
                            endIndex = message.IndexOf("</message>");
                            if (endIndex > -1)
                            {
                                subLength = endIndex + "</message>".Length;
                                string resultStr = message.Substring(0, subLength);

                                evt = xmlParser.parseXML(resultStr);
                                finesseObj.raiseEvent(evt);
                                //Console.WriteLine("\n\n1. result -> " + resultStr);
                                message = message.Substring(subLength, message.Length - subLength);
                            }
                            else
                            {
                                sb.Append(message);
                                break;
                            }
                        }
                        else
                        {
                            endIndex = message.IndexOf("</message>");
                            if (endIndex > -1)
                            {
                                subLength = endIndex + "</message>".Length;
                                string resultStr = message.Substring(0, subLength);

                                if (sb.ToString().Length > 0)
                                {
                                    sb.Append(resultStr);
                                    resultStr = sb.ToString().Replace("&gt;", ">").Replace("&lt;", "<");
                                    evt = xmlParser.parseXML(resultStr);
                                    finesseObj.raiseEvent(evt);
                                    //Console.WriteLine("\n\n2. result -> " + sb.ToString());
                                    sb = new StringBuilder();
                                }
                                message = message.Substring(subLength, message.Length - subLength);
                            }
                            else
                            {
                                if (sb.ToString().Length > 0)
                                {
                                    sb.Append(message);
                                    break;
                                }
                                else
                                {
                                    sb = new StringBuilder();
                                    break;
                                }
                            }
                        }
                    }

                }
                


                /*
                while ((bytelen = writeStream.Read(buffer, 0, buffer.Length)) > 0)
                {

                    string message = Encoding.UTF8.GetString(buffer, 0, bytelen);
                    message = message.Replace("&lt;", "<");
                    message = message.Replace("&gt;", ">");

                    message = message.Replace("\n", "");

                    logwrite.write("FinesseReceiver runThread", message.Replace("\n", ""));

                    int startIndex = message.IndexOf("<message");
                    int endIndex = message.IndexOf("</message>");

                    int endTaglen = "</message>".Length;
                    int messagelen = message.Length;

                    string tempStr = "";

                    if (startIndex > -1)
                    {
                        if (endIndex > -1)
                        {
                            sb.Append(message.Substring(startIndex, endIndex + endTaglen - startIndex));
                            evt = xmlParser.parseXML(sb.ToString());
                            finesseObj.raiseEvent(evt);
                            //Console.WriteLine("result -> " + sb.ToString());
                            sb = new StringBuilder();
                            if (message.Length > endIndex + endTaglen)
                            {
                                // 완료 XML 뒤에 데이터가 있는경우
                                int start = endIndex + endTaglen;
                                tempStr = message.Substring(start, messagelen - start);

                                startIndex = tempStr.IndexOf("<message");
                                endIndex = tempStr.IndexOf("</message>");

                                if (startIndex > -1)
                                {
                                    if (endIndex > -1)
                                    {
                                        // 완료 XML 이 뒤에 또 붙은 경우
                                        start = endIndex + endTaglen;
                                        tempStr = tempStr.Substring(startIndex, start - startIndex);
                                        sb.Append(tempStr);
                                        evt = xmlParser.parseXML(sb.ToString());
                                        finesseObj.raiseEvent(evt);
                                        //Console.WriteLine("result -> " + sb.ToString());
                                        sb = new StringBuilder();
                                    }
                                    else
                                    {
                                        // 완료 XML 이 아닌경우
                                        tempStr = tempStr.Substring(startIndex, tempStr.Length);
                                        sb.Append(tempStr);
                                    }
                                }

                            }

                        }
                        else
                        {
                            // 마지막 <message>이 또 붙어서 XML 이 끝이 나지 않은 경우
                            tempStr = message.Substring(startIndex, messagelen - startIndex);
                            sb = new StringBuilder();
                            sb.Append(tempStr);
                        }
                    }
                    else if (endIndex > -1)
                    {
                        tempStr = message.Substring(0, endIndex + endTaglen);
                        sb.Append(tempStr);
                        evt = xmlParser.parseXML(sb.ToString());
                        finesseObj.raiseEvent(evt);
                        //Console.WriteLine("result -> " + sb.ToString());
                        sb = new StringBuilder();
                    }

                }
                */

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

            }
            finally
            {

                finesseClient.sessionClose();
                finesseClient.setXMPPAuth(false);   // Finesse 세션이 끊어지면 다른 서버로 재접속 할떄 XMPP 인증이 필요하기 때문에 Flag 세팅

                // 사용자가 Disconnect 를 요청하지 않고 세션이 끊어진 경우 재접속 시도
                if (!finesseClient.getDisconnectReq())
                {
                    logwrite.write("FinesseReceiver runThread", "########## Finesse Session Closed !! ##########");

                    Event evt = new ErrorEvent();
                    evt.setEvtCode(EVENT_TYPE.ON_DISCONNECTION);
                    evt.setEvtMsg("Finesse Session Disconnected");
                    evt.setCurFinesseIP(finesseClient.getCurrentServerIP());
                    finesseObj.raiseEvent(evt);

                    if (finesseClient.finesseReConnect() == ERRORCODE.SUCCESS)
                    {
                        logwrite.write("FinesseReceiver runThread", " TRY TO CHECK AGENT PREVIOUS STATE");
                        // XMPP 인증 성공하면 이전 상담원 상태를 가져온다.
                        finesseClient.checkAgentState();
                    }
                    else
                    {
                        // 서버 세션이 끊어지고, 재접속이 안될시 서버 프로세스가 올라올때까지 감지하는 스레드 시작한다.
                        ISocketSender finesseSender = new FinesseSender(logwrite, finesseClient);
                        ThreadStart ts = new ThreadStart(finesseSender.runThread);
                        Thread thread = new Thread(ts);
                        thread.Start();
                    }
                }
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
