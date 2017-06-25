using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VO;

namespace HTTP
{
    class URLHandler
    {

        public string getUserURL(string serverIP , Agent agent)
        {
            return "http://" + serverIP + "/finesse/api/User/" + agent.getAgentID();
        }
        public string getDialogURL(string serverIP, Agent agent)
        {
            return "http://" + serverIP + "/finesse/api/User/" + agent.getAgentID() + "/Dialogs";
        }

        public string getAnswerURL(string serverIP, Agent agent, string dialogID)
        {
            return "http://" + serverIP + "/finesse/api/Dialog/" + dialogID;
        }

        public string getCallDialogURL(string serverIP, Agent agent, string dialogID)
        {
            return "http://" + serverIP + "/finesse/api/Dialog/" + dialogID;
        }

        public string getReasonCodeURL(string serverIP, Agent agent)
        {
            return "http://" + serverIP + "/finesse/api/User/"+agent.getAgentID()+"/ReasonCodes?category=NOT_READY";
        }
      
    }
}
