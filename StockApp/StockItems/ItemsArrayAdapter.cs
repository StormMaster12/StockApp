using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using StockApp.HTTP;

namespace StockApp.StockItems
{
    class ItemsArrayAdapter : ArrayAdapter<tescoApiJson>
    {
        private Context mContext { get; set; }
        private List<tescoApiJson> ItemsList { get; set; }

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
            if (item != null)
            {
                textView.Text = "Name: " + item.items.description + "\n Amount: 1 \n Expiry Date: 12/09/2018";
                //imageView.SetImageURI(new Android.Net.Uri("http://google.com"));
                itemButton.Text = "Click To see more information about :" + item.items.description;
                itemButton.Click += (sender, e) =>
                {
                    string[] strHttp = new string[8];
                    strHttp[0] = mContext.Resources.GetString(Resource.String.webServerUrl); //     "http://ec2-35-177-33-89.eu-west-2.compute.amazonaws.com/";
                    strHttp[1] = mContext.Resources.GetString(Resource.String.getSpecific);
                    strHttp[2] = GetItem(position).items.gtin.ToString();
                    //strHttp[3] = GetItem(position).Pan;
                    //strHttp[4] = GetItem(position).Amount;
                    //strHttp[5] = GetItem(position).expiryDate;

                    Intent intent = new Intent(mContext, typeof(ItemsDetailedActivity));
                    intent.PutExtra("getSpecificList", strHttp);
                    mContext.StartActivity(intent);
                };
            }
            return rowView;
        }
    }
}
