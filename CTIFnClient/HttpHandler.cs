using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using VO;
using CONST;
using System.IO;
using CTIFnClient;
using XML;

namespace HTTP
{
    class HttpHandler
    {
        
        private HttpWebRequest request;
        private WebResponse response;
        private StreamWriter writer;
        private StreamReader reader;

        private XMLHandler xmlHandler;
        private URLHandler urlHandler;

        private LogWrite logwrite;

        private string URL;
        private string serverIP;

        public HttpHandler(LogWrite logwrite)
        {
            this.logwrite = logwrite;
            this.xmlHandler = new XMLHandler();
            this.urlHandler = new URLHandler();
        }

        public int requestRESTAPI(string url ,  Agent agent ,  string methodType ,string requestData)
        {
            this.URL = url;

            try
            {

                request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = methodType;
                request.ContentType = "application/xml";
                request.ContentLength = requestData.Length;
                string basicEncode = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(agent.getAgentID() + ":" + agent.getAgentPwd()));

                
                request.Headers.Add("Authorization", "Basic " + basicEncode);

                logwrite.write("requestRESTAPI", "============================= REQUEST REST API =================================");
                logwrite.write("requestRESTAPI", "  URL \t : " + URL);
                logwrite.write("requestRESTAPI", "  DATA \t : " + requestData);
                logwrite.write("requestRESTAPI", "  basicEncode \t : " + basicEncode);
                logwrite.write("requestRESTAPI", "=================================================================================");


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

        public int loginRequest(string serverIP , Agent agent)
        {
            return requestRESTAPI(urlHandler.getLoginURL(serverIP, agent), agent , "PUT", xmlHandler.getLogin(agent.getExtension()));
        }

        public int logoutRequest(string serverIP, Agent agent)
        {
            return requestRESTAPI(urlHandler.getLogoutURL(serverIP, agent), agent ,"PUT",xmlHandler.getLogout());
        }

        public int makeCallRequest(string serverIP, Agent agent , string dialNumber)
        {
            return requestRESTAPI(urlHandler.getMakeCallURL(serverIP, agent), agent ,"POST", xmlHandler.getMakeCall(agent.getExtension(), dialNumber));
        }

    }
}
