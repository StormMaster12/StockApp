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
using Android.Support.V7.App;
using StockApp.HTTP;

namespace StockApp.StockItems
{
    [Activity(Label = "Detailed Item Activity")]
    class ItemsDetailedActivity : AppCompatActivity, IActivityResponse
    {
        private List<tescoApiJson> detailedList;
	private HttpPost httpPost;
	private TextView itemName;
	private TextView itemStatistics;
	private TextView itemDescription;
	private ImageView itemImage;
        private string[] strHttpList;
		
        protected override void OnCreate(Bundle savedInstanceState)
        {
            httpPost = new HttpPost();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StockItem_Detailed);

            Intent intent = Intent;
            strHttpList =  intent.GetStringArrayExtra("getSpecificList");

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
		
		public void proccessFinish(List<tescoApiJson> jsonList)
        {
            tescoApiJson item = jsonList[0];

            itemName.Text = item.items[0].description;

            string strDescription="";

            try
            {

                strDescription += "Nutrition    | " + item.items[0].nutrition.per100Header + "\n";

                foreach (var subItem in item.items[0].nutrition.nutrients)
                {
                    strDescription += subItem.name + "  | " + subItem.valuePer100 + "\n";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            string strIngredients = "";
            try
            {
                strIngredients += "Ingredients \n";
                foreach (var subItem in item.items[0].ingredients)
                {
                    strIngredients += subItem + "\n";
                }
            }
            catch (Exception e)
            {
                itemStatistics.Text = strIngredients;
            }

            itemDescription.Text = strDescription;

            httpPost.Cancel(true);
            
        }
    }
}
