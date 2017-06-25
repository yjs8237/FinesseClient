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
        private HttpWebResponse response;
        private StreamWriter writer;
        private StreamReader reader;

        private XMLHandler xmlHandler;
        private URLHandler urlHandler;

        private LogWrite logwrite;

        private string URL;
        //private string serverIP;

        public HttpHandler(LogWrite logwrite)
        {
            this.logwrite = logwrite;
            this.xmlHandler = new XMLHandler();
            this.urlHandler = new URLHandler();
        }

        public string requestGETAPI(string url, Agent agent, string methodType)
        {
            this.URL = url;

            try
            {

                request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = methodType;

                string basicEncode = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(agent.getAgentID() + ":" + agent.getAgentPwd()));

                request.Headers.Add("Authorization", "Basic " + basicEncode);

                logwrite.write("requestRESTAPI", "============================= REQUEST REST API =================================");
                logwrite.write("requestRESTAPI", "  URL \t : " + URL);
                logwrite.write("requestRESTAPI", "  METHOD \t : " + methodType);
                logwrite.write("requestRESTAPI", "  basicEncode \t : " + basicEncode);

                response = (HttpWebResponse)request.GetResponse();
                int code = Convert.ToInt32(response.StatusCode);


                Stream webStream = response.GetResponseStream();
                reader = new StreamReader(webStream);

                string responseStr = reader.ReadToEnd();

                logwrite.write("requestRESTAPI", "============================= RESPONSE REST API =================================");
                logwrite.write("requestRESTAPI", "  code \t : " + code);
                logwrite.write("requestRESTAPI", "  DATA \t : " + responseStr);
                logwrite.write("requestRESTAPI", "=================================================================================");

                reader.Close();

                return responseStr;

            }
            catch (Exception e)
            {
                logwrite.write("requestRESTAPI", e.ToString());
                return null;
            }

        }


        public int requestRESTAPI(string url ,  Agent agent ,  string methodType ,string requestData)
        {
            this.URL = url;

            try
            {

                request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = methodType;
                if (!methodType.Equals("GET"))
                {
                    request.ContentType = "application/xml";
                    request.ContentLength = requestData.Length;
                }
                string basicEncode = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(agent.getAgentID() + ":" + agent.getAgentPwd()));

                request.Headers.Add("Authorization", "Basic " + basicEncode);

                logwrite.write("requestRESTAPI", "============================= REQUEST REST API =================================");
                logwrite.write("requestRESTAPI", "  URL \t : " + URL);
                logwrite.write("requestRESTAPI", "  METHOD \t : " + methodType);
                logwrite.write("requestRESTAPI", "  DATA \t : " + requestData);
                logwrite.write("requestRESTAPI", "  basicEncode \t : " + basicEncode);

                if (!methodType.Equals("GET"))
                {
                    writer = new StreamWriter(request.GetRequestStream());
                    writer.Write(requestData);
                    writer.Close();
                }

                response = (HttpWebResponse) request.GetResponse();
                int code = Convert.ToInt32(response.StatusCode);
               

                Stream webStream = response.GetResponseStream();
                reader = new StreamReader(webStream);

                string responseStr = reader.ReadToEnd();

                logwrite.write("requestRESTAPI", "============================= RESPONSE REST API =================================");
                logwrite.write("requestRESTAPI", "  code \t : " + code);
                logwrite.write("requestRESTAPI", "  DATA \t : " + responseStr);
                logwrite.write("requestRESTAPI", "=================================================================================");

                reader.Close();


            }
            catch (Exception e)
            {
                logwrite.write("requestRESTAPI", e.ToString());
                return ERRORCODE.FAIL;
            }

            return ERRORCODE.SUCCESS;
        }


        /*
         *  GET 방식 
         * */
        public string checkAgentState(string serverIP, Agent agent)
        {
            return requestGETAPI(urlHandler.getUserURL(serverIP, agent), agent, "GET" );
        }

        public string reasonCodeRequest(string serverIP, Agent agent)
        {
            return requestGETAPI(urlHandler.getReasonCodeURL(serverIP, agent), agent, "GET");
        }

        /*
        *  POST , PUT 방식 
        * */

        public int loginRequest(string serverIP , Agent agent)
        {
            return requestRESTAPI(urlHandler.getUserURL(serverIP, agent), agent , "PUT", xmlHandler.getLogin(agent.getExtension()));
        }

        public int logoutRequest(string serverIP, Agent agent)
        {
            return requestRESTAPI(urlHandler.getUserURL(serverIP, agent), agent, "PUT", xmlHandler.getLogout());
        }
        public int setCalldataRequest(string serverIP, Agent agent, string varName, string varValue, string dialogID)
        {
            return requestRESTAPI(urlHandler.getCallDialogURL(serverIP, agent, dialogID), agent, "PUT", xmlHandler.getSetCallData(varName, varValue));
        }

        

        public int makeCallRequest(string serverIP, Agent agent , string dialNumber)
        {
            return requestRESTAPI(urlHandler.getDialogURL(serverIP, agent), agent ,"POST", xmlHandler.getMakeCall(agent.getExtension(), dialNumber));
        }

        public int ccTransferRequest(string serverIP, Agent agent, string dialNumber, string dialogID)
        {
            return requestRESTAPI(urlHandler.getCallDialogURL(serverIP, agent, dialogID), agent, "PUT", xmlHandler.getCCTransfer(agent.getExtension(), dialNumber));
        }

        public int answerRequest(string serverIP, Agent agent, string dialogID)
        {
            return requestRESTAPI(urlHandler.getAnswerURL(serverIP, agent, dialogID), agent, "PUT", xmlHandler.getAnswer(agent.getExtension()));
        }

        public int holdRequest(string serverIP, Agent agent, string dialogID)
        {
            return requestRESTAPI(urlHandler.getCallDialogURL(serverIP, agent, dialogID), agent, "PUT", xmlHandler.getHold(agent.getExtension()));
        }
        public int retrieveRequest(string serverIP, Agent agent, string dialogID)
        {
            return requestRESTAPI(urlHandler.getCallDialogURL(serverIP, agent, dialogID), agent, "PUT", xmlHandler.getRetrieve(agent.getExtension()));
        }

        public int releaseRequest(string serverIP, Agent agent, string dialogID)
        {
            return requestRESTAPI(urlHandler.getAnswerURL(serverIP, agent, dialogID), agent, "PUT", xmlHandler.getRelease(agent.getExtension()));
        }

        public int agentStateChangeRequest(string serverIP, Agent agent, string state)
        {
            return requestRESTAPI(urlHandler.getUserURL(serverIP, agent), agent, "PUT", xmlHandler.getAgentState(state));
        }

        public int agentStateChangeRequest(string serverIP, Agent agent, string state, string reasonCode)
        {
            return requestRESTAPI(urlHandler.getUserURL(serverIP, agent), agent, "PUT", xmlHandler.getAgentState(state, reasonCode));
        }

    }
}
