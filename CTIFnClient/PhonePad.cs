using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace VO
{
    [DataContract]
    class PhonePad
    {

        public PhonePad()
        {
            this.data = new ArrayList();
            this.classType = "PhonePadInfo";
        }
        [DataMember]
        private string cmd;
        [DataMember]
        private string key;
        [DataMember]
        private string type;
        [DataMember]
        private string ret;
        [DataMember]
        private ArrayList data;
        [DataMember]
        private string classType;


        public void setCmd(string cmd)
        {
            this.cmd = cmd;
        }
        public string getCmd()
        {
            return this.cmd;
        }
        public void setClassType(string classType)
        {
            this.classType = classType;
        }
        public string getClassType()
        {
            return this.classType;
        }

        public void setKey(string key)
        {
            this.key = key;
        }
        public string getKey()
        {
            return this.key;
        }

        public void setType(string type)
        {
            this.type = type;
        }
        public string getType()
        {
            return this.type;
        }
        public ArrayList getData()
        {
            return this.data;
        }

        public void setData(string value)
        {
            this.data.Add(value);
        }

        public void setRet(string ret)
        {
            this.ret = ret;
        }
        public string getRet()
        {
            return this.ret;
        }

    }
}
