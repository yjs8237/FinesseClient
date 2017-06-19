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

    
}
