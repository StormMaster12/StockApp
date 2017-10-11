using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using StockApp.HTTP;
using System;
using System.Collections.Generic;

namespace StockApp.StockItems
{
    class ItemsArrayAdapter : ArrayAdapter<tescoApiJson>
    {
        private Context mContext { get; set; }
        private List<tescoApiJson> ItemsList { get; set; }
        DateTime dateTime;

        public ItemsArrayAdapter(Context context, List<tescoApiJson> list): base(context, Resource.Layout.StockItem_Fragment)
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
                dateTime = DateTime.Now.Date;
                DateTime expDate;
                expDate = Convert.ToDateTime(item.expiryDate);

                if (expDate.ToString() == "" || expDate == null)
                {
                    expDate = new DateTime(2222, 01, 01);
                    item.expiryDate = expDate.ToShortDateString();
                }

                if (Int32.Parse(item.Amount) >= 1 || expDate > dateTime)
                {
                    GradientDrawable gradientDrawable = rowView.Background as GradientDrawable;
                    gradientDrawable.SetStroke(20, new Color(34,139,34));
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
