using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VO;

namespace HTTP
{
    class URLHandler
    {

        public string getLoginURL(string serverIP , Agent agent)
        {
            return "http://" + serverIP + "/finesse/api/User/" + agent.getAgentID();
        }

        public string getLogoutURL(string serverIP, Agent agent)
        {
            return "http://" + serverIP + "/finesse/api/User/" + agent.getAgentID();
        }

        public string getMakeCallURL(string serverIP, Agent agent)
        {
            return "http://" + serverIP + "/finesse/api/User/" + agent.getAgentID() + "/Dialogs";
        }
    }
}
