
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Gms.Vision.Barcodes;
using Android.Gms.Common.Apis;

using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V7.App;

using StockApp.HTTP;

namespace StockApp.StockItems
{
  class ItemsFragment : Android.Support.V4.App.Fragment
  {
    private HttpPost getHttp;
    private string[] strHttp = new string[8];
    private string strResult;
  
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      View view = inflater.Inflate(Resource.Layout.Fragment_ItemsMain, container, false);
      
      populateItems(view);
      
      return view;
    }
    
    private void populateItems(View view)
    {
      strHttp[0] =  GetString(Resource.String.webServerUrl);
      strHttp[1] = GetString(Resource.String.getAll);
      getHttp = new HttpPost();
      
      strResult = (string)getHttp.Execute(strHttp).Get();
      Console.WriteLine(strResult);
    }

    public static ItemsFragment newInstance()
        {
            ItemsFragment fragment = new ItemsFragment();
            Bundle args = new Bundle();

            fragment.Arguments = args;
            return fragment;
        }
  }
}
