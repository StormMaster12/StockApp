using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;
using StockApp.HTTP;

namespace StockApp.StockItems
{
    [Activity(Label = "Detailed Item Activity")]
    class ItemsDetailedActivity : AppCompatActivity
    {
        private List<tescoApiJson> detailedList;
	    private TextView itemName;
	    private TextView itemStatistics;
        private TableLayout tableNutrition;
	    private ImageView itemImage;
        private int position;

        public ItemsDetailedActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StockItem_Detailed);

            Intent intent = Intent;
            position = intent.GetIntExtra("Position", 0);

            itemName = FindViewById(Resource.Id.DetailedItemName) as TextView;
            itemStatistics = FindViewById(Resource.Id.DetailedItemStatistics) as TextView;
            tableNutrition = FindViewById(Resource.Id.nutritionTable) as TableLayout;

            itemImage = FindViewById(Resource.Id.DetailedItemImage) as ImageView;
            populateItems();
        }

		public void populateItems()
        {
            tescoApiJson item = ((StockAppApplicaiton)Application).tescoApiList[position];

            itemName.Text = item.items[0].description;
            string tableName = "nutrition";

            try
            {
                string[] tempArr = { "Nutrition", item.items[0].nutrition.per100Header };
                createTableRow(tempArr, tableName);

                foreach (var subItem in item.items[0].nutrition.nutrients)
                {
                    tempArr[0] = subItem.name;
                    tempArr[1] = subItem.valuePer100 ;
                    createTableRow(tempArr, tableName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                string[] tempArr = { "No Nutrition Information Provided" };
                createTableRow(tempArr, tableName);
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
                strIngredients = "No Ingredients Provided";
            }
            finally
            {
                itemStatistics.Text = strIngredients;
            }      
        }

        private void createTableRow(string[] strColumns, string table)
        {
            TableRow tableRow = new TableRow(this);
            tableRow.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent);
            foreach (string column in strColumns)
            {
                TextView textColumn = new TextView(this);
                textColumn.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent);
                textColumn.Text = column;
                tableRow.AddView(textColumn);
            }
            if (table == "nutrition")
            {
                tableNutrition.AddView(tableRow, new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent));
            }
        }
    }
}
