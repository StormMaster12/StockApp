
using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;

using StockApp.HTTP;

namespace StockApp.StockItems
{
    class ItemsFragment : Android.Support.V4.App.Fragment, IActivityResponse
    {
        private HttpPost getHttp = new HttpPost(); 
        private string[] strHttp = new string[8];
        private string strResult { get; set; }
        private ListView lvStockItem { get; set; }
        private List<tescoApiJson> itemsResponse = new List<tescoApiJson>();
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


        public void proccessFinish(List<tescoApiJson> jsonList)
        {
            try
            {
                foreach (tescoApiJson rootJson in jsonList)
                {
                    ItemsArrayAdapter.Add(rootJson);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }
    }
}
