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
    
        // Intializes the View Elements.
        private Button btnReadBarcode { get; set; }
        private CompoundButton btnAddItem { get; set; }
        private CompoundButton btnRemoveItem { get; set; }
        private TextView statusMessage { get; set; }
        private TextView barcodeValue { get; set; }

        // Intializes the class elements that will be used.
        private MobileBarcodeScanner scanner;
        private HttpPost httpPost;

        // Intailizes variables.
        private static int RC_BARCODE_CAPTURE = 9001;
        private static string TAG = "BarcodeMain";

        private bool boolRemoveItem = new bool();

        // Constructor which does nothing. Class is created fron newInstance()
        public BarcodeFragment() { }

        // This creates the view hierarchy for this fragment. Needs to associate the layout to this fragment.
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Create the httpPost variable and set the IActivityResponse variable to point to this class.
            httpPost = new HttpPost();
            httpPost.activityResponse = this;

            // This deals with creating the hierarchy to the view. And setting the view elements to the variables above.
            View view = inflater.Inflate(Resource.Layout.Fragment_BarcodeMain, container, false);
            statusMessage = view.FindViewById(Resource.Id.txtBarcodeStatus) as TextView;
            barcodeValue = view.FindViewById(Resource.Id.txtBarcodeValue) as TextView;

            btnReadBarcode = view.FindViewById(Resource.Id.read_barcode) as Button;
            btnAddItem = view.FindViewById(Resource.Id.addItem) as CompoundButton;
            btnRemoveItem = view.FindViewById(Resource.Id.removeItem) as CompoundButton;
            
            // Intialize the Barcode Scanner Activity
            MobileBarcodeScanner.Initialize(base.Activity.Application);
            scanner = new MobileBarcodeScanner();

            // Adds delegates to the button.Click
            btnAddItem.Click += delegate
            {
                boolRemoveItem = false;
            };

            btnRemoveItem.Click += delegate {

                boolRemoveItem = true;
            };

            
            btnReadBarcode.Click += async delegate
            {
                // On click run the scanner, and wait for the result. Hence the delegate has to be async.
                // so that other proccess can continue to run. Such as detecting rotation.
                var result = await scanner.Scan();
                
                // If the scanning was successful. Execute the http request. Passing the url, string add new and the barcode result.
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
    
        // This is what creates the fragment. bundle args, allows for arguments to be passed to the fragment.    
        public static BarcodeFragment newInstance()
        {
            BarcodeFragment fragment = new BarcodeFragment();
            Bundle args = new Bundle();

            fragment.Arguments = args;
            return fragment;
        }

        // This is inherited from IActivityResponse. 
        // Called from httpPost once the http request has finished.
        public void proccessFinish(List<tescoApiJson> jsonList)
        {
            // Try exists incase there is a problem with the data from the http response.
            // Needs to have more detailed information. But at the moment allows the code to 
            // contiune to execute even if there is an error
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
