using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using VO;
using CONST;
using System.IO;

namespace HTTP
{
    class HttpHandler
    {
        private Agent agent;

        private HttpWebRequest request;
        private WebResponse response;
        private StreamWriter writer;
        private StreamReader reader;

        private string URL;
        private string serverIP;

        public HttpHandler(string serverIP , Agent agent)
        {
            //http://192.168.230.134/finesse/api/User/7112
            this.serverIP = serverIP;
            this.agent = agent;
            this.URL = "http://" + serverIP + "/finesse/api/User/" + agent.getAgentID();
        }

        public int requestRESTAPI(string requestData)
        {
            try
            {
                request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "PUT";
                request.ContentType = "application/xml";
                request.ContentLength = requestData.Length;

                request.Headers.Add("Authorization", "Basic " + agent.getAgentID() + agent.getAgentPwd());

                writer = new StreamWriter(request.GetRequestStream());
                writer.Write(requestData);
                writer.Close();


                response = request.GetResponse();
                Stream webStream = response.GetResponseStream();
                reader = new StreamReader(webStream);

                string responseStr = reader.ReadToEnd();



                reader.Close();


            }
            catch (Exception e)
            {

            }

            return ERRORCODE.SUCCESS;
        }

    }
}
