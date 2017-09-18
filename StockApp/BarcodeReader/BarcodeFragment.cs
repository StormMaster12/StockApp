using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Gms.Vision.Barcodes;
using Android.Gms.Common.Apis;

using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V7.App;
using ZXing.Net.Mobile;
using ZXing.Mobile;

namespace StockApp.BarcodeReader
{
    class BarcodeFragment : Android.Support.V4.App.Fragment
    { 
        private CompoundButton autoFocus { get; set; }
        private CompoundButton useFlash { get; set; }
        private Button btnReadBarcode { get; set; }

        private TextView statusMessage { get; set; }
        private TextView barcodeValue { get; set; }

        private static int RC_BARCODE_CAPTURE = 9001;
        private static string TAG = "BarcodeMain";
        MobileBarcodeScanner scanner;

        public BarcodeFragment() { }

        public void StartNewActivity(object sender, EventArgs e)
        {

            //Intent intent = new Intent(Context.ApplicationContext,Activity.ClassLoader.LoadClass("barcodeFragmentActivity"));//("StockApp.StockApp", typeof(BarcodeFragmentActivity));
            //intent.SetClassName("StockApp.StockApp", );
            //Intent intent = new Intent(Activity.BaseContext, typeof(BarcodeFragmentActivity));
            //intent.PutExtra("AutoFocus", autoFocus.Checked);
            //intent.PutExtra("UseFlash", useFlash.Checked);

            //StartActivityForResult(intent, RC_BARCODE_CAPTURE);

        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == RC_BARCODE_CAPTURE)
            {
                if(resultCode == CommonStatusCodes.Success)
                {
                    if(data != null)
                    {
                        Barcode barcode = (Barcode)data.GetParcelableExtra(BarcodeFragmentActivity.BarcodeObject);
                        statusMessage.Text = "Barcode Success";
                        barcodeValue.Text = (barcode.DisplayValue);
                        Console.WriteLine("Barcode Read : " + barcode.DisplayValue.ToString());
                    
                    }
                    else
                    {
                        statusMessage.Text = "Barcode Failed";
                    }
                }
                else
                {
                    statusMessage.Text = "Barcode Error";
                }
            }
            else
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Fragment_BarcodeMain, container, false);
            statusMessage = view.FindViewById(Resource.Id.txtBarcodeStatus) as TextView;
            barcodeValue = view.FindViewById(Resource.Id.txtBarcodeValue) as TextView;

            autoFocus = view.FindViewById(Resource.Id.auto_focus) as CompoundButton;
            useFlash = view.FindViewById(Resource.Id.use_flash) as CompoundButton;
            btnReadBarcode = view.FindViewById(Resource.Id.read_barcode) as Button;
            scanner = new MobileBarcodeScanner();
            scanner.Initialize (Application);

            btnReadBarcode.Click += async delegate {
            
                    var result = await scanner.Scan();

                    if (result != null)
                        Console.WriteLine("Scanned Barcode: " + result.Text);
            };

            //btnReadBarcode.Click += delegate {
//
//                Intent intent = new Intent("com.google.zxing.client.android.SCAN");
//                StartActivityForResult(intent, 0);
//            };



            return view;
        }

        public void onActivityResult(int requestCode,int resultCode, Intent intent)
        {
            if(resultCode == 0)
            {
                if(resultCode == (int)Result.Ok)
                {
                    string contents = intent.GetStringExtra("SCAN_RESULT");
                    string format = intent.GetStringExtra("SCAN_RESULT_FORMAT");

                    Console.WriteLine("BARCODE RESULT : {0} \n Format Result : {1}", contents, format);
                }
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        private void HandleScanResult(ZXing.Result result)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
                msg = "Found Barcode: " + result.Text;
            else
                msg = "Scanning Canceled!";

            Toast.MakeText(Activity, msg, ToastLength.Short).Show();
        }


        public static BarcodeFragment newInstance()
        {
            BarcodeFragment fragment = new BarcodeFragment();
            Bundle args = new Bundle();

            fragment.Arguments = args;
            return fragment;
        }
    }
}
