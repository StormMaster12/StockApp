using System;

using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Util;
using Android.Graphics;

namespace StockApp.UI
{
    [Register("stockapp.stockapp.ui.CameraSourcePreview")]
    class CameraSourcePreview : ViewGroup
    {
        private static string TAG = "CameraSourcePreview";

        private Context mContext;
        private SurfaceView mSurfaceView;
        public bool mStartRequested;
        private bool mSurfaceAvaialbe;

        private CameraSource mCameraSource;
        private GraphicOverlay mOverlay;

        private static CameraSourcePreview Instance { get; set; }

        public CameraSourcePreview(Context context, IAttributeSet attrs) : base(context,attrs)
        {
            
            Console.WriteLine("Main Constructor Started");
            mContext = context;

            mSurfaceView = new SurfaceView(context);
            AddView(mSurfaceView);

            SurfaceCallback instance = new SurfaceCallback();
            mSurfaceView.Holder.AddCallback(instance);
            


            mStartRequested = false;
            mSurfaceAvaialbe = false;
        }

        public void start(CameraSource cameraSource)
        {
            if (cameraSource == null)
            {
                stop();
            }

            mCameraSource = cameraSource;

            if(mCameraSource != null)
            {
                mStartRequested = true;
                startIfReady();
            }
        }

        public void start(CameraSource cameraSource, GraphicOverlay graphicOverlay)
        {
            mSurfaceAvaialbe = true;
            mOverlay = graphicOverlay;
            start(cameraSource);
        }

        public void stop()
        {
            if(mCameraSource != null)
            {
                mCameraSource.stop();
            }
        }

        public void release()
        {
            if(mCameraSource != null)
            {
                mCameraSource.release();
                mCameraSource = null;
            }
        }

        private bool isPortaitMode()
        {
            int orientation = (int)mContext.Resources.Configuration.Orientation;
            if (orientation == (int)Android.Content.Res.Orientation.Landscape)
            {
                return false;
            }
            else if (orientation == (int)Android.Content.Res.Orientation.Portrait)
            {
                return true;
            }

            return false;
        }

        private void startIfReady()
        {
            Console.WriteLine("mStartRequested : {0} , mSurfaceAvailabe : {1}", mStartRequested, mSurfaceAvaialbe);

            //mStartRequested = true;
            if (mStartRequested && mSurfaceAvaialbe)
            {
                Console.WriteLine("Before Camera Call, @110");
                mCameraSource.start(mSurfaceView.Holder);
                Console.WriteLine("After Camera Call, @112");
                if (mOverlay != null)
                {
                    Size size = CameraSource.mPreviewSize;
                    int min = Math.Min(size.Width, size.Height);
                    int max = Math.Max(size.Width, size.Width);
                    
                    if(isPortaitMode())
                    {
                        mOverlay.setCameraInfo(min, max, (int)mCameraSource.getCameraFacing());
                    }
                    else
                    {
                        mOverlay.setCameraInfo(max, min, (int)mCameraSource.getCameraFacing());
                    }
                    //mOverlay.Clear();
                }
                mStartRequested = false;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {

            //AddView(mSurfaceView);
            if (mSurfaceView != null)
            {
                Console.WriteLine("MSurfaceView Is not null");
            }
            else
            {
                throw new Exception("Surface View Is Null");
            }
            //mCameraSource = new CameraSource(Context,holder);
            Console.WriteLine("Drawing the Layout");
            int intWidth = 320;
            int intHeight = 240;

            Console.WriteLine(mCameraSource);

            if(mCameraSource != null)
            {
                Size size = CameraSource.mPreviewSize;

                Console.Write(size);

                if(size != null)
                {
                    intWidth = size.Width;
                    intHeight = size.Height;
                }
            }

            if(isPortaitMode())
            {
                int tmp = intWidth;
                intHeight = intWidth;
                intWidth = tmp;
            }

            int layoutWidth =  l - r;
            int layoutHeight = t - b;

            int childWidth = layoutWidth;
            int childHeight = (int)(((float)layoutWidth / (float) intHeight)*intWidth);

            if (childHeight > layoutWidth)
            {
                childHeight = layoutHeight;
                childWidth = (int)(((float)layoutHeight / (float)intHeight) * intWidth);

            }
            
            for (int i =0 ; i < ChildCount; i++ )
            {
                GetChildAt(i).Layout(0, 0, childWidth, childHeight);
            }

            Console.WriteLine("Starting If Ready");

            try
            {
                startIfReady();
            }
            catch (Exception e)
            {
                Log.Debug("Something went wrong", e.ToString());
            }
        }

        private class SurfaceCallback :Java.Lang.Object, ISurfaceHolderCallback
        {
            public SurfaceCallback()
            {
            }

            //public IntPtr Handle => throw new NotImplementedException();

            public void Dispose()
            {
            }

            public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
            {
            }

            public void SurfaceCreated(ISurfaceHolder holder)
            {
                Instance.mSurfaceAvaialbe = true;
                try
                {
                    Instance.startIfReady(); 
                }
                catch (Exception e)
                {
                    Log.Debug("Something went wrong",e.ToString());
                }
            }

            public void SurfaceDestroyed(ISurfaceHolder holder)
            {
                Instance.mSurfaceAvaialbe = false;
            }
        }
    }
}
