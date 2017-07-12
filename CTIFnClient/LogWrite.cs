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

        private object lockObject = new object();

        private static int logDays = 14;

        private LogWrite() {

            deleteOldFiles();

            filepath = filepath + DateTime.Now.ToString("yyyyMMdd") ;

            fileName = filepath + @"\client_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log";

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            
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

        private void deleteOldFiles()
        {
            DirectoryInfo logFolder = new DirectoryInfo(filepath);
            try
            {
                foreach (DirectoryInfo dir in logFolder.GetDirectories())
                {
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        if (file.Extension != ".log")
                        {
                            continue;
                        }
                        // 14 일지난 로그파일 삭제
                        if (file.CreationTime < DateTime.Now.AddDays(-(logDays)))
                        {
                            file.Delete();
                        }

                    }

                    if (dir.CreationTime < DateTime.Now.AddDays(-(logDays)))
                    {
                        dir.Delete();
                    }
                }
            }
            catch (Exception e)
            {

            }
         
        }


        public  void write(String methodName, String msg)
        {
            try
            {
                lock (lockObject)
                {

                    if (File.Exists(fileName))
                    {
                        fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                    }
                    else
                    {
                        fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
                    }
                    sw = new StreamWriter(fs, Encoding.Default);


                    if (methodName.Length == 0 && msg.Length == 0)
                    {
                        sw.WriteLine("");
                        sw.Flush();
                        return;
                    }

                    String nowTime = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss:fff");

                    StringBuilder sb = new StringBuilder();
                    methodName = String.Format("{0,-20}", methodName);
                    
                    sb.Append("[").Append(nowTime).Append("][").Append(methodName).Append("]");
                    sb.Append("\t").Append(msg);

                    if (msg.Contains("Event") || msg.Contains("EVENT"))
                    {
                        //sw.WriteLine("");
                    }
                    if (methodName.Equals("FinesseReceiver runThread"))
                    {
                        sw.WriteLine("");
                        sw.WriteLine("");
                    }
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                sw.Close();
            }
            
        }


    }
}
