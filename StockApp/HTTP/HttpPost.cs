using System;
using Android.OS;
using Java.Net;
using Java.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Android.Content;
using System.Collections.ObjectModel;

namespace StockApp.HTTP
{
    class HttpPost : AsyncTask<string, string , string>
    {
        public readonly string REQUEST_METHOD = "GET";
        public readonly int READ_TIMEOUT = 15000;
        public readonly int CONNECTION_TIMEOUT = 15000;
        public Context mContext { get; set; }
        ObservableCollection<tescoApiJson> rootJson = null;
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

        protected ObservableCollection<tescoApiJson> doHmtl(params string[] strParams)
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
                    strPost += "expiryDate=" + strParams[4] + "&";
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

                rootJson = JsonConvert.DeserializeObject<ObservableCollection<tescoApiJson>>(strResponse);

                if (requestType == "tescoData")
                {
                    foreach (tescoApiJson item in rootJson)
                    {
                        item.flags = new Dictionary<string, string>();
                        item.flags.Add("dataReturned", "true");
                    }
                }
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

        private void processFinish(ObservableCollection<tescoApiJson> jsonList)
        {
            ObservableCollection<tescoApiJson> tlist = StockAppApplicaiton.getconfig().tescoApiList;
            
            if( StockAppApplicaiton.getconfig().acct != null && jsonList != null)
            {
                foreach (tescoApiJson rootJson in jsonList)
                {
                    if (!tlist.Contains(rootJson))
                    {
                        DateTime dateTime = DateTime.Now.Date;
                        DateTime expDate;
                        expDate = Convert.ToDateTime(rootJson.expiryDate);

                        if (expDate.ToString() == "" || expDate == null || expDate.ToShortDateString() == "01/01/0001")
                        {
                            expDate = new DateTime(2222, 01, 01);
                            rootJson.expiryDate = expDate.ToShortDateString();
                        }
                        int comparedDate = DateTime.Compare(dateTime, expDate);
                        int amount;
                        bool success = Int32.TryParse(rootJson.Amount, out amount);
                        if (success)
                        {
                            if (amount > 0 && comparedDate < 0)
                            {
                                rootJson.flags["onShoppingList"] = "false";
                            }
                            else
                            {
                                rootJson.flags["onShoppingList"] = "true";
                            }
                            tlist.Add(rootJson);
                        }

                    }
                }
            }
        }

        protected override void OnPostExecute(string result)
        {
            base.OnPostExecute(result);
            processFinish(rootJson);
            activityResponse.proccessFinish(rootJson);
        }

        protected override string RunInBackground(params string[] @params)
        {
            doHmtl(@params);
            return "Completed";
        }
    }
}

    
    
   
  



