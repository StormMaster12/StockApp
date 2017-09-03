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

namespace StockApp
{
    class BarcodeFragment : Android.Support.V4.App.Fragment, View.IOnClickListener
    {
        private CompoundButton autoFocus { get; set; }
        private CompoundButton useFlash { get; set; }
        private Button btnReadBarcode { get; set; }

        private TextView statusMessage { get; set; }
        private TextView barcodeValue { get; set; }

        private static int RC_BARCODE_CAPTURE = 9001;
        private static string TAG = "BarcodeMain";

        public BarcodeFragment() { }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.read_barcode)
            {

                Intent intent = new Intent(Context.ApplicationContext,Activity.ClassLoader.LoadClass("barcodeFragmentActivity"));//("StockApp.StockApp", typeof(BarcodeFragmentActivity));
                //intent.SetClassName("StockApp.StockApp", );
                intent.PutExtra("AutoFocus", autoFocus.Checked);
                intent.PutExtra("UseFlash", useFlash.Checked);

                if (intent.ResolveActivity(Activity.PackageManager) != null)
                {
                    StartActivityForResult(intent, RC_BARCODE_CAPTURE);
                }
                
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == RC_BARCODE_CAPTURE)
            {
                if(resultCode == CommonStatusCodes.Success)
                {
                    if(data != null)
                    {
                      //  Barcode barcode = data.GetParcelableExtra(BarcodeFragmentActivity.BarcodeObject);
                    }
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Fragment_BarcodeMain, container, false);
            statusMessage = view.FindViewById(Resource.Id.txtBarcodeStatus) as TextView;
            barcodeValue = view.FindViewById(Resource.Id.txtBarcodeValue) as TextView;

            autoFocus = view.FindViewById(Resource.Id.auto_focus) as CompoundButton;
            useFlash = view.FindViewById(Resource.Id.use_flash) as CompoundButton;

            view.FindViewById(Resource.Id.read_barcode).SetOnClickListener(this);
            btnReadBarcode = view.FindViewById(Resource.Id.read_barcode) as Button;



            return view;
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