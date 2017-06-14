using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CTIFnClient
{
    class CallEvent : EventArgs
    {
        private String message;

        public void setMessage(String msg)
        {
            this.message = msg;
        }

        public String getMessage()
        {
            return this.message;
        }

    }
}
