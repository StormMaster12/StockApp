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
using System.Collections.ObjectModel;

namespace StockApp.ShoppingList
{
    class ShoppingListAdapter : ArrayAdapter<tescoApiJson>
    {

        Context context;
        List<tescoApiJson> Itemslist;

        public ShoppingListAdapter(Context context, List<tescoApiJson> list) : base(context, Resource.Layout.StockItem_Fragment)
        {
            this.context = context;
            this.Itemslist = list;
            foreach(tescoApiJson item in Itemslist)
            {
                Add(item);
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            View rowView = layoutInflater.Inflate(Resource.Layout.ShoppingList_RowFragment, parent, false);
            TextView textView = rowView.FindViewById(Resource.Id.ShoppingList_TextView) as TextView;
            CompoundButton compoundButton = rowView.FindViewById(Resource.Id.ShoppingList_BoughtItem) as CompoundButton;

            tescoApiJson item = GetItem(position);
            textView.Text = item.items[0].description;

            return rowView;
        }
    }
}