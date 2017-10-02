using System;

using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Java.Lang;

using StockApp.BarcodeReader;
using StockApp.StockItems;

namespace StockApp
{
    class ViewPagerAdapter : FragmentPagerAdapter
    {
        // The constructor. All this does is pass the fm to the base constructor. Allows the base to do the construciton.
        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm)
            : base(fm)
        {
        }
        // Provides the amount of pages in the adapter.
        public override int Count
        {
            get { return 2; }
        }
        // Returns the fragment for the specific page in the adapter. Position 0 will return the BarcodeFragment
        // and Postion 1 will return the ItemsFragment.
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {   
            if (position == 0)
            {
                return BarcodeFragment.newInstance();
            }
            else if(position == 1)
            {
                return ItemsFragment.newInstance();
            }
            return null;
            
        }
        // This provdes the title bar what to show.
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String("Problem " + (position + 1));
        }
    }
}
