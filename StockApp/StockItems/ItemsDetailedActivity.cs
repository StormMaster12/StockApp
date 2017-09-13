?using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using StockApp.HTTP;

namespace StockApp.StockItems
{
    [Activity(Label = "Detailed Item Activity")]
    class ItemsDetailedActivity : AppCompatActivity, IActivityResponse
    {
        private List<RootJson> detailedList;
		private HttpPost httpPost = new HttpPost();
		private TextView itemName;
		private TextView itemStatistics;
		private TextView itemDescription;
		private ImageView itemImage;
		
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StockItem_Detailed);

            Intent intent = Intent;
            strHttpList =  intent.GetSerializableExtra("getSpecificList") as string[];

            itemName = FindViewById(Resource.Id.DetailedItemName) as TextView;
            itemStatistics = FindViewById(Resource.Id.DetailedItemStatistics) as TextView;
            itemDescription = FindViewById(Resource.Id.DetailedItemDescription) as TextView;
            itemImage = FindViewById(Resource.Id.DetailedItemImage) as ImageView;
	
			httpPost.activityResponse = this;
			
			httpPost.Execute(strHttpList);
        }

        public ItemsDetailedActivity()
        {

        }
		
		public void proccessFinish(List<RootJson> jsonList)
        {
            RootJson item = jsonList[0];

            itemName.Text = item.Name;
            itemStatistics.Text = "Date Purchased : " + item.purchaseDate + "\n Expiry Date: " + item.expiryDate + "\n Amount Left: " + item.Amount + "\n Pan: " + item.Pan;
            itemDescription.Text = "Short Description:\n " + item.Short_Description + "\n Long Description:\n " + item.Long_Description;
			
			httpPost.Cancel(true);
            
        }
    }
}
