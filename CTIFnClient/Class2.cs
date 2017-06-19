using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;


namespace CTIFnClient
{
    public sealed class LogWrite
    {
        private static volatile LogWrite instance;
        private static object syncRoot = new Object();

        private String filepath = @"C:\isiFnClient\";
        private String fileName;
        private FileStream fs;
        private StreamWriter sw;

        private LogWrite() {

            fileName = @"C:\isiFnClient\" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log";

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            if(File.Exists(fileName)) {
                fs = new FileStream(fileName, FileMode.Append , FileAccess.Write);
            } else {
                fs = new FileStream(fileName, FileMode.CreateNew , FileAccess.Write);
            }
            sw = new StreamWriter(fs, Encoding.Default);
            
        }

        
        public static LogWrite getInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new LogWrite();
                    }
                }
            }
            return instance;
        }

        public void write(String methodName, String msg)
        {

            String nowTime = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss:fff");

            StringBuilder sb = new StringBuilder();
            sb.Append("[").Append(nowTime).Append("][").Append(methodName).Append("]");
            sb.Append(msg);

            sw.WriteLine(sb.ToString());
            sw.Flush();

            
        }


    }
}
