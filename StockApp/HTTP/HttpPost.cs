using System;
using Android.OS;
using Java.Net;
using Java.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Android.Content;

namespace StockApp.HTTP
{
    class HttpPost : AsyncTask<string, string , string>
    {
        public readonly string REQUEST_METHOD = "GET";
        public readonly int READ_TIMEOUT = 15000;
        public readonly int CONNECTION_TIMEOUT = 15000;
        public Context mContext { get; set; }
        List<tescoApiJson> rootJson = null;
        public IActivityResponse activityResponse = null;
        string strPost = "";
        private string idToken;
        private StockAppApplicaiton appApplicaiton = StockAppApplicaiton.getconfig();

        public HttpPost()
        {
        }

        private HttpURLConnection openConnection(string url, string urlHeader, string requestType, string httpType , HttpURLConnection uRLConnection)
        {
            string strUrl;
            strUrl = url + urlHeader;
            URL myUrl = new URL(strUrl);
            uRLConnection = (HttpURLConnection)myUrl.OpenConnection();
            uRLConnection.DoInput = true;
            

            if (httpType == "POST")
            {
                uRLConnection.RequestMethod = "POST";
                strPost += "requestType=" + requestType + "&";
                strPost += "tableName=Stock" + "&";
                uRLConnection.DoOutput = true;
            }      
            else
            {
                uRLConnection.RequestMethod = "GET";
            }
            return uRLConnection;
        }

        protected List<tescoApiJson> doHmtl(params string[] strParams)
        {
            string requestType = strParams[1];
            string strResponse = "";
            string httpType = "";

            if (requestType != "tescoData")
            {
                if (appApplicaiton.acct == null)
                {
                    return null;
                }
                else
                {
                    idToken = appApplicaiton.acct.IdToken;
                    strPost += "idToken=" + idToken + "&";
                }
            }

            HttpURLConnection uRLConnection = null;

            try
            {               
                if (requestType == "getSpecific" || requestType == "removeSpecific")
                {
                    httpType = "POST";
                    uRLConnection = openConnection(strParams[0], "Php_Landing.php", requestType, httpType, uRLConnection);
                    strPost += "Pan= " + strParams[2] + "&";
                }
                else if (requestType == "addNew")
                {
                    httpType = "POST";
                    uRLConnection = openConnection(strParams[0], "Php_Landing.php", requestType, httpType, uRLConnection);
                    strPost +="Pan= " +strParams[2] + "&";
                    strPost += "Amount= " + strParams[3] + "&";
                    strPost += "expiryDate= " + strParams[4] + "&";
                    strPost += "boolRemoveItem=" + strParams[5] + "&";
                }
                else if (requestType == "tescoData")
                {
                    httpType = "GET";
                    string urlHeader = "?gtin=" + strParams[2];
                    uRLConnection = openConnection(strParams[0], urlHeader, requestType, httpType, uRLConnection);
                    uRLConnection.SetRequestProperty("Ocp-Apim-Subscription-Key","b775ff6e5f284518851939473019dd7a");
                }
                else if (requestType == "getAll")
                {
                    httpType = "POST";
                    uRLConnection = openConnection(strParams[0], "Php_Landing.php", requestType, httpType, uRLConnection);
                }

                if (httpType == "POST")
                {
                    OutputStream outputPost = new BufferedOutputStream(uRLConnection.OutputStream);
                    BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(uRLConnection.OutputStream, "UTF-8"));

                    writer.Write(strPost);

                    writer.Flush();
                    writer.Close();
                    uRLConnection.OutputStream.Close();

                    System.Console.WriteLine("Output String : {0}", outputPost.ToString());
                }
                

                int responseCode = (int)uRLConnection.ResponseCode;

                if (responseCode == (int)HttpStatus.Ok)
                {
                    string line;
                    BufferedReader br = new BufferedReader(new InputStreamReader(uRLConnection.InputStream));
                    if (requestType == "tescoData")
                    {
                        strResponse += "[";
                    }
                    while((line = br.ReadLine()) != null)
                    {
                        strResponse += line;
                    }
                    if (requestType == "tescoData")
                    {
                        strResponse += "]";
                    }
                }
                else if(responseCode == (int)HttpStatus.Unauthorized)
                {
                    strResponse = "[{'Error': 'Unauthorized User' }]";
                }
                else
                {
                    strResponse = "Nothing Received";
                } 
                strResponse = Regex.Unescape(strResponse);
                strResponse = strResponse.Replace(@"\", "");

                rootJson = JsonConvert.DeserializeObject<List<tescoApiJson>>(strResponse);
            }
            catch(MalformedURLException error)
            {
                System.Console.WriteLine("Malformed URL: {0} ", error);
            }
            catch(SocketTimeoutException error)
            {
                System.Console.WriteLine("Socket Timeout: {0} ", error);
            }
            catch(Exception error)
            {
                System.Console.WriteLine("Error : {0}", error);
            }
            finally
            {
                uRLConnection.Disconnect();
            }

            return rootJson;
            
        }

        protected override void OnPostExecute(string result)
        {
            base.OnPostExecute(result);
            activityResponse.proccessFinish(rootJson);
        }

        protected override string RunInBackground(params string[] @params)
        {
            doHmtl(@params);
            return "Completed";
        }
    }
}

    
    
   
  



