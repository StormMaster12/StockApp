//Add Libarys Here. I dont currently know. Will add when I have access to Visual Studio
using System;
using Android.Webkit;
using Android.Media;
using Android.Provider;
using Android.OS;
using Java.Net;

namespace StockApp.HTTP
{
    class HttpPost : AsyncTask<string, string , string>
    {
        public readonly string REQUEST_METHOD = "GET";
        public readonly int READ_TIMEOUT = 15000;
        public readonly int CONNECTION_TIMEOUT = 15000;

        public MediaType JSON { get; private set; }

        public HttpPost()
        {
        }

        private string createPost(URL url, string json)
        {
            HttpPost newpost = new HttpPost();
            try
            {
                newpost.createPost(url, json);
                return newpost.Execute().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return "";
        }

        private string httpjson(string requestType, int pan, int amount, DateTime currentDate, DateTime expiryDate,
    string shortDescription, string longDescritption)
        {
            return "{ 'requestType' : '" + requestType + "',"
            + "'pan' : '" + pan.ToString() + "',"
            + "'amount' : '" + amount.ToString() + "',"
            + "'shortDescription' : '" + shortDescription + "',"
            + "'longDescription' : '" + longDescritption + "',"
            + "'currentDate' : '" + currentDate.ToString("d") + "',"
            + "'expiryDate' : '" + expiryDate.ToString("d") + "'}";
        }

        protected string doInBackground(params string[] strParams)
        {
            string strUrl = strParams[0];
            string json;

            if (strParams[1] == "@string/getAll")
            {
                for (int i =1; i< strParams.Length; i++)
                {
                    strParams[i] = "";
                }
                json = httpjson(strParams[1], int.Parse(strParams[2]), int.Parse(strParams[3]), new DateTime(long.Parse(strParams[4])), new DateTime(long.Parse(strParams[5])), strParams[6], strParams[7]);
            }
            else
            {
                json = httpjson(strParams[1], int.Parse(strParams[2]), int.Parse(strParams[3]), new DateTime(long.Parse(strParams[4])), new DateTime(long.Parse(strParams[5])), strParams[6], strParams[7]);
            }

            URL myUrl = new URL(strUrl);
            string result = createPost(myUrl, json);
            return result;
        }

        protected void onPostExecute(string result)
        {
            base.OnPostExecute(result);
        }

        protected override string RunInBackground(params string[] @params)
        {
            throw new NotImplementedException();
        }
    }
}

    
    
   
  



