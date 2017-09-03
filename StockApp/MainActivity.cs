using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V7.App;
using System.Reflection;
using System.Linq;

namespace StockApp
{

    [Activity(Label = "StockApp", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "StockApp");
            for (int i = 0; i < typelist.Length; i++)
            {
                Console.WriteLine(typelist[i].Name);
            }

            // Set the content view from the "Main" layout resource:
            SetContentView(Resource.Layout.Activity_ScreenSlide);

            // Instantiate the deck of flash cards:
            FlashCardDeck flashCards = new FlashCardDeck();

            // Instantiate the adapter and pass in the deck of flash cards:
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager, flashCards);

            // Find the ViewPager and plug in the adapter:

            ViewPager pager = FindViewById(Resource.Id.viewpager) as ViewPager;
            pager.Adapter = adapter;
        }

    }
}

