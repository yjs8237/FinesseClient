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
    }
}
