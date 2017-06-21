using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XML
{
    class XMLHandler
    {

        public string getLogin(string extension)
        {
            return "<User><state>LOGIN</state> <extension>" + extension + "</extension></User>";
        }

        public string getLogout()
        {
            return "<User><state>LOGOUT</state></User>";
        }

        public string getMakeCall(string extension , string dialNumber)
        {
            return "<Dialog><requestedAction>MAKE_CALL</requestedAction><fromAddress>" + extension + "</fromAddress><toAddress>" + dialNumber + "</toAddress></Dialog>";
        }

        public string getAnswer(string extension)
        {
            return "<Dialog><targetMediaAddress>"+extension+"</targetMediaAddress><requestedAction>ANSWER</requestedAction></Dialog>";
        }
        public string getRelease(string extension)
        {
            return "<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>DROP</requestedAction></Dialog>";
        }

        public string getAgentState(string state)
        {
            return "<User><state>"+state+"</state></User>";
        }

        public string getAgentState(string state, string reasonCode)
        {
            return "<User><state>"+state+"</state><reasonCodeId>"+reasonCode+"</reasonCodeId></User>";
        }
    }
}
