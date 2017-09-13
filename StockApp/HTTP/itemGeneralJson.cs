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
using Newtonsoft.Json;

namespace StockApp.HTTP
{
    public class itemGeneralJson : RootJson
    {

        /// <summary>
        /// Name of A product
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Pan of a product
        /// </summary>
        [JsonProperty("Pan")]
        public string Pan { get; set; }

        /// <summary>
        /// The amount of an item in the database
        /// </summary>
        [JsonProperty("Amount")]
        public string Amount { get; set; }

        /// <summary>
        /// What It says on the tin
        /// </summary>
        [JsonProperty("Short_Description")]
        public string Short_Description { get; set; }

        /// <summary>
        /// What It says on the tin
        /// </summary>
        [JsonProperty("Long_Desciption")]
        public string Long_Description { get; set; }

        /// <summary>
        /// The date the item was purchased
        /// </summary>
        [JsonProperty("purchaseDate")]
        public string purchaseDate { get; set; }

        /// <summary>
        /// The date the item will expire
        /// </summary>
        [JsonProperty("expiryDate")]
        public string expiryDate { get; set; }

        [JsonProperty("idStock")]
        public int Id { get; set; }

        /// <summary>
        /// Contains the image information
        /// </summary>
        public string imageBinary { get; set; }
    }
}