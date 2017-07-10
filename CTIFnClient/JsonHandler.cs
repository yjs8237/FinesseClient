using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using VO;
using System.IO;
using System.Net;
namespace JSON
{
    class JsonHandler
    {

        private PhonePad phonePadVO;
        private string extension;
        private DataContractJsonSerializer jsonSer;
        private MemoryStream stream1;

        public JsonHandler(string extension)
        {
            phonePadVO = new PhonePad();
            this.extension = extension;
            phonePadVO.setKey(extension);
        }

        public void setType(string type)
        {
            phonePadVO.setType(type);    
        }
        public void setAccount(string account)
        {
            phonePadVO.setData(account);
        }

        public void setCmd(string cmd)
        {
            phonePadVO.setCmd(cmd);
        }

        public string getJsonData()
        {
            stream1 = new MemoryStream();
            jsonSer = new DataContractJsonSerializer(typeof(PhonePad));
            jsonSer.WriteObject(stream1, phonePadVO);

            stream1.Position = 0;
            StreamReader reader = new StreamReader(stream1);

            string returnData = reader.ReadToEnd();

            stream1.Close();
            reader.Close();

            return returnData;
        }

        public PhonePad recvJson(string jsonData)
        {
            PhonePad phonePad = new PhonePad();
            stream1 = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            //jsonSer = new DataContractJsonSerializer(phonePad.GetType());
            jsonSer = new DataContractJsonSerializer(typeof(PhonePad));
            phonePad = (PhonePad)jsonSer.ReadObject(stream1);

            stream1.Close();

            return phonePad;
        }
    }
}
