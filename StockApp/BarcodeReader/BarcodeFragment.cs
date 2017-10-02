using System;
using Android.OS;
using Android.Views;
using Android.Widget;

using Android.Gms.Common.Apis;
using ZXing.Mobile;
using StockApp.HTTP;
using System.Collections.Generic;
using System.Text;

namespace StockApp.BarcodeReader
{
    class BarcodeFragment : Android.Support.V4.App.Fragment, IActivityResponse
    { 
        private Button btnReadBarcode { get; set; }
        private Button btnConfirm { get; set; }
        private Button btnDelete { get; set; }
        private CompoundButton btnAddItem { get; set; }
        private CompoundButton btnRemoveItem { get; set; }
        private TextView statusMessage { get; set; }
        private TextView barcodeValue { get; set; }

        MobileBarcodeScanner scanner;
        private HttpPost httpPost;

        private bool boolRemoveItem = new bool();
        private tescoApiJson apiJson;

        public BarcodeFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Fragment_BarcodeMain, container, false);
            statusMessage = view.FindViewById(Resource.Id.txtBarcodeStatus) as TextView;
            barcodeValue = view.FindViewById(Resource.Id.txtBarcodeValue) as TextView;

            btnReadBarcode = view.FindViewById(Resource.Id.read_barcode) as Button;
            btnConfirm = view.FindViewById(Resource.Id.confirm) as Button;
            btnDelete = view.FindViewById(Resource.Id.delete) as Button;

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
                httpPost = new HttpPost();
                httpPost.activityResponse = this;

                var result = await scanner.Scan();

                if (result != null)
                {
                    string[] strHttp = { GetString(Resource.String.webServerUrl), GetString(Resource.String.addNew), result.Text, "1","23/09/2017",boolRemoveItem.ToString() };
                    httpPost.Execute(strHttp);

                    Console.WriteLine("Scanned Barcode: " + result.Text);
                }
                        
            };

            btnConfirm.Click += delegate
            {
                barcodeValue.Text = "";
                statusMessage.Text = "Confirmed \nScan a New Item?";
                btnConfirm.Visibility = ViewStates.Gone;
                btnDelete.Visibility = ViewStates.Gone;
                btnReadBarcode.Visibility = ViewStates.Visible;
                btnAddItem.Visibility = ViewStates.Visible;
                btnRemoveItem.Visibility = ViewStates.Visible;
            };

            btnDelete.Click += delegate
            {
                httpPost = new HttpPost();
                httpPost.activityResponse = this;

                barcodeValue.Text = "";
                statusMessage.Text = "Not the Correct Item\nScan a New Item? ";

                string[] strHttp = { GetString(Resource.String.webServerUrl), GetString(Resource.String.removeSpecific),apiJson.items[0].gtin};
                httpPost.Execute(strHttp);

                btnConfirm.Visibility = ViewStates.Gone;
                btnDelete.Visibility = ViewStates.Gone;
                btnReadBarcode.Visibility = ViewStates.Visible;
                btnAddItem.Visibility = ViewStates.Visible;
                btnRemoveItem.Visibility = ViewStates.Visible; 
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
                apiJson = jsonList[0];
                if (apiJson.flags == "dataReturned")
                {
                    string strDescription = AddSpacesToSentence(apiJson.items[0].description);
                    string strGTIN = AddSpacesToSentence(apiJson.items[0].gtin);

                    statusMessage.Text = strDescription;
                    barcodeValue.Text = strGTIN;
                }
                else if(apiJson.flags == "removed")
                {
                    barcodeValue.Text = "Data Removed";
                    return;
                }
            }
            catch (Exception e)
            {
                statusMessage.Text = "Retrieval From Server Uncessfull";
            }
            finally
            {
                btnReadBarcode.Visibility = ViewStates.Gone;
                btnAddItem.Visibility = ViewStates.Gone;
                btnRemoveItem.Visibility = ViewStates.Gone;
                btnConfirm.Visibility = ViewStates.Visible;
                btnDelete.Visibility = ViewStates.Visible;

                httpPost.Cancel(true);
                httpPost.Dispose();
            }

            

        }

        string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
