using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VO
{
    public class Agent
    {
        private String agentID;
        private String agentPwd;
        private String extension;

        public Agent(String agentID, String agentPwd, String extension , String peripheral)
        {
            this.agentID = agentID;
            this.agentPwd = agentPwd;
            this.extension = extension;
        }

        public void setAgentID(String agentID)
        {
            this.agentID = agentID;
        }
        public String getAgentID()
        {
            return this.agentID;
        }

        public void setAgentPwd(String agentPwd)
        {
            this.agentPwd = agentPwd;
        }

        public String getAgentPwd()
        {
            return this.agentPwd;
        }

        public void setExtension(String extension)
        {
            this.extension = extension;
        }

        public String getExtension()
        {
            return this.extension;
        }


    }
}
