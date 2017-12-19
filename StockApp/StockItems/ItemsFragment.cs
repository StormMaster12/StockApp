
using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;

using StockApp.HTTP;
using StockApp.SignIn;
using Android.Support.V4.App;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace StockApp.StockItems
{
    public class ItemsFragment : Fragment, IActivityResponse, IOnSignIn
    {
        private HttpPost getHttp; 
        private string[] strHttp = new string[8];
        private string strResult { get; set; }
        private ListView lvStockItem { get; set; }
        private ObservableCollection<tescoApiJson> itemsResponse = new ObservableCollection<tescoApiJson>();
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
            if (StockAppApplicaiton.getconfig().acct != null && !StockAppApplicaiton.getconfig().SignedIn)
            {
                getHttp = new HttpPost();
                getHttp.activityResponse = this;
                strHttp[0] = GetString(Resource.String.webServerUrl);
                strHttp[1] = GetString(Resource.String.getAll);

                getHttp.Execute(strHttp);
                ((StockAppApplicaiton)Activity.Application).SignedIn = true;
            }
            else if(StockAppApplicaiton.getconfig().acct == null)
            {
                ObservableCollection<tescoApiJson> tescoApiList = StockAppApplicaiton.getconfig().tescoApiList;
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

            StockAppApplicaiton.getconfig().tescoApiList.CollectionChanged += fragment.ModifyItems;

            fragment.Arguments = args;
            return fragment;
        }

        public void proccessFinish(ObservableCollection<tescoApiJson> jsonList)
        {
            getHttp.Cancel(true);
            getHttp.Dispose();
        }

        public void update()
        {
            populateItems();
        }

        private void ModifyItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (StockAppApplicaiton.getconfig().acct != null)
            {
                ObservableCollection<tescoApiJson> tlist = StockAppApplicaiton.getconfig().tescoApiList;

                foreach (var item in tlist)
                {
                    bool found = false;
                    for (int i = 0; i < ItemsArrayAdapter.Count; i++)
                    {
                        if (ItemsArrayAdapter.GetItem(i).Equals(item))
                        {
                            found = true;
                            break;
                        }
                        else
                        {
                            found = false;
                        }
                    }

                    if (!found)
                    {
                        ItemsArrayAdapter.Add(item);
                    }
                }
            }

            
        }
    }
}