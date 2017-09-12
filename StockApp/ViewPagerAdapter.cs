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

        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm)
            : base(fm)
        {
        }

        public override int Count
        {
            get { return 2; }
        }

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

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String("Problem " + (position + 1));
        }
    }
}