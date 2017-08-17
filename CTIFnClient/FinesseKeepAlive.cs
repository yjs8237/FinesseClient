using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using CTIFnClient;
using TCPSOCKET;
using System.Collections;
using CONST;
using VO;


namespace ThreadGroup
{
    class FinesseKeepAlive : ISocketSender
    {

        private LogWrite logwrite;
        private TcpClient sock = null;
        private FinesseClient finesseClient;

        private NetworkStream writeStream;
        private StreamReader reader;
        private StreamWriter writer;

        private Agent agent;

        public FinesseKeepAlive(TcpClient sock, Agent agent, FinesseClient finesseClient)
        {
            this.sock = sock;
            this.writeStream = sock.GetStream();
            Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
            writer = new StreamWriter(writeStream);
            this.finesseClient = finesseClient;
            this.logwrite = LogWrite.getInstance();
            this.agent = agent;
        }

        public void runThread()
        {
            logwrite.write("Finesse FinesseKeepAlive", "Finesse FinesseKeepAlive Thread Start!!");

            try
            {
                while (true)
                {
                    Thread.Sleep(10000);
                    string agentID = agent.getAgentID();
                    string serverIP = finesseClient.getCurrentServerIP();

                    FinesseDomain domainVO = FinesseDomain.getInstance();
                    string domain = domainVO.getFinesseDomain();

                    string strMsg = @"<iq id='" + agentID + "@" + serverIP + "/pidgin' to='" + domain + "' type='get' from='" + agentID + "@" + serverIP + "/pidgin'><ping xmlns='urn:xmpp:ping'/></iq>";

                    logwrite.write("Finesse FinesseKeepAlive", "SEND -> " + strMsg);
                    writer.WriteLine(strMsg);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (sock != null)
                {
                    sock.Close();
                }
            }

        }
    }
}
