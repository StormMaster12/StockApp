using System;
using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using StockApp.SignIn;
using Android.Content;
using Android.Support.V4.App;
using static Android.Support.V4.View.ViewPager;
using System.Collections.Generic;

using StockApp.BarcodeReader;
using StockApp.StockItems;

namespace StockApp
{

    [Activity(Label = "StockApp", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private List<Android.Support.V4.App.Fragment> fragments = new List<Android.Support.V4.App.Fragment>();
        // Overrided function. The first thing the activity will run.
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Passes the savedInstanceState to the AppCompatActivity OnCreate. 
            base.OnCreate(savedInstanceState);

            // Set the content view from the "Main" layout resource:
            SetContentView(Resource.Layout.Activity_ScreenSlide);
            Toolbar toolBar = FindViewById(Resource.Id.my_toolbar) as Toolbar;
            SetSupportActionBar(toolBar);
            // Instantiate the adapter
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            fragments.Add(BarcodeFragment.newInstance());
            fragments.Add(ItemsFragment.newInstance());
            adapter.fragments = fragments;

            ((StockAppApplicaiton)this.Application).ItemsFragment = fragments[1] as ItemsFragment;

            // Find the ViewPager and plug in the adapter:

            ViewPager pager = FindViewById(Resource.Id.viewpager) as ViewPager;
            pager.Adapter = adapter;
            pager.AddOnPageChangeListener(new PageChangeListen(adapter));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_signin:
                    Intent intent = new Intent(this, typeof(SigninClass));
                    StartActivity(intent);
                    return true;
                case Resource.Id.action_settings:
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

        class MyHandler : Handler
        {
            FragmentPagerAdapter myPager;
            public MyHandler(FragmentPagerAdapter Pager)
            {
                myPager = Pager;
            }

            public override void HandleMessage(Message msg)
            {
                if (msg.What == 1)
                {
                    myPager.NotifyDataSetChanged();
                }
            }
        }

        class PageChangeListen : Java.Lang.Object, IOnPageChangeListener
        {

            FragmentPagerAdapter myPager;
            public PageChangeListen(FragmentPagerAdapter Pager)
            {
                myPager = Pager;
            }

            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
            }

            public void OnPageScrollStateChanged(int state)
            {
            }

            public void OnPageSelected(int position)
            {
                if (position == 2)
                {
                    Message msg = new Message();
                    msg.What = 1;
                    new MyHandler(myPager).SendMessage(msg);
                }
            }
        }
    }
}

