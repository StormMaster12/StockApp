//Add Libarys Here. I dont currently know. Will add when I have access to Visual Studio


namespace StockApp.HTTP
{

  public class HttpPost : AsyncTask <string, void, string>
  {
  
    public readonly string REQUEST_METHOD = "GET";
    public readonly int READ_TIMEOUT = 15000;
    public readonly int CONNECTION_TIMEOUT = 15000;
    
    public MediaType JSON {get; private set } = MediaType.parse("application/json; charset=utf-8");
    
    OkHttpClient client = new OkHttpClient();
    
    public HttpPost()
    {
    }
    
    
    private string post(string url, string json)
    {
      RequestBody body = RequestBody.create(JSON, json);
      Request request = new Request.Builder()
        .url(url)
        .post(body)
        .build();
      try
      {
        Response response = client.newCall(request).execute()
        return response.body.ToString();
      }
      catch (exception as e)
      {
        Console.writeLine(e.ToString());
      }
    }
    
    private string httpjson(string requestType, int pan, int amount, DateTime currentDate, DateTime expiryDate,
    string shortDescription, string longDescritption)
    {
      return "{ 'requestType' : '" + requestType + "',"
      + "'pan' : '" + pan.ToString() + "',"
      + "'amount' : '" + amount.ToString() + "',"
      + "'shortDescription' : '" + shortDescription + "',"
      + "'longDescription' : '" + longDescription + "',"
      + "'currentDate' : '" + currentDate.ToString("d") + "',"
      + "'expiryDate' : '" + expiryDate.ToString("d") + "'}"
    }
    
    protected override string doInBackground (params string[] strParams)
      {
        string strUrl = strParams[0];
        string json = httpjson(strParams[1],strParams[2,strParams[3],strParams[4],strParams[5],strParams[6],strParams[7]);

        URL myUrl = new URL(strUrl);
        string result = post(myUrll,json);
        return result;
      }
    
    protected override void onPostExecute(string result)
    {
      base.onPostExecute(result);
    }
  
  }

}
