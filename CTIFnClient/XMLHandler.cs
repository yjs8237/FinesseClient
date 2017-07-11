using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CONST;
namespace XML
{
    class XMLHandler
    {

        public string getLogin(string extension)
        {
            return "<User><state>"+CALL.LOGIN+"</state> <extension>" + extension + "</extension></User>";
        }
        public string getLogout()
        {
            return "<User><state>"+CALL.LOGOUT+"</state></User>";
        }
        public string getMakeCall(string extension , string dialNumber)
        {
            return "<Dialog><requestedAction>"+CALL.MAKE_CALL+"</requestedAction><fromAddress>" + extension + "</fromAddress><toAddress>" + dialNumber + "</toAddress></Dialog>";
        }
        public string getArsTransfer(string extension, string dialNumber)
        {
            return "<Dialog><requestedAction>" + CALL.TRANSFER_SST + "</requestedAction><toAddress>" + dialNumber + "</toAddress><targetMediaAddress>" + extension + "</targetMediaAddress></Dialog>";
        }
        public string getAnswer(string extension)
        {
            return "<Dialog><targetMediaAddress>"+extension+"</targetMediaAddress><requestedAction>"+CALL.ANSWER+"</requestedAction></Dialog>";
        }
        public string getHold(string extension)
        {
            return "<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>"+CALL.HOLD+"</requestedAction></Dialog>";
        }
        public string getRetrieve(string extension)
        {
            return "<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>"+CALL.RETRIEVE+"</requestedAction></Dialog>";
        }
        public string getRelease(string extension)
        {
            return "<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>"+CALL.DROP+"</requestedAction></Dialog>";
        }
        public string getAgentState(string state)
        {
            return "<User><state>"+state+"</state></User>";
        }
        public string getSetCallData(string varName , string varValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Dialog><requestedAction>"+CALL.UPDATE_CALL_DATA+"</requestedAction><mediaProperties><wrapUpReason>Happy customer!</wrapUpReason><callvariables>");
            sb.Append("<CallVariable>");
            sb.Append("<name>").Append(varName).Append("</name>");
            sb.Append("<value>").Append(varValue).Append("</value>");
            sb.Append("</CallVariable>");
            sb.Append("</callvariables>").Append("</mediaProperties></Dialog>");
            return sb.ToString();
        }
        public string getCCTransfer(string extension, string dialNumber)
        {
            return "<Dialog><requestedAction>"+CALL.CONSULT_CALL+"</requestedAction><toAddress>"+dialNumber+"</toAddress><targetMediaAddress>"+extension+"</targetMediaAddress></Dialog>";
        }
        public string getTransfer(string extension, string dialNumber)
        {
            return "<Dialog><requestedAction>" + CALL.TRANSFER + "</requestedAction><toAddress>" + dialNumber + "</toAddress><targetMediaAddress>" + extension + "</targetMediaAddress></Dialog>";
        }
        public string getConference(string extension, string dialNumber)
        {
            return "<Dialog><requestedAction>" + CALL.CONFERENCE + "</requestedAction><targetMediaAddress>" + extension + "</targetMediaAddress></Dialog>";
        }
        public string getCCConference(string extension, string dialNumber)
        {
            //return "<Dialog><requestedAction>" + CALL.CONFERENCE + "</requestedAction><toAddress>" + dialNumber + "</toAddress><targetMediaAddress>" + extension + "</targetMediaAddress></Dialog>";
            return "<Dialog><requestedAction>" + CALL.CONSULT_CALL + "</requestedAction><toAddress>" + dialNumber + "</toAddress><targetMediaAddress>" + extension + "</targetMediaAddress></Dialog>";
        }
        public string getAgentState(string state, string reasonCode)
        {
            return "<User><state>"+state+"</state><reasonCodeId>"+reasonCode+"</reasonCodeId></User>";
        }
    }
}
