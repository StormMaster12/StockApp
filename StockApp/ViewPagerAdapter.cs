using Android.OS;
using Android.Support.V4.App;
using Java.Lang;
using System.Collections.Generic;
using static Android.Support.V4.View.ViewPager;

namespace StockApp
{
    class ViewPagerAdapter : FragmentPagerAdapter
    {
        public List<Fragment> fragments { get; set; }
        // The constructor. All this does is pass the fm to the base constructor. Allows the base to do the construciton.
        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm)
            : base(fm)
        {
        }
        // Provides the amount of pages in the adapter.
        public override int Count
        {
            get { return fragments.Count; }
        }
        // Returns the fragment for the specific page in the adapter. Position 0 will return the BarcodeFragment
        // and Postion 1 will return the ItemsFragment.
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }
        // This provdes the title bar what to show.
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String("Fragment " + (position + 1));
        }

    }

}
