using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVENTOBJ
{
    class ErrorEvent : Event
    {
        private string errorMessage;
        private string errorType;
        private string serverType;


        public void setServerType(string serverType)
        {
            this.serverType = serverType;
        }
        public string getServerType()
        {
            return this.serverType;
        }
        public void setErrorMessage(string message) {
            this.errorMessage = message;
        }
        public string getErrorMessage()
        {
            return errorMessage;
        }

        public void setErrorType(string errorType)
        {
            this.errorType = errorType;
        }
        public string getErrorType()
        {
            return errorType;
        }

    }
}
