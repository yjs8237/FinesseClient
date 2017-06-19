using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;


namespace CTIFnClient
{
    class ServerInfo
    {

        private ArrayList ipArrList;
        private int port;
        private string serverDomain;

        public ServerInfo(String aIP, String bIP, int port)
        {
            ipArrList = new ArrayList();
            ipArrList.Add(aIP);
            ipArrList.Add(bIP);
            this.port = port;
        }

        public ServerInfo(String aIP, String bIP, int port, string domain)
        {
            ipArrList = new ArrayList();
            ipArrList.Add(aIP);
            ipArrList.Add(bIP);
            this.port = port;
            this.serverDomain = domain;
        }

        public ServerInfo(String aIP, String bIP, String cIP,int port)
        {
            ipArrList = new ArrayList();
            ipArrList.Add(aIP);
            ipArrList.Add(bIP);
            ipArrList.Add(cIP);
            this.port = port;
        }

        public ServerInfo(String aIP, String bIP, String cIP, String dIP , int port)
        {
            ipArrList = new ArrayList();
            ipArrList.Add(aIP);
            ipArrList.Add(bIP);
            ipArrList.Add(cIP);
            ipArrList.Add(dIP);
            this.port = port;
           
        }



        public ArrayList getIPList()
        {
            return this.ipArrList;
        }

        public int getPort()
        {
            return this.port;
        }

        public string getDomain()
        {
            return this.serverDomain;
        }


    }
}
