using System;

using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Java.Lang;

using StockApp.BarcodeReader;

namespace StockApp
{
    class ViewPagerAdapter : FragmentPagerAdapter
    {
        public FlashCardDeck flashCardDeck;

        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm, FlashCardDeck flashCards)
            : base(fm)
        {
            this.flashCardDeck = flashCards;
        }

        public override int Count
        {
            get { return flashCardDeck.NumCards + 1; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            if (position != (flashCardDeck.NumCards ))
            {
                return (Android.Support.V4.App.Fragment)
                ViewPagerFragment.newInstance(flashCardDeck[position].Problem, flashCardDeck[position].Answer);
            }
            else
            {
                return BarcodeFragment.newInstance();
            }
            
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String("Problem " + (position + 1));
        }
    }
}