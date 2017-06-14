using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using CTIFnClient;

namespace ThreadGroup
{
    class FinesseReceiver : ISocketReceiver
    {
        private StreamReader reader;
        private LogWrite logwrite;
        private Finesse finesseObj;

        public FinesseReceiver(StreamReader reader , Finesse finesseObj)
        {
            this.reader = reader;
            this.logwrite = LogWrite.getInstance();
            this.finesseObj = finesseObj;   // Finesse 로 부터 받은 콜 관련 데이터 이벤트 콜백 호출을 위한 객체
        }

        public void runThread()
        {
            try
            {
                String readLine = "";

                while (true)
                {
                    readLine = reader.ReadLine();
                    logwrite.write("FinesseReceiver runThread", readLine);
                }

            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                logwrite.write("FinesseReceiver runThread", e.StackTrace);
            }
        }



        private void callRaiseEvent(String msg)
        {
            // 이벤트 발생 로직 구현 


            finesseObj.GetEventOnCallBegin(msg);
        }
       

    }
}
