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

namespace StockApp.HTTP
{
    class RootJson
    {
        public string Name {get; set;}
        public string Pan { get; set; }
        public string Amount { get; set; }
        public string Short_Description { get; set; }
        public string Long_Description { get; set; }
        public string purchaseDate { get; set; }
        public string expiryDate { get; set; }
    }
}