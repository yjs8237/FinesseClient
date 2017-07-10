﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVENTOBJ
{
    class EVENT_TYPE
    {
        public const string ALERTING = "ALERTING";
        public const string ESTABLISHED = "ESTABLISHED";
        public const string DROPPED = "DROPPED";
        public const string WRAP_UP = "WRAP_UP";
        public const string ACTIVE = "ACTIVE";
        public const string FAILED = "FAILED";
        public const  string ON_AGENTSTATE_CHANGE = "ON_AGENTSTATE_CHANGE";
        public const  string ON_CONNECTION = "ON_CONNECTION";
        public const string ON_DISCONNECTION = "ON_DISCONNECTION";
        
        public const string INITIATING = "INITIATING";
        public const string INITIATED = "INITIATED";
        public const string ERROR = "ERROR";
        public const string HELD = "HELD";
        
    }
}
