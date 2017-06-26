using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VO
{
    class FinesseDomain
    {
        private static volatile FinesseDomain instance;
        private static object syncRoot = new Object();

        private static object syncDomain = new Object();

        private  string domain;

        private FinesseDomain()
        {
        }

        public static FinesseDomain getInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new FinesseDomain();
                    }
                }
            }
            return instance;
        }

        public void setFinesseDomain(string domain)
        {
            lock (syncDomain)
            {
                this.domain = domain;
            }
        }

        public string getFinesseDomain()
        {
            return this.domain;
        }

    }
}
