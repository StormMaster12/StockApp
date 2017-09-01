using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Vision;
using Android.Util;
using Android.Support.Annotation;
using Android.Gms.Common.Images;
using Android.Content.Res;
using Android.Graphics;

namespace StockApp.UI
{
    class CameraSourcePreview<T> : ViewGroup where T : GraphicOverlay<T>.Graphic
    {
        private static string TAG = "CameraSourcePreview";

        private Context mContext;
        private SurfaceView mSurfaceView;
        public bool mStartRequested;
        private bool mSurfaceAvaialbe;

        private Android.Gms.Vision.CameraSource mCameraSource;
        private GraphicOverlay<T> mOverlay;

        private static CameraSourcePreview<T> Instance { get; set; }
        
        public CameraSourcePreview(Context context, IAttributeSet attrs) : base(context,attrs)
        {
            Instance = this;

            mContext = context;
            mStartRequested = false;
            mSurfaceAvaialbe = false;

            SurfaceCallback instance = new SurfaceCallback();

            mSurfaceView = new SurfaceView(context);
            mSurfaceView.Holder.AddCallback(instance);
            AddView(mSurfaceView);
        }


        public void start(Android.Gms.Vision.CameraSource cameraSource)
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

        public void start(Android.Gms.Vision.CameraSource cameraSource, GraphicOverlay<T> graphicOverlay)
        {
            mOverlay = graphicOverlay;
            start(cameraSource);
        }

        public void stop()
        {
            if(mCameraSource != null)
            {
                mCameraSource.Stop();
            }
        }

        public void release()
        {
            if(mCameraSource != null)
            {
                mCameraSource.Release();
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
            if (mStartRequested && mSurfaceAvaialbe)
            {
                mCameraSource.Start(mSurfaceView.Holder);
                if (mOverlay != null)
                {
                    Android.Gms.Common.Images.Size size = mCameraSource.PreviewSize;
                    int min = Math.Min(size.Width, size.Height);
                    int max = Math.Max(size.Width, size.Width);
                    
                    if(isPortaitMode())
                    {
                        mOverlay.setCameraInfo(min, max, (int)mCameraSource.CameraFacing);
                    }
                    else
                    {
                        mOverlay.setCameraInfo(max, min, (int)mCameraSource.CameraFacing);
                    }
                    mOverlay.Clear();
                }
                mStartRequested = false;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int intWidth = 320;
            int intHeight = 240;

            if(mCameraSource != null)
            {
                Android.Gms.Common.Images.Size size = mCameraSource.PreviewSize;

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

            try
            {
                startIfReady();
            }
            catch (Exception e)
            {
                Log.Debug("Something went wrong", e.ToString());
            }
        }


        private class SurfaceCallback : ISurfaceHolderCallback
        {
            public IntPtr Handle => throw new NotImplementedException();

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