using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using CTIFnClient;
using TCPSOCKET;
using CONST;
using EVENTOBJ;


namespace ThreadGroup
{
    class AEMSReceiver : ISocketReceiver
    {
        private StreamReader reader;
        private LogWrite logwrite;
        private TcpClient sock = null;
        private Finesse finesseObj;
        private NetworkStream writeStream;
        private AEMSClient aemsClient;

        public AEMSReceiver(StreamReader reader, Finesse finesseObj)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;
        }

        public AEMSReceiver(TcpClient sock, Finesse finesseObj , AEMSClient aemsClient)
        {
            this.sock = sock;
            this.finesseObj = finesseObj;
            this.logwrite = LogWrite.getInstance();
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            this.reader = new StreamReader(writeStream, encode);
            this.aemsClient = aemsClient;
        }

        public void runThread()
        {
            try
            {
                /*
                String readLine = "";

                while (true)
                {
                    readLine = reader.ReadLine();
                    logwrite.write("AEMSReceiver runThread", readLine);
                }
                 * */

                logwrite.write("AEMSReceiver runThread", "\t AEMS Receiver Thread Start!!");

                int BUFFERSIZE = sock.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int bytelen = 0;
                while ((bytelen = writeStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytelen);
                    logwrite.write("recv", message);
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
                logwrite.write("AEMSReceiver runThread", e.ToString());

            }
            finally
            {
                aemsClient.sessionClose();

                if (!aemsClient.getDisconnectReq())
                {
                    logwrite.write("AEMSReceiver runThread", "########## AEMS Session Closed !! ##########");

                    Event evt = new Event();
                    evt.setEvtCode(EVENT_TYPE.ON_DISCONNECTION);
                    evt.setEvtMsg("AEMS Session Disconnected");
                    evt.setCurAemsIP(aemsClient.getCurrentServerIP());
                    finesseObj.raiseEvent(evt);

                    if (aemsClient.reConnect() != ERRORCODE.SUCCESS)
                    {
                        // 서버 세션이 끊어지고, 재접속이 안될시 서버 프로세스가 올라올때까지 감지하는 스레드 시작한다.

                        ISocketSender aemsSender = new AEMSSender(logwrite, aemsClient);
                        ThreadStart ts = new ThreadStart(aemsSender.runThread);
                        Thread thread = new Thread(ts);
                        thread.Start();
                    }
                }
            }


        }
    }
}
