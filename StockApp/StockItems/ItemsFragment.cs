
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

namespace StockApp.StockItems
{
  class ItemsFragment : Android.Support.V4.App.Fragment
  {
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      View view = inflater.Inflate(Resource.Layout.Fragment_ItemsMain, container, false);
      
      populateItems(view);
      
      return view;
    }
    
    private void populateItems(View view)
    {
      //RequestQueue 
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
