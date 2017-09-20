//Add Libarys Here. I dont currently know. Will add when I have access to Visual Studio
using System;
using Android.Webkit;
using Android.Media;
using Android.Provider;
using Android.OS;
using Java.Net;
using Android.Views;
using Java.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StockApp.HTTP
{
    class HttpPost : AsyncTask<string, string , string>
    {
        public readonly string REQUEST_METHOD = "GET";
        public readonly int READ_TIMEOUT = 15000;
        public readonly int CONNECTION_TIMEOUT = 15000;
        List<tescoApiJson> rootJson = null;
        public IActivityResponse activityResponse = null;

        public MediaType JSON { get; private set; }

        public HttpPost()
        {
        }

        protected List<tescoApiJson> doHmtl(params string[] strParams)
        {
            string strUrl = strParams[0] + "Php_Landing.php";
            string requestType = strParams[1];
            string strResponse = "";
            string strPost = "";

            var strExemptions = new List<string> { "<html>", "<body>", "</body>" , "</html>" };
            URL myUrl = new URL(strUrl);
            HttpURLConnection uRLConnection = null;

            try
            {
                uRLConnection = (HttpURLConnection)myUrl.OpenConnection();
                uRLConnection.RequestMethod = "POST";
                strPost += "requestType=" + requestType + "&";
                strPost += "tableName=Stock" + "&";
                uRLConnection.DoInput = true;
                uRLConnection.DoOutput = true;
                

                if (requestType == "getSpecific")
                {
                    strPost += "Pan= " + strParams[2] + "&";
                    
                }
                else if (requestType == "addNew")
                {
                    strPost +="Pan= " +strParams[2] + "&";
                    strPost += "Amount= " + strParams[3] + "&";
                    strPost += "expiryDate= " + strParams[4] + "&";
                    strPost += "boolRemoveItem=" + strParams[5] + "&";
                }
    
                OutputStream outputPost = new BufferedOutputStream(uRLConnection.OutputStream);
                BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(uRLConnection.OutputStream, "UTF-8"));

                writer.Write(strPost);

                writer.Flush();
                writer.Close();
                uRLConnection.OutputStream.Close();

                System.Console.WriteLine("Output String : {0}" , outputPost.ToString());

                int responseCode = (int)uRLConnection.ResponseCode;

                if (responseCode == (int)HttpStatus.Ok)
                {
                    string line;

                    BufferedReader br = new BufferedReader(new InputStreamReader(uRLConnection.InputStream));

                    while((line = br.ReadLine()) != null)
                    {
                        line = Regex.Replace(line, @"\s", "");
                        if (!strExemptions.Contains(line))
                        {
                            strResponse += line.Replace("</body>", "");
                        }
                    }
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
            activityResponse.proccessFinish(rootJson);
            base.OnPostExecute(result);
            
        }

        protected override string RunInBackground(params string[] @params)
        {
            doHmtl(@params);
            return "Completed";
        }
    }
}

    
    
   
  



