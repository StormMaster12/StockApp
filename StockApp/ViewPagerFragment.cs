using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace StockApp
{
    public class ViewPagerFragment : Android.Support.V4.App.Fragment
    {
        private static string FLASH_CARD_QUESTION = "card_question";
        private static string FLASH_CARD_ANSWER = "card_answer";

        public ViewPagerFragment() { }

        public static ViewPagerFragment newInstance(String question, String answer)
        {
            ViewPagerFragment fragment = new ViewPagerFragment();

            Bundle args = new Bundle();
            args.PutString(FLASH_CARD_QUESTION, question);
            args.PutString(FLASH_CARD_ANSWER, answer);
            fragment.Arguments = args;

            return fragment;
        }

        public override View OnCreateView(
            LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string question = Arguments.GetString(FLASH_CARD_QUESTION, "");
            string answer = Arguments.GetString(FLASH_CARD_ANSWER, "");

            View view = inflater.Inflate(Resource.Layout.Fragment_ScreenSlide, container, false);
            TextView txtView = (TextView)view.FindViewById(Resource.Id.flash_card_question);
            txtView.Text = question;

            return view;
        }
    }
}