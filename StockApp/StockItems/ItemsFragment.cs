
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
using System.Collections;

namespace StockApp.StockItems
{
    class ItemsFragment : Android.Support.V4.App.Fragment, IActivityResponse
    {
        private HttpPost getHttp = new HttpPost(); 
        private string[] strHttp = new string[8];
        private string strResult { get; set; }
        private ListView lvStockItem { get; set; }
        private List<RootJson> itemsResponse = new List<RootJson>();
        private ItemsArrayAdapter ItemsArrayAdapter { get; set; }
        private View view { get; set; }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.StockItem__Overview, container, false);
            lvStockItem = (ListView)view.FindViewById(Resource.Id.StockItemOverview);
            ItemsArrayAdapter = new ItemsArrayAdapter(Activity, itemsResponse);

            lvStockItem.Adapter = ItemsArrayAdapter;

            getHttp.activityResponse = this;
            populateItems(view);

            return view;
        }

        private void populateItems(View view)
        {
            strHttp[0] = GetString(Resource.String.webServerUrl);
            strHttp[1] = GetString(Resource.String.getAll);


            getHttp.Execute(strHttp);
            Console.WriteLine(strResult);
        }

        public static ItemsFragment newInstance()
        {
            ItemsFragment fragment = new ItemsFragment();
            Bundle args = new Bundle();

            fragment.Arguments = args;
            return fragment;
        }


        public void proccessFinish(List<RootJson> jsonList)
        {
            foreach(RootJson rootJson in jsonList)
            {
                ItemsArrayAdapter.Add(rootJson);
            }
            
        }
    }
}
