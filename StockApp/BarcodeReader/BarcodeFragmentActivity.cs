using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

using Android.Gms.Vision.Barcodes;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Vision;

using Android.Hardware;

using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.Design.Widget;

using StockApp.UI;
using Android.Util;

namespace StockApp.BarcodeReader
{
    [Activity(Label = "Barcode Fragment Activity")]
    class BarcodeFragmentActivity : AppCompatActivity
    {
        private static string tag = "Barcode-Reader";
        private static int RC_HANDLE_GMS = 9001;
        private static int RC_HANDLE_CAMERA_PERM = 2;

        public static string AutoFocus = "AutoFocus";
        public static string UseFlash = "UseFlash";
        public static string BarcodeObject = "Barcode";

        private CameraSourcePreview mPreview;
        private UI.CameraSource mCameraSource;
        private GraphicOverlay mGraphicOverlay;

        private ScaleGestureDetector scaleGestureDetector;
        private GestureDetector getsureDetector;

        private View layout;
        private static BarcodeFragmentActivity thisInstance;

        public override View OnCreateView(View parent, string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(parent, name, context, attrs);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            thisInstance = this;
            SetContentView(Resource.Layout.Barcode_Capture);

            mPreview = (CameraSourcePreview)FindViewById(Resource.Id.preview);
            mGraphicOverlay = (GraphicOverlay)FindViewById(Resource.Id.graphicOverlay);
            getsureDetector = new GestureDetector(this, new CaptureGestureListener());
            scaleGestureDetector = new ScaleGestureDetector(this, new ScaleListener());

            bool autoFocus = Intent.GetBooleanExtra(AutoFocus, false);
            bool useFlash = Intent.GetBooleanExtra(UseFlash, false);

            //createCameraSource(autoFocus, useFlash);

            int rc = (int)ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera);
            if (rc == (int)Permission.Granted)
            {
                createCameraSource(autoFocus, useFlash);
            }
            else
            {
                requestCameraPermission();

            }

            Snackbar.Make(mGraphicOverlay, "Tap to capture. Pinch Strech to Zoom", Snackbar.LengthLong).Show();
        }

        private void requestCameraPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera };
            Console.WriteLine(ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera));
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                
                Snackbar.Make(layout, "Require Camera Permions To Read Barcodes", Snackbar.LengthIndefinite)
                    .SetAction("Ok", new Action<View> (delegate(View obj)
                    {
                        ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM);
                    }
                        )).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM);
            }

            //Activity thisActivity = this;
            //View.IOnClickListener listener = new View.IOnClickListener;
            
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            bool b = scaleGestureDetector.OnTouchEvent(e);
            bool c = getsureDetector.OnTouchEvent(e);
            return b || c || base.OnTouchEvent(e);
        }

        private void createCameraSource(bool autoFocus, bool useFlash)
        {
            Console.WriteLine("Creating the camera source");
            Context context = this;

            BarcodeDetector barcodeDetector = new BarcodeDetector.Builder(context).Build();
            BarcodeTrackerFactory barcodeFactory = new BarcodeTrackerFactory(mGraphicOverlay);
            barcodeDetector.SetProcessor(new MultiProcessor.Builder(barcodeFactory).Build());

            if (!barcodeDetector.IsOperational)
            {
                IntentFilter lowstorageFilter = new IntentFilter(Intent.ActionDeviceStorageLow);
                bool hasLowStorage = RegisterReceiver(null, lowstorageFilter) != null;

                if (hasLowStorage)
                {
                    Toast.MakeText(this, "Low Storage Error", ToastLength.Long);
                }
            }

            StockApp.UI.CameraSource.Builder builder = new StockApp.UI.CameraSource.Builder(base.ApplicationContext, barcodeDetector)
                .setFacing(StockApp.UI.CameraSource.CAMERA_FACING_BACK)
                .setRequestedPreviewSize(1600, 1024)
                .setRequestedFps(15.0f);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            { 
                mCameraSource = builder.setFocusMode(autoFocus ? Camera.Parameters.FocusModeContinuousPicture : null).build();
            }

            mCameraSource = builder.setFlashMode(useFlash ? Camera.Parameters.FlashModeTorch : null).build();
        }

        private void startCameraSource()
        {
            int code = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(ApplicationContext);

            if(code != ConnectionResult.Success)
            {
                Dialog dig = GoogleApiAvailability.Instance.GetErrorDialog(this, code, RC_HANDLE_GMS);
                dig.Show();
            }

            if (mCameraSource != null)
            {
                try
                {
                    Console.WriteLine("Starting the Camera Inside mPreview");
                    mPreview.start(mCameraSource, mGraphicOverlay);
                }
                catch (InvalidOperationException)
                {
                    mCameraSource.release();
                    mCameraSource = null;
                }
            }
        }

        private bool OnTap(float rawX, float rawY)
        {
            Finish();
            int[] location = new int[2];
            mGraphicOverlay.GetLocationOnScreen(location);
            float x = (rawX - location[0]);
            float y = (rawY - location[1]);

            Barcode best = null;
            float bestDistance = float.MaxValue;

            foreach (GraphicOverlay.Graphic graphic in mGraphicOverlay.getGraphics())
            {
                Barcode barcode = graphic.GetBarcode();

                if(barcode.BoundingBox.Contains((int)x,(int)y))
                {
                    best = barcode;
                    break;
                }

                float dx = x - barcode.BoundingBox.CenterX();
                float dy = y - barcode.BoundingBox.CenterY();

                float distance = (dx * dx) + (dy * dy);

                if ( distance > bestDistance)
                {
                    best = barcode;
                    bestDistance = distance;
                }

                if (best != null)
                {
                    Intent data = new Intent();
                    data.PutExtra(BarcodeObject, best);
                    SetResult(CommonStatusCodes.Success, data);
                    Finish();
                    return true;
                }
                
            }
            return false;

        }

        protected override void OnResume()
        {
            base.OnResume();
            Console.WriteLine("We have resumed");
            startCameraSource();
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (mPreview != null)
            {
                mPreview.stop();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(mPreview != null)
            {
                mPreview.release();
            }
        }

        private class CaptureGestureListener : GestureDetector.SimpleOnGestureListener
        {
            public override bool OnSingleTapConfirmed(MotionEvent e)
            {
                return thisInstance.OnTap(e.RawX, e.RawY) || base.OnSingleTapConfirmed(e);
            }
        }

        private class ScaleListener : Java.Lang.Object, ScaleGestureDetector.IOnScaleGestureListener
        {

            public bool OnScale(ScaleGestureDetector detector)
            {
                return false;
            }

            public bool OnScaleBegin(ScaleGestureDetector detector)
            {
                return true;
            }

            public void OnScaleEnd(ScaleGestureDetector detector)
            {
            }
        }
    }
}
