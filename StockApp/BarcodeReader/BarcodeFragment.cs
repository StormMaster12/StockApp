using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Android.Gms.Vision.Barcodes;
using Android.Gms.Common.Apis;
using ZXing.Mobile;
using StockApp.HTTP;
using System.Collections.Generic;

namespace StockApp.BarcodeReader
{
    class BarcodeFragment : Android.Support.V4.App.Fragment, IActivityResponse
    { 
        private Button btnReadBarcode { get; set; }
        private CompoundButton btnAddItem { get; set; }
        private CompoundButton btnRemoveItem { get; set; }
        private TextView statusMessage { get; set; }
        private TextView barcodeValue { get; set; }

        MobileBarcodeScanner scanner;
        private HttpPost httpPost;

        private static int RC_BARCODE_CAPTURE = 9001;
        private static string TAG = "BarcodeMain";

        private bool boolRemoveItem = new bool();

        public BarcodeFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            httpPost = new HttpPost();
            httpPost.activityResponse = this;

            View view = inflater.Inflate(Resource.Layout.Fragment_BarcodeMain, container, false);
            statusMessage = view.FindViewById(Resource.Id.txtBarcodeStatus) as TextView;
            barcodeValue = view.FindViewById(Resource.Id.txtBarcodeValue) as TextView;

            btnReadBarcode = view.FindViewById(Resource.Id.read_barcode) as Button;
            btnAddItem = view.FindViewById(Resource.Id.addItem) as CompoundButton;
            btnRemoveItem = view.FindViewById(Resource.Id.removeItem) as CompoundButton;
            MobileBarcodeScanner.Initialize(base.Activity.Application);
            scanner = new MobileBarcodeScanner();

            btnAddItem.Click += delegate
            {
                boolRemoveItem = false;
            };

            btnRemoveItem.Click += delegate {

                boolRemoveItem = true;
            };

            btnReadBarcode.Click += async delegate
            {

                var result = await scanner.Scan();

                if (result != null)
                {
                    string[] strHttp = { GetString(Resource.String.webServerUrl), GetString(Resource.String.addNew), result.Text, "","",boolRemoveItem.ToString() };
                    httpPost.Execute(strHttp);

                    Console.WriteLine("Scanned Barcode: " + result.Text);
                }
                        
            };
            return view;
        } 

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static BarcodeFragment newInstance()
        {
            BarcodeFragment fragment = new BarcodeFragment();
            Bundle args = new Bundle();

            fragment.Arguments = args;
            return fragment;
        }

        public void proccessFinish(List<tescoApiJson> jsonList)
        {
            try
            {
                statusMessage.Text = jsonList[0].items[0].description;
                barcodeValue.Text = jsonList[0].items[0].gtin;
            }
            catch (Exception e)
            {
                statusMessage.Text = "Retrieval From Server Uncessfull";
            }
        }
    }
}
