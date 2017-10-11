
using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;

using StockApp.HTTP;
using StockApp.SignIn;
using Android.Support.V4.App;

namespace StockApp.StockItems
{
    public class ItemsFragment : Fragment, IActivityResponse, IOnSignIn
    {
        private HttpPost getHttp; 
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
            
            populateItems();

            return view;
        }

        private void populateItems()
        {
            if (((StockAppApplicaiton) Activity.Application).acct != null && !((StockAppApplicaiton)Activity.Application).SignedIn)
            {
                getHttp = new HttpPost();
                getHttp.activityResponse = this;
                strHttp[0] = GetString(Resource.String.webServerUrl);
                strHttp[1] = GetString(Resource.String.getAll);

                getHttp.Execute(strHttp);
                ((StockAppApplicaiton)Activity.Application).SignedIn = true;
            }
            else if(((StockAppApplicaiton)Activity.Application).acct == null)
            {

                List<tescoApiJson> tescoApiList = ((StockAppApplicaiton)Activity.Application).tescoApiList;
                for (int count = ItemsArrayAdapter.Count - 1; count >= 0; count--)
                {
                    ItemsArrayAdapter.Remove(tescoApiList[count]);
                    tescoApiList.RemoveAt(count);
                }

            }
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

                ((StockAppApplicaiton)Activity.Application).tescoApiList = jsonList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                getHttp.Cancel(true);
                getHttp.Dispose();
            }
        }

        public void update()
        {
            populateItems();
        }
    }
}
