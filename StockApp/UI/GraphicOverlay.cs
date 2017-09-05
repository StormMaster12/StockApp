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
using Android.Gms.Vision;
using Android.Util;
using Android.Graphics;
using Java.Util;


namespace StockApp.UI
{
    [Register("stockapp.stockapp.ui.GraphicOverlay")]
    class GraphicOverlay : View
    {
        private Object mLock = new object();
        private int mPreviewWidth { get; set; }
        private float mWidthScaleFactor { get; set; } = 1.0f;
        private int mPreviewHeight;
        private float mHeightScaleFactor { get; set; } = 1.0f;
        private int mFacing { get; set; } = (int)CameraFacing.Back;
        private HashSet<GraphicOverlay.Graphic> mGraphics = new HashSet<GraphicOverlay.Graphic>();

        public GraphicOverlay(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public void Clear()
        {
            lock(mLock)
            {
                mGraphics.Clear();
            }
        }

        public void Add(Graphic graphic)
        {
            lock(mLock)
            {
                mGraphics.Add(graphic);
            }
            PostInvalidate();
        }

        public void Remove(Graphic graphic)
        {
            lock(mLock)
            {
                mGraphics.Remove(graphic);
            }
            PostInvalidate();
        }

        public List<Graphic> getGraphics()
        {
            lock(mLock)
            {
                return mGraphics.ToList();
            }
        }

        public void setCameraInfo(int previewWidth, int previewHeight, int facing)
        {
            lock(mLock)
            {
                mPreviewHeight = previewHeight;
                mPreviewWidth = previewWidth;
                mFacing = facing;
            }

            PostInvalidate();
        }

        protected void onDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            lock(mLock)
            {
                if(mPreviewWidth !=0 && mPreviewHeight !=0)
                {
                    mWidthScaleFactor = (float)canvas.Width / (float)mPreviewWidth;
                    mHeightScaleFactor = (float)canvas.Height / (float)mPreviewHeight;
                }

                foreach (Graphic graphic in mGraphics)
                {
                    graphic.Draw(canvas);
                }
            }
        }

        public abstract class Graphic
        {
            private GraphicOverlay mOverlay;
            public Graphic(GraphicOverlay overlay)
            {
                mOverlay = overlay;
            }

            public abstract void Draw(Canvas canvas);

            public float scaleX(float horizontal) { return horizontal * mOverlay.mWidthScaleFactor; }
            public float scaleY(float vertical) { return vertical * mOverlay.mHeightScaleFactor; }

            public float translateX(float x)
            {
                float scale = scaleX(x);

                if(mOverlay.mFacing == (int)CameraFacing.Front)
                {
                    return mOverlay.Width - scale;
                }
                else
                {
                    return scale;
                }
            }

            public float translateY(float y) { return scaleY(y); }

            public void postInvalidate() { mOverlay.PostInvalidate(); }
        }



    }
}
