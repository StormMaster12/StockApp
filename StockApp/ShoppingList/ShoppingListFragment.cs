using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;


using StockApp.HTTP;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace StockApp.ShoppingList
{
    public class ShoppingListFragment : Android.Support.V4.App.Fragment
    {
        private View view { get; set; }
        private ListView lvShoppingList { get; set; }
        private ObservableCollection<tescoApiJson> itemList { get; set; }
        private List<tescoApiJson> onShoppingList { get; set; }
        private ShoppingListAdapter adapter { get; set; }

        private ShoppingListFragment()
        {
            StockAppApplicaiton.getconfig().tescoApiList.CollectionChanged += HandleChange;
            onShoppingList = new List<tescoApiJson>();
        }

        private void HandleChange(object sender, NotifyCollectionChangedEventArgs  e)
        {
            if (StockAppApplicaiton.getconfig().acct != null)
            {
                foreach (tescoApiJson x in e.NewItems)
                {
                    if (x.flags["onShoppingList"] == "true")
                    {
                        onShoppingList.Add(x);
                        //adapter.Add(x);
                    }
                }
            } 
            else
            {
                onShoppingList.RemoveAll(item => item.GetType() == typeof(tescoApiJson));
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.Fragment_ShoppingList, container, false);
            lvShoppingList = (ListView)view.FindViewById(Resource.Id.ShoppingListView);
            adapter = new ShoppingListAdapter(Activity, onShoppingList);

            lvShoppingList.Adapter = adapter;

            return view;
        }

        public static ShoppingListFragment newInstance()
        {
            ShoppingListFragment fragment = new ShoppingListFragment();
            //fragment.AddHandler();
            Bundle args = new Bundle();

            fragment.Arguments = args;
            return fragment;
        }
    }
}