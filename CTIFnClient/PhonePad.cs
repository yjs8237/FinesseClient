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
            this.data = new Hashtable();
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
        private Hashtable data;

        public void setCmd(string cmd)
        {
            this.cmd = cmd;
        }
        public string getCmd()
        {
            return this.cmd;
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
        public Hashtable getData()
        {
            return this.data;
        }

        public void setData(string key , string value)
        {
            if (this.data.ContainsKey(key))
            {
                this.data.Remove(key);
            }
            this.data.Add(key, value);
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
