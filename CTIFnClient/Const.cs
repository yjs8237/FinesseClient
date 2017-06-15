using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CONST
{
    class ERRORCODE
    {

        public readonly static int SUCCESS = 0;
        public readonly static int SOCKET_CONNECTION_FAIL = -100;
        public readonly static int FAIL = -1;
        public readonly static int LOGIN_FAIL = -3;
    }

    class CONNECTION
    {
        public readonly static int CONNECTION_TIMEOUT = 3000;
    }

    class SERVERINFO
    {
        public readonly static int Finesse_PORT = 5222;
       
    }

    class EVENT
    {
        public const int OnConnection = 100;
        public const int OnConnectionClosed = 200;
        public const int OnCallBegin = 300;
        public const int OnCallDlivered = 400;
        public const int OnCallEstablished = 500;
        public const int OnCallHeld = 600;
        public const int OnCallRetrieved = 700;
        public const int OnCallConnectionCleared = 800;
        public const int OnLoginFail = 900;
        public const int OnPasswordCheked = 1000;
        public const int OnConnectionFail = 1100;
        
    }
    
}
