using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPSOCKET;
using ThreadGroup;


namespace CTIFnClient
{
    public abstract class Finesse
    {

        private ClientSocket FinesseClient;
        private ClientSocket AEMSClient;
        private ClientSocket ISPSClient;


        private LogWrite logwrite;


        public Finesse()
        {
            logwrite = LogWrite.getInstance();
        }

        public int fnConnect(String fn_A_IP , String fn_B_IP , String AEMS_A_IP , String AEMS_B_IP , int AEMS_Port , String ISPS_A_IP , String ISPS_B_IP , int ISPS_Port , int loglevel )
        {
            logwrite.write("fnConnect", "call fnConnect()");

            
            StringBuilder sb = new StringBuilder();
            logwrite.write("fnConnect", "Finesse A \t [" + fn_A_IP + "]");
            logwrite.write("fnConnect", "Finesse B \t [" + fn_B_IP + "]");
            logwrite.write("fnConnect", "AEMS A \t [" + AEMS_A_IP + "]");
            logwrite.write("fnConnect", "AEMS B \t [" + AEMS_B_IP + "]");
            logwrite.write("fnConnect", "AEMS Port \t [" + AEMS_Port + "]");
            logwrite.write("fnConnect", "ISPS A \t [" + ISPS_A_IP + "]");
            logwrite.write("fnConnect", "ISPS B \t [" + ISPS_B_IP + "]");
            logwrite.write("fnConnect", "ISPS Port \t [" + ISPS_Port + "]");
            logwrite.write("fnConnect", "Loglevel \t [" + loglevel + "]");

            int finesseport = SERVERINFO.Finesse_PORT;

            // 각 서버정보 객체화
            ServerInfo finesseInfo = new ServerInfo(fn_A_IP, fn_B_IP, finesseport);
            ServerInfo aemsInfo = new ServerInfo(AEMS_A_IP, AEMS_B_IP, AEMS_Port);
            ServerInfo ispsInfo = new ServerInfo(ISPS_A_IP, ISPS_B_IP, ISPS_Port);

            FinesseClient = new FinesseClient(logwrite);
            FinesseClient.setServerInfo(finesseInfo);
            if (FinesseClient.startClient() != ERRORCODE.SUCCESS)
            {
                return ERRORCODE.FAIL;
            }
            
            

            
            return 0;
        }











        public abstract void GetEventOnConnection();


        public void test()
        {
            Console.WriteLine("test");
            logwrite.write("testMethod" , "test");
        }

    }



}
