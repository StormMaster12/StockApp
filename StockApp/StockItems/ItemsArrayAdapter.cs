using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using StockApp.HTTP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StockApp.StockItems
{
    class ItemsArrayAdapter : ArrayAdapter<tescoApiJson>
    {
        private Context mContext { get; set; }
        private ObservableCollection<tescoApiJson> ItemsList { get; set; }

        public ItemsArrayAdapter(Context context, ObservableCollection<tescoApiJson> list): base(context, Resource.Layout.StockItem_Fragment)
        {
            this.mContext = context;
            this.ItemsList = list;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);

            View rowView = inflater.Inflate(Resource.Layout.StockItem_Fragment, parent, false);
            TextView textView = (TextView)rowView.FindViewById(Resource.Id.ItemDescription);
            ImageView imageView = (ImageView)rowView.FindViewById(Resource.Id.ItemImage);
            Button itemButton = (Button)rowView.FindViewById(Resource.Id.ItemButton);

            tescoApiJson item =  GetItem(position);
            if (item.items.Count != 0)
            {
                if (item.flags["onShoppingList"] == "false")
                {
                    GradientDrawable gradientDrawable = rowView.Background as GradientDrawable;
                    gradientDrawable.SetStroke(20, new Color(34,139,34));
                }
                else
                {
                    GradientDrawable gradientDrawable = rowView.Background as GradientDrawable;
                    gradientDrawable.SetStroke(20, new Color(218, 19, 19));
                }

                textView.Text = "Name: " + item.items[0].description + "\n Amount: " + item.Amount + "\n Expiry Date: " + item.expiryDate;
                itemButton.Text = "Click To see more information about :" + item.items[0].description;
                itemButton.Click += (sender, e) =>
                {
                    Intent intent = new Intent(mContext, typeof(ItemsDetailedActivity));
                    intent.PutExtra("Position", position);
                    mContext.StartActivity(intent);
                };
            }
            return rowView;
        }
    }
}
