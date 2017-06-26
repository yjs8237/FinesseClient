using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VO
{
    public sealed class Agent
    {
        private static volatile Agent instance;
        private static object syncRoot = new Object();

        private static object syncHashTable = new Object();

        private Hashtable agentInfoTable;


        private Agent()
        {
            agentInfoTable = new Hashtable();
        }

        public static Agent getInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new Agent();
                    }
                }
            }
            return instance;
        }

        public void setAgentID(String agentID)
        {
            lock (syncHashTable)
            {
                if (agentInfoTable.ContainsKey("ID"))
                {
                    agentInfoTable.Remove("ID");
                }
                this.agentInfoTable.Add("ID", agentID);
            }
        }
        public String getAgentID()
        {
            lock (syncHashTable)
            {
                return (string) this.agentInfoTable["ID"];
            }
        }

        public void setAgentPwd(String agentPwd)
        {
            lock (syncHashTable)
            {
                if (agentInfoTable.ContainsKey("PWD"))
                {
                    agentInfoTable.Remove("PWD");
                }
                this.agentInfoTable.Add("PWD", agentPwd);
            }
        }

        public String getAgentPwd()
        {
            lock (syncHashTable)
            {
                return (string)this.agentInfoTable["PWD"];
            }
        }

        public void setExtension(String extension)
        {
            lock (syncHashTable)
            {
                if (agentInfoTable.ContainsKey("EXTENSION"))
                {
                    agentInfoTable.Remove("EXTENSION");
                }
                this.agentInfoTable.Add("EXTENSION", extension);
            }
        }

        public String getExtension()
        {
            lock (syncHashTable)
            {
                return (string)this.agentInfoTable["EXTENSION"];
            }
        }


    }
}
